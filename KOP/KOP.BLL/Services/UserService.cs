using KOP.BLL.Interfaces;
using KOP.Common.Dtos;
using KOP.Common.Dtos.AssessmentDtos;
using KOP.Common.Dtos.GradeDtos;
using KOP.Common.Enums;
using KOP.Common.Interfaces;
using KOP.DAL.Entities.AssessmentEntities;
using KOP.DAL.Interfaces;

namespace KOP.BLL.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAssessmentService _assessmentService;
        private readonly IMappingService _mappingService;

        public UserService(IUnitOfWork unitOfWork, IAssessmentService assessmentService, IMappingService mappingService)
        {
            _unitOfWork = unitOfWork;
            _assessmentService = assessmentService;
            _mappingService = mappingService;
        }

        public async Task<UserDto> GetUser(int id)
        {

            var user = await _unitOfWork.Users.GetAsync(x => x.Id == id, includeProperties: new string[]
            {
                "Grades.Qualification",
                "Grades.ValueJudgment",
                "Grades.Marks",
                "Grades.Kpis",
                "Grades.Projects",
                "Grades.StrategicTasks",
                "Grades.TrainingEvents",
                "Grades.Assessments.AssessmentType.AssessmentMatrix",
            });

            if (user == null)
            {
                throw new Exception($"User with ID {id} not found.");
            }

            var userDto = _mappingService.CreateUserDto(user);

            return userDto;
        }

        public async Task<IEnumerable<GradeSummaryDto>> GetUserGradesSummaries(int employeeId)
        {

            var grades = await _unitOfWork.Grades.GetAllAsync(x => x.UserId == employeeId && x.SystemStatus == SystemStatuses.COMPLETED);
            var gradeSummaryDtoList = new List<GradeSummaryDto>();

            foreach (var grade in grades)
            {
                gradeSummaryDtoList.Add(new GradeSummaryDto
                {
                    Id = grade.Id,
                    Number = grade.Number,
                    StartDate = grade.StartDate,
                    EndDate = grade.EndDate,
                    DateOfCreation = grade.DateOfCreation,
                });
            }

            return gradeSummaryDtoList;
        }

        public async Task<List<AssessmentDto>> GetUserLastAssessmentsOfEachAssessmentType(int userId, int supervisorId)
        {
            var user = await _unitOfWork.Users.GetAsync(x => x.Id == userId, includeProperties: new string[]
            {
                    "Assessments.AssessmentType"
            });

            if (user == null)
            {
                throw new Exception($"User with ID {userId} not found.");
            }

            var userAssessmentTypes = user.Assessments.GroupBy(x => x.AssessmentType);
            var userLastAssessmentsOfEachType = new List<AssessmentDto>();

            foreach (var assessmentType in userAssessmentTypes)
            {
                var lastAssessment = assessmentType.OrderByDescending(x => x.DateOfCreation).First();

                var lastAssessmentDto = new AssessmentDto
                {
                    Id = lastAssessment.Id,
                    UserId = userId,
                    Number = lastAssessment.Number,
                    AssessmentTypeName = assessmentType.Key.Name,
                };

                var isActiveAssessment = await _assessmentService.IsActiveAssessment(supervisorId, userId, lastAssessment.Id);

                lastAssessmentDto.IsActiveAssessment = isActiveAssessment;

                userLastAssessmentsOfEachType.Add(lastAssessmentDto);
            }

            return userLastAssessmentsOfEachType;
        }

        public async Task<IBaseResponse<List<AssessmentResultDto>>> GetColleaguesAssessmentResultsForAssessment(int userId)
        {
            try
            {
                var dtos = new List<AssessmentResultDto>();
                var colleaguesAssessmentResultsForAssessment = await _unitOfWork.AssessmentResults
                    .GetAllAsync(x => x.JudgeId == userId && x.Assessment.UserId != userId && x.SystemStatus == SystemStatuses.PENDING && x.AssignedBy.HasValue);

                foreach (var assessmentResult in colleaguesAssessmentResultsForAssessment)
                {
                    var assessment = await _unitOfWork.Assessments.GetAsync(x => x.Id == assessmentResult.AssessmentId, includeProperties: new string[]
                    {
                        "User",
                        "AssessmentResults.AssessmentResultValues",
                        "AssessmentType.AssessmentMatrix.Elements"
                    });

                    var assessmentResultDto = new AssessmentResultDto
                    {
                        Id = assessmentResult.Id,
                        AssessmentId = assessmentResult.AssessmentId,
                        SystemStatus = assessmentResult.SystemStatus,
                        Type = assessmentResult.Type,
                        Sum = assessmentResult.AssessmentResultValues.Sum(x => x.Value),
                        TypeName = assessment.AssessmentType.Name,
                        MinValue = assessment.AssessmentType.AssessmentMatrix.MinAssessmentMatrixResultValue,
                        MaxValue = assessment.AssessmentType.AssessmentMatrix.MaxAssessmentMatrixResultValue,
                    };

                    assessmentResultDto.Judge = new UserDto
                    {
                        Id = userId,
                    };

                    assessmentResultDto.Judged = new UserDto
                    {
                        Id = assessment.UserId,
                        ImagePath = assessmentResult.Assessment.User.ImagePath,
                        FullName = assessmentResult.Assessment.User.FullName,
                    };

                    foreach (var value in assessmentResult.AssessmentResultValues)
                    {
                        assessmentResultDto.Values.Add(new AssessmentResultValueDto
                        {
                            Value = value.Value,
                            AssessmentMatrixRow = value.AssessmentMatrixRow,
                        });
                    }

                    foreach (var element in assessment.AssessmentType.AssessmentMatrix.Elements)
                    {
                        assessmentResultDto.Elements.Add(new AssessmentMatrixElementDto
                        {
                            Column = element.Column,
                            Row = element.Row,
                            Value = element.Value,
                            HtmlClassName = element.HtmlClassName
                        });
                    }

                    assessmentResultDto.ElementsByRow = assessmentResultDto.Elements.OrderBy(x => x.Column).GroupBy(x => x.Row).OrderBy(x => x.Key).ToList();

                    dtos.Add(assessmentResultDto);
                }

                return new BaseResponse<List<AssessmentResultDto>>()
                {
                    Data = dtos,
                    StatusCode = StatusCodes.OK
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<List<AssessmentResultDto>>()
                {
                    Description = $"[UserService.GetColleaguesAssessmentResultsForAssessment] : {ex.Message}",
                    StatusCode = StatusCodes.InternalServerError,
                };
            }
        }

        public async Task AssessUser(AssessUserDto assessUserDto)
        {
            var assessmentResult = await _unitOfWork.AssessmentResults.GetAsync(x => x.Id == assessUserDto.AssessmentResultId,
                includeProperties: [
                    "Judge",
                    "Assessment.AssessmentResults",
                    "Assessment.AssessmentType",
            ]);

            if (assessmentResult == null)
            {
                throw new Exception($"AssessmentResult with ID {assessUserDto.AssessmentResultId} not found.");
            }

            assessmentResult.ResultDate = DateOnly.FromDateTime(DateTime.Today);
            assessmentResult.SystemStatus = SystemStatuses.COMPLETED;

            for (int i = 0; i < assessUserDto.ResultValues.Count; i++)
            {
                assessmentResult.AssessmentResultValues.Add(new AssessmentResultValue
                {
                    Value = Convert.ToInt32(assessUserDto.ResultValues[i]),
                    AssessmentMatrixRow = (i + 1),
                });
            }

            _unitOfWork.AssessmentResults.Update(assessmentResult);

            if (assessmentResult.Type == AssessmentResultTypes.UrpAssessment)
            {
                var otherUrpAssessmentResults = assessmentResult.Assessment.AssessmentResults.Where(x =>
                    x.AssessmentId == assessmentResult.AssessmentId &&
                    x.Id != assessmentResult.Id &&
                    x.Type == AssessmentResultTypes.UrpAssessment)
                    .ToList();

                foreach (var urpAssessmentResult in otherUrpAssessmentResults)
                {
                    _unitOfWork.AssessmentResults.Remove(urpAssessmentResult);
                }
            }

            var assessmentType = assessmentResult.Assessment.AssessmentType.SystemAssessmentType;
            var completedAssessmentResults = assessmentResult.Assessment.AssessmentResults.Where(x => x.SystemStatus == SystemStatuses.COMPLETED).ToList();

            var IsFinalized = _assessmentService.IsAssessmentFinalized(assessmentType, completedAssessmentResults);

            if (IsFinalized)
            {
                var grade = await _unitOfWork.Grades.GetAsync(x => x.Id == assessmentResult.Assessment.GradeId);

                if (grade == null)
                {
                    throw new Exception($"Grade with ID {assessmentResult.Assessment.GradeId} not found.");
                }
                else if (assessmentType == SystemAssessmentTypes.ManagementCompetencies)
                {
                    grade.IsManagmentCompetenciesFinalized = true;
                }
                else if (assessmentType == SystemAssessmentTypes.СorporateСompetencies)
                {
                    grade.IsCorporateCompetenciesFinalized = true;
                }

                _unitOfWork.Grades.Update(grade);
            }

            _unitOfWork.AssessmentResults.Update(assessmentResult);
            await _unitOfWork.CommitAsync();
        }

        public bool CanChooseJudges(IEnumerable<string> userRoles, AssessmentDto assessmentDto)
        {
            var isUserInRole = userRoles.Any(role => role == "Urp" || role == "Supervisor");
            var isAssessmentTypeCorporate = assessmentDto.SystemAssessmentType == SystemAssessmentTypes.СorporateСompetencies;
            var isStatusPending = assessmentDto.SystemStatus == SystemStatuses.PENDING;
            var completedJudgesCount = assessmentDto.CompletedAssessmentResults.Count(x => x.AssignedBy.HasValue);

            return isUserInRole && isAssessmentTypeCorporate && isStatusPending && completedJudgesCount < 3;
        }

        public async Task ApproveGrade(int gradeId)
        {
            var grade = await _unitOfWork.Grades.GetAsync(x => x.Id == gradeId, includeProperties: "Assessments.AssessmentResults");

            if (grade == null)
            {
                throw new Exception($"Grade with ID {gradeId} not found.");
            }

            grade.GradeStatus = GradeStatuses.APPROVED_BY_EMPLOYEE;

            _unitOfWork.Grades.Update(grade);
            await _unitOfWork.CommitAsync();
        }
    }
}