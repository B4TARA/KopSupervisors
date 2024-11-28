using KOP.BLL.Interfaces;
using KOP.Common.DTOs;
using KOP.Common.DTOs.AssessmentDTOs;
using KOP.Common.Enums;
using KOP.Common.Interfaces;
using KOP.DAL.Entities.AssessmentEntities;
using KOP.DAL.Interfaces;

namespace KOP.BLL.Services
{
    public class AssessmentService : IAssessmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMappingService _mappingService;

        public AssessmentService(IUnitOfWork unitOfWork, IMappingService mappingService)
        {
            _unitOfWork = unitOfWork;
            _mappingService = mappingService;
        }

        // Получить объект качественной оценки
        public async Task<IBaseResponse<AssessmentDTO>> GetAssessment(int id, SystemStatuses? systemStatus = null)
        {
            try
            {
                var assessment = await _unitOfWork.Assessments.GetAsync(x => x.Id == id, includeProperties: new string[]
                {
                    "AssessmentType.AssessmentMatrix.Elements",
                    "AssessmentResults.AssessmentResultValues",
                    "AssessmentResults.Judge",
                    "AssessmentResults.Judged"
                });

                if (assessment == null)
                {
                    return new BaseResponse<AssessmentDTO>()
                    {
                        Description = $"[AssessmentService.GetAssessment] : Качественная оценка с id = {id} не найдена",
                        StatusCode = StatusCodes.EntityNotFound,
                    };
                }

                var dto = new AssessmentDTO
                {
                    Id = id,
                    Number = assessment.Number,
                    EmployeeId = assessment.EmployeeId,
                    SystemStatus = assessment.SystemStatus,
                    NextAssessmentDate = assessment.NextAssessmentDate,
                };

                var results = new List<AssessmentResult>();

                if (systemStatus == null)
                {
                    results = assessment.AssessmentResults;
                }
                else
                {
                    results = assessment.AssessmentResults.Where(x => x.SystemStatus == systemStatus).ToList();
                }

                foreach (var result in results)
                {
                    var resultDTO = _mappingService.CreateAssessmentResultDTO(result, assessment.AssessmentType.AssessmentMatrix) ;

                    if (resultDTO.StatusCode != StatusCodes.OK || resultDTO.Data == null)
                    {
                        return new BaseResponse<AssessmentDTO>()
                        {
                            Description = resultDTO.Description,
                            StatusCode = resultDTO.StatusCode,
                        };
                    }

                    dto.AssessmentResults.Add(resultDTO.Data);

                    dto.AverageValue += resultDTO.Data.Sum;
                }

                dto.AverageValue = dto.AverageValue / dto.AssessmentResults.Count();

                return new BaseResponse<AssessmentDTO>()
                {
                    Data = dto,
                    StatusCode = StatusCodes.OK
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<AssessmentDTO>()
                {
                    Description = $"[AssessmentService.GetEmployeeAssessment] : {ex.Message}",
                    StatusCode = StatusCodes.InternalServerError,
                };
            }
        }

        // Получить объект результата качественной оценки по оценщику
        public async Task<IBaseResponse<AssessmentResultDTO>> GetAssessmentResult(int judgeId, int assessmentId)
        {
            try
            {
                var result = await _unitOfWork.AssessmentResults
                    .GetAsync(
                        x => x.AssessmentId == assessmentId && x.JudgeId == judgeId,
                        includeProperties: new string[]
                        {
                            "Judge",
                            "Judged",
                            "AssessmentResultValues",
                            "Assessment.AssessmentType.AssessmentMatrix.Elements"
                        });

                if (result == null)
                {
                    return new BaseResponse<AssessmentResultDTO>()
                    {
                        Description = $"[AssessmentService.GetAssessmentResult] : Результат качественной оценки не найден",
                        StatusCode = StatusCodes.EntityNotFound,
                    };
                }

                var dto = _mappingService.CreateAssessmentResultDTO(result, result.Assessment.AssessmentType.AssessmentMatrix);

                if (dto.StatusCode != StatusCodes.OK || dto.Data == null)
                {
                    return new BaseResponse<AssessmentResultDTO>()
                    {
                        Description = dto.Description,
                        StatusCode = dto.StatusCode,
                    };
                }

                return new BaseResponse<AssessmentResultDTO>()
                {
                    Data = dto.Data,
                    StatusCode = StatusCodes.OK
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<AssessmentResultDTO>()
                {
                    Description = $"[AssessmentService.GetAssessmentResult] : {ex.Message}",
                    StatusCode = StatusCodes.InternalServerError,
                };
            }
        }

        // Получить объект типа качественной оценки по оценщику
        public async Task<IBaseResponse<AssessmentTypeDTO>> GetAssessmentType(int employeeId, int assessmentTypeId)
        {
            try
            {
                var assessmentType = await _unitOfWork.AssessmentTypes
                    .GetAsync(
                        x => x.Id == assessmentTypeId,
                        includeProperties: new string[]
                        {
                            "AssessmentMatrix.Elements"
                        });

                if (assessmentType == null)
                {
                    return new BaseResponse<AssessmentTypeDTO>()
                    {
                        Description = $"[EmployeeService.GetAssessmentType] : Тип количественной оценки с id = {assessmentTypeId} не найден",
                        StatusCode = StatusCodes.EntityNotFound,
                    };
                }

                var assessments = await _unitOfWork.Assessments
                    .GetAllAsync(
                        x => x.AssessmentTypeId == assessmentTypeId && x.EmployeeId == employeeId,
                        includeProperties: new string[]
                        {
                            "AssessmentResults.Judge",
                            "AssessmentResults.Judged",
                            "AssessmentResults.AssessmentResultValues"
                        });

                var assessmentTypeDTO = new AssessmentTypeDTO
                {
                    Id = assessmentType.Id,
                    Name = assessmentType.Name,
                    EmployeeId = employeeId,
                };

                foreach (var assessment in assessments.OrderBy(x => x.Number))
                {
                    var assessmentDTO = new AssessmentDTO
                    {
                        Id = assessment.Id,
                        Number = assessment.Number,
                        EmployeeId = assessment.EmployeeId,
                        SystemStatus = assessment.SystemStatus,
                        StartDate = assessment.StartDate,
                        EndDate = assessment.EndDate,
                        NextAssessmentDate = assessment.NextAssessmentDate,
                    };

                    // Для каждого завершенного результата качественной оценки
                    foreach (var assessmentResult in assessment.AssessmentResults.Where(x => x.SystemStatus == SystemStatuses.COMPLETED))
                    {
                        var assessmentResultDTO = new AssessmentResultDTO
                        {
                            Id = assessmentResult.Id,
                            SystemStatus = assessmentResult.SystemStatus,
                            Sum = assessmentResult.AssessmentResultValues.Sum(x => x.Value),
                        };

                        assessmentResultDTO.Judge = new EmployeeDTO
                        {
                            Id = assessmentResult.Judge.Id,
                            ImagePath = assessmentResult.Judge.ImagePath,
                            FullName = assessmentResult.Judge.FullName,
                        };

                        assessmentResultDTO.Judged = new EmployeeDTO
                        {
                            Id = assessment.EmployeeId,
                        };

                        foreach (var assessmentResultValue in assessmentResult.AssessmentResultValues)
                        {
                            assessmentResultDTO.Values.Add(new AssessmentResultValueDTO
                            {
                                Value = assessmentResultValue.Value,
                                AssessmentMatrixRow = assessmentResultValue.AssessmentMatrixRow,
                            });
                        }

                        foreach (var assessmentMatrixElement in assessmentType.AssessmentMatrix.Elements)
                        {
                            assessmentResultDTO.Elements.Add(new AssessmentMatrixElementDTO
                            {
                                Row = assessmentMatrixElement.Row,
                                Value = assessmentMatrixElement.Value,
                            });
                        }

                        assessmentResultDTO.ElementsByRow = assessmentResultDTO.Elements.GroupBy(x => x.Row).ToList();

                        assessmentDTO.AssessmentResults.Add(assessmentResultDTO);
                    }


                    assessmentTypeDTO.Assessments.Add(assessmentDTO);
                }

                return new BaseResponse<AssessmentTypeDTO>()
                {
                    Data = assessmentTypeDTO,
                    StatusCode = StatusCodes.OK
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<AssessmentTypeDTO>()
                {
                    Description = $"[EmployeeService.GetEmployeeAssessmentType] : {ex.Message}",
                    StatusCode = StatusCodes.InternalServerError,
                };
            }
        }

        // Получить все типы качественных оценок по сотруднику
        public async Task<IBaseResponse<List<AssessmentTypeDTO>>> GetAssessmentTypes(int employeeId)
        {
            try
            {
                // Получаем все качественные оценки сотрудника
                var assessments = await _unitOfWork.Assessments.GetAllAsync(x => x.EmployeeId == employeeId, includeProperties: new string[]
                {
                    "AssessmentType",
                });

                // Группируем массив оценок по "Типу оценок"
                var assessmentTypes = assessments.GroupBy(x => x.AssessmentType);

                var assessmentTypeDTOs = new List<AssessmentTypeDTO>();

                foreach (var assessmentType in assessmentTypes)
                {
                    var assessmentTypeDTO = new AssessmentTypeDTO
                    {
                        Id = assessmentType.Key.Id,
                        Name = assessmentType.Key.Name,
                        EmployeeId = employeeId,
                    };

                    assessmentTypeDTOs.Add(assessmentTypeDTO);
                }

                return new BaseResponse<List<AssessmentTypeDTO>>()
                {
                    Data = assessmentTypeDTOs,
                    StatusCode = StatusCodes.OK
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<List<AssessmentTypeDTO>>()
                {
                    Description = $"[AssessmentService.GetAssessmentTypes] : {ex.Message}",
                    StatusCode = StatusCodes.InternalServerError,
                };
            }
        }

        // Проверка необходимости проведения качественной оценки в пределах всех оценок (разных типов)
        public async Task<IBaseResponse<bool>> IsActiveAssessment(int judgeId, int judgedId)
        {
            try
            {
                var result = await _unitOfWork.AssessmentResults.GetAsync(x => x.JudgeId == judgeId && x.JudgedId == judgedId && x.SystemStatus == SystemStatuses.PENDING);

                if (result == null)
                {
                    return new BaseResponse<bool>()
                    {
                        Data = false,
                        StatusCode = StatusCodes.OK
                    };
                }

                return new BaseResponse<bool>()
                {
                    Data = true,
                    StatusCode = StatusCodes.OK
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<bool>()
                {
                    Description = $"[AssessmentService.IsActiveAssessment] : {ex.Message}",
                    StatusCode = StatusCodes.InternalServerError,
                };
            }
        }

        // Проверка необходимости проведения качественной оценки в пределах определенной оценки (определенного типа)
        public async Task<IBaseResponse<bool>> IsActiveAssessment(int judgeId, int judgedId, int assessmentId)
        {
            try
            {
                var result = await _unitOfWork.AssessmentResults.GetAsync(x => x.JudgeId == judgeId && x.JudgedId == judgedId && x.AssessmentId == assessmentId && x.SystemStatus == SystemStatuses.PENDING);

                if (result == null)
                {
                    return new BaseResponse<bool>()
                    {
                        Data = false,
                        StatusCode = StatusCodes.OK
                    };
                }

                return new BaseResponse<bool>()
                {
                    Data = true,
                    StatusCode = StatusCodes.OK
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<bool>()
                {
                    Description = $"[AssessmentService.IsActiveAssessment] : {ex.Message}",
                    StatusCode = StatusCodes.InternalServerError,
                };
            }
        }
    }
}