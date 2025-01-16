using KOP.BLL.Interfaces;
using KOP.Common.DTOs;
using KOP.Common.DTOs.AssessmentDTOs;
using KOP.Common.Enums;
using KOP.Common.Interfaces;
using KOP.DAL.Interfaces;

namespace KOP.BLL.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAssessmentService _assessmentService;
        private readonly IMappingService _mappingService;

        public EmployeeService(IUnitOfWork unitOfWork, IAssessmentService assessmentService, IMappingService mappingService)
        {
            _unitOfWork = unitOfWork;
            _assessmentService = assessmentService;
            _mappingService = mappingService;
        }

        // Получить информацию о сотруднике по id
        public async Task<IBaseResponse<EmployeeDTO>> GetEmployee(int id)
        {
            try
            {
                // Получаем сотрудника по идентификатору
                var employee = await _unitOfWork.Employees.GetAsync(x => x.Id == id, includeProperties: new string[]
                {
                    "Grades.Qualification",
                    "Grades.ValueJudgment",
                    "Grades.Marks",
                    "Grades.Kpis",
                    "Grades.Projects",
                    "Grades.StrategicTasks",
                    "Grades.TrainingEvents",
                });

                if (employee == null)
                {
                    return new BaseResponse<EmployeeDTO>()
                    {
                        Description = $"[EmployeeService.GetEmployee] : Пользователь с id = {id} не найден",
                        StatusCode = StatusCodes.EntityNotFound,
                    };
                }

                var employeeDTO = _mappingService.CreateEmployeeDTO(employee);

                if (employeeDTO.StatusCode != StatusCodes.OK || employeeDTO.Data == null)
                {
                    return new BaseResponse<EmployeeDTO>()
                    {
                        Description = employeeDTO.Description,
                        StatusCode = employeeDTO.StatusCode,
                    };
                }

                return new BaseResponse<EmployeeDTO>()
                {
                    Data = employeeDTO.Data,
                    StatusCode = StatusCodes.OK
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<EmployeeDTO>()
                {
                    Description = $"[EmployeeService.GetEmployee] : {ex.Message}",
                    StatusCode = StatusCodes.InternalServerError,
                };
            }
        }

        // Получить последнюю качественную оценку каждого типа по id сотрудника
        public async Task<IBaseResponse<List<AssessmentDTO>>> GetEmployeeLastAssessments(int employeeId, int supervisorId)
        {
            try
            {
                // Получаем сотрудника по идентификатору
                var employee = await _unitOfWork.Employees.GetAsync(x => x.Id == employeeId, includeProperties: new string[] { "Assessments.AssessmentType" });

                if (employee == null)
                {
                    return new BaseResponse<List<AssessmentDTO>>()
                    {
                        Description = $"[EmployeeService.GetEmployeeLastAssessments] : Пользователь с id = {employeeId} не найден",
                        StatusCode = StatusCodes.EntityNotFound,
                    };
                }

                // Группируем массив оценок по "Типу оценок"
                var assessmentsByAssessmentType = employee.Assessments.GroupBy(x => x.AssessmentType);

                var lastAssessmentDTOs = new List<AssessmentDTO>();

                // Достаем последнюю оценку для каждого типа (группы)
                foreach (var assessmentGroup in assessmentsByAssessmentType)
                {
                    var lastAssessment = assessmentGroup.OrderByDescending(x => x.DateOfCreation).First();

                    var lastAssessmentDTO = new AssessmentDTO
                    {
                        Id = lastAssessment.Id,
                        EmployeeId = employeeId,
                        Number = lastAssessment.Number,
                        AssessmentTypeName = assessmentGroup.Key.Name,
                    };

                    var response = await _assessmentService.IsActiveAssessment(supervisorId, employeeId, lastAssessment.Id);

                    if (response.StatusCode != StatusCodes.OK)
                    {
                        return new BaseResponse<List<AssessmentDTO>>()
                        {
                            Description = response.Description,
                            StatusCode = response.StatusCode,
                        };
                    }

                    lastAssessmentDTO.IsActiveAssessment = response.Data;

                    lastAssessmentDTOs.Add(lastAssessmentDTO);
                }

                return new BaseResponse<List<AssessmentDTO>>()
                {
                    Data = lastAssessmentDTOs,
                    StatusCode = StatusCodes.OK
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<List<AssessmentDTO>>()
                {
                    Description = $"[EmployeeService.GetEmployeeLastAssessments] : {ex.Message}",
                    StatusCode = StatusCodes.InternalServerError,
                };
            }
        }

        // Получить объект качественной самооценки
        public async Task<IBaseResponse<AssessmentResultDTO>> GetSelfAssessment(int employeeId, int assessmentId)
        {
            try
            {
                var dto = new AssessmentResultDTO();

                // Получаем объект самооценки по assessmentId
                var assessment = await _unitOfWork.Assessments.GetAsync(x => x.Id == assessmentId);

                // Получаем тип качественной оценки, к которому относится данная оценка
                var assessmentType = await _unitOfWork.AssessmentTypes.GetAsync(x => x.Id == assessment.AssessmentTypeId);

                //Получаем результат самооценки
                var selfResult = await _unitOfWork.AssessmentResults.GetAsync(x => x.JudgedId == employeeId && x.JudgeId == employeeId && x.AssessmentId == assessmentId, includeProperties: new string[]
                {
                    "AssessmentResultValues"
                });

                if (selfResult == null)
                {
                    dto.SystemStatus = SystemStatuses.NOT_EXIST;

                    return new BaseResponse<AssessmentResultDTO>()
                    {
                        Data = dto,
                        StatusCode = StatusCodes.OK
                    };
                }
                else if (selfResult.SystemStatus == SystemStatuses.COMPLETED)
                {
                    // Получаем значения результатов самооценки
                    var selfValues = selfResult.AssessmentResultValues;

                    foreach (var selfValue in selfValues)
                    {
                        dto.Values.Add(new AssessmentResultValueDTO
                        {
                            Value = selfValue.Value,
                            AssessmentMatrixRow = selfValue.AssessmentMatrixRow,
                        });

                        // Создаем нужное количество объектов для последующего заполения результатами оценщиков
                        dto.AverageValues.Add(new AssessmentResultValueDTO
                        {
                            Value = 0,
                            AssessmentMatrixRow = selfValue.AssessmentMatrixRow,
                        });
                    }

                    // Получаем результаты оценки по всем оценщикам, кроме самооценки
                    var results = await _unitOfWork.AssessmentResults.GetAllAsync(x => x.JudgedId == employeeId && x.AssessmentId == assessmentId, includeProperties: new string[]
                    {
                        "AssessmentResultValues"
                    });

                    foreach (var result in results)
                    {
                        // Получаем значения результатов оценщика
                        var values = result.AssessmentResultValues;

                        foreach (var value in values)
                        {
                            dto.AverageValues.First(x => x.AssessmentMatrixRow == value.AssessmentMatrixRow).Value += value.Value;
                        }
                    }

                    // Делим на количество оценщиков для получения среднего результата
                    foreach (var averageValue in dto.AverageValues)
                    {
                        var sumValue = dto.AverageValues.First(x => x.AssessmentMatrixRow == averageValue.AssessmentMatrixRow).Value;

                        var result = Math.Round((double)sumValue / results.Count(), 1);

                        dto.AverageValues.First(x => x.AssessmentMatrixRow == averageValue.AssessmentMatrixRow).Value = result;

                        dto.AverageResult += result;
                    }
                }

                dto.Judge = new EmployeeDTO
                {
                    Id = employeeId,
                };

                dto.Judged = new EmployeeDTO
                {
                    Id = employeeId,
                };

                dto.Id = selfResult.Id;
                dto.AssessmentId = selfResult.AssessmentId;
                dto.SystemStatus = selfResult.SystemStatus;

                // Получаем матрицу качественной оценки, которая относится к данному типу
                var assessmentMatrix = await _unitOfWork.AssessmentMatrices.GetAsync(x => x.Id == assessmentType.AssessmentMatrixId, includeProperties: new string[]
                {
                    "Elements"
                });

                dto.MinValue = assessmentMatrix.MinAssessmentMatrixResultValue;
                dto.MaxValue = assessmentMatrix.MaxAssessmentMatrixResultValue;

                var dtos = new List<AssessmentMatrixElementDTO>();

                foreach (var element in assessmentMatrix.Elements)
                {
                    dtos.Add(new AssessmentMatrixElementDTO
                    {
                        Row = element.Row,
                        Value = element.Value,
                    });
                }

                dto.Elements = dtos;

                dto.ElementsByRow = dto.Elements.GroupBy(x => x.Row).ToList();

                return new BaseResponse<AssessmentResultDTO>()
                {
                    Data = dto,
                    StatusCode = StatusCodes.OK
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<AssessmentResultDTO>()
                {
                    Description = $"[EmployeeService.GetSelfAssessment] : {ex.Message}",
                    StatusCode = StatusCodes.InternalServerError,
                };
            }
        }

        // Получить коллег сотрудника для качественной оценки
        public async Task<IBaseResponse<List<AssessmentResultDTO>>> GetColleagueAssessmentResults(int employeeId)
        {
            try
            {
                //Получаем назначеные результаты оценок коропративных компетенций
                var assessmentResults = await _unitOfWork.AssessmentResults.GetAllAsync(x => x.JudgeId == employeeId && x.JudgedId != employeeId && x.SystemStatus == SystemStatuses.PENDING);

                var dtos = new List<AssessmentResultDTO>();

                // Получаем каждого оцениваемого коллегу по идентификатору
                foreach (var assessmentResult in assessmentResults)
                {
                    // Получаем объект оценки по assessmentId
                    var assessment = await _unitOfWork.Assessments.GetAsync(x => x.Id == assessmentResult.AssessmentId, includeProperties: new string[]
                    {
                        "AssessmentResults.Judged",
                        "AssessmentResults.AssessmentResultValues",
                        "AssessmentType.AssessmentMatrix.Elements"
                    });

                    var dto = new AssessmentResultDTO
                    {
                        Id = assessmentResult.Id,
                        AssessmentId = assessmentResult.AssessmentId,
                        SystemStatus = assessmentResult.SystemStatus,
                        Sum = assessmentResult.AssessmentResultValues.Sum(x => x.Value),
                        TypeName = assessment.AssessmentType.Name,
                        MinValue = assessment.AssessmentType.AssessmentMatrix.MinAssessmentMatrixResultValue,
                        MaxValue = assessment.AssessmentType.AssessmentMatrix.MaxAssessmentMatrixResultValue,
                    };

                    dto.Judge = new EmployeeDTO
                    {
                        Id = employeeId,
                    };

                    dto.Judged = new EmployeeDTO
                    {
                        Id = assessment.EmployeeId,
                        ImagePath = assessmentResult.Judged.ImagePath,
                        FullName = assessmentResult.Judged.FullName,
                    };

                    foreach (var value in assessmentResult.AssessmentResultValues)
                    {
                        dto.Values.Add(new AssessmentResultValueDTO
                        {
                            Value = value.Value,
                            AssessmentMatrixRow = value.AssessmentMatrixRow,
                        });
                    }

                    foreach (var element in assessment.AssessmentType.AssessmentMatrix.Elements)
                    {
                        dto.Elements.Add(new AssessmentMatrixElementDTO
                        {
                            Row = element.Row,
                            Value = element.Value,
                        });
                    }

                    dto.ElementsByRow = dto.Elements.GroupBy(x => x.Row).ToList();

                    dtos.Add(dto);
                }

                return new BaseResponse<List<AssessmentResultDTO>>()
                {
                    Data = dtos,
                    StatusCode = StatusCodes.OK
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<List<AssessmentResultDTO>>()
                {
                    Description = $"[EmployeeService.GetColleagueAssessmentResults] : {ex.Message}",
                    StatusCode = StatusCodes.InternalServerError,
                };
            }
        }

        // Оценить сотрудника (в том числе и самооценка)
        public async Task<IBaseResponse<object>> AssessEmployee(AssessEmployeeDTO assessEmployeeDTO)
        {
            try
            {
                // Находим объект оценки по id
                var assessmentResult = await _unitOfWork.AssessmentResults.GetAsync(x => x.Id == assessEmployeeDTO.AssessmentResultId);

                assessmentResult.ResultDate = DateOnly.FromDateTime(DateTime.Today);

                assessmentResult.SystemStatus = SystemStatuses.COMPLETED;

                // Создаем объекты результатов оценки
                for (int i = 0; i < assessEmployeeDTO.ResultValues.Count(); i++)
                {
                    assessmentResult.AssessmentResultValues.Add(new DAL.Entities.AssessmentEntities.AssessmentResultValue
                    {
                        Value = Convert.ToInt32(assessEmployeeDTO.ResultValues[i]),
                        AssessmentMatrixRow = (i + 1),
                    });
                }

                _unitOfWork.AssessmentResults.Update(assessmentResult);

                // Проверка на условия завершения оценки
                // ПОМЕНЯТЬ ЭТОТ КОСТЫЛЬ !!! (если уже есть в БД оценка одна (руководителя или самооценка), которая завершена
                var secondAssessmentResult = await _unitOfWork.AssessmentResults.GetAsync(x => x.AssessmentId == assessmentResult.AssessmentId && x.SystemStatus == SystemStatuses.COMPLETED);

                if (secondAssessmentResult == null)
                {
                    await _unitOfWork.CommitAsync();

                    return new BaseResponse<object>()
                    {
                        StatusCode = StatusCodes.OK
                    };
                }
                else
                {
                    var assessment = await _unitOfWork.Assessments.GetAsync(x => x.Id == assessmentResult.AssessmentId, includeProperties: new string[]
                    {
                        "AssessmentType",
                    });

                    assessment.SystemStatus = SystemStatuses.COMPLETED;

                    _unitOfWork.Assessments.Update(assessment);

                    await _unitOfWork.CommitAsync();
                }

                return new BaseResponse<object>()
                {
                    StatusCode = StatusCodes.OK
                };
                // // // // // // // // // // //
            }
            catch (Exception ex)
            {
                return new BaseResponse<object>()
                {
                    Description = $"[EmployeeService.AssessEmployee] : {ex.Message}",
                    StatusCode = StatusCodes.InternalServerError,
                };
            }
        }

        // Получить последнюю качественную оценку конкретного типа
        public async Task<IBaseResponse<AssessmentDTO>> GetLastAssessment(int employeeId, int assessmentTypeId)
        {
            try
            {
                // Получаем сотрудника по идентификатору
                var employee = await _unitOfWork.Employees.GetAsync(x => x.Id == employeeId, includeProperties: new string[]
                {
                    "Assessments.AssessmentType.AssessmentMatrix.Elements",
                    "Assessments.AssessmentResults.AssessmentResultValues",
                    "Assessments.AssessmentResults.Judge",
                    "Assessments.AssessmentResults.Judged"
                });

                if (employee == null)
                {
                    return new BaseResponse<AssessmentDTO>()
                    {
                        Description = $"[EmployeeService.GetLastAssessment] : Пользователь с id = {employeeId} не найден",
                        StatusCode = StatusCodes.EntityNotFound,
                    };
                }

                // Получаем последнюю оценку определеноого типа
                var lastAssessment = employee.Assessments.Where(x => x.AssessmentTypeId == assessmentTypeId).OrderByDescending(x => x.DateOfCreation).FirstOrDefault();

                if (lastAssessment == null)
                {
                    return new BaseResponse<AssessmentDTO>()
                    {
                        Description = $"[EmployeeService.GetLastAssessment] : Тип с id = {assessmentTypeId} не содержит последней оценки",
                        StatusCode = StatusCodes.EntityNotFound,
                    };
                }

                var lastAssessmentDtoResponse = _mappingService.CreateAssessmentDTO(lastAssessment);

                if (lastAssessmentDtoResponse.StatusCode != StatusCodes.OK || lastAssessmentDtoResponse.Data == null)
                {
                    return new BaseResponse<AssessmentDTO>()
                    {
                        Description = lastAssessmentDtoResponse.Description,
                        StatusCode = lastAssessmentDtoResponse.StatusCode,
                    };
                }

                return new BaseResponse<AssessmentDTO>()
                {
                    Data = lastAssessmentDtoResponse.Data,
                    StatusCode = StatusCodes.OK
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<AssessmentDTO>()
                {
                    Description = $"[EmployeeService.GetLastAssessment] : {ex.Message}",
                    StatusCode = StatusCodes.InternalServerError,
                };
            }
        }
    }
}