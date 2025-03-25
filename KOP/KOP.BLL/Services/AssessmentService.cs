using KOP.BLL.Interfaces;
using KOP.Common.Dtos.AssessmentDtos;
using KOP.Common.Enums;
using KOP.DAL.Entities.AssessmentEntities;
using KOP.DAL.Interfaces;
using KOP.EmailService;
using Microsoft.Extensions.Configuration;

namespace KOP.BLL.Services
{
    public class AssessmentService : IAssessmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMappingService _mappingService;
        private readonly IEmailSender _emailSender;
        private readonly IConfiguration _configuration;
        private readonly ICommonService _commonService;

        public AssessmentService(IUnitOfWork unitOfWork, IMappingService mappingService, IEmailSender emailSender,
            IConfiguration configuration, ICommonService commonService)
        {
            _unitOfWork = unitOfWork;
            _mappingService = mappingService;
            _emailSender = emailSender;
            _configuration = configuration;
            _commonService = commonService;
        }

        public async Task<AssessmentDto> GetAssessment(int id)
        {
            var assessment = await _unitOfWork.Assessments.GetAsync(x => x.Id == id,
                includeProperties: new string[]{
                    "AssessmentType.AssessmentInterpretations",
                    "AssessmentType.AssessmentMatrix.Elements",
                    "AssessmentResults.AssessmentResultValues",
                    "AssessmentResults.Judge",
                    "User",
                });

            if (assessment == null)
            {
                throw new Exception($"Assessment with ID {id} not found.");
            }

            var assessmentDto = _mappingService.CreateAssessmentDto(assessment);
            return assessmentDto;
        }

        public async Task<AssessmentResultDto?> GetAssessmentResult(int judgeId, int assessmentId)
        {
            var assessmentResult = await _unitOfWork.AssessmentResults.GetAsync(
                x => x.AssessmentId == assessmentId && x.JudgeId == judgeId,
                includeProperties: new string[]
                {
                    "Judge",
                    "AssessmentResultValues",
                    "Assessment.AssessmentType.AssessmentMatrix.Elements",
                    "Assessment.User",
                }
            );

            if (assessmentResult == null)
            {
                return null;
            }

            var assessmentResultDto = _mappingService.CreateAssessmentResultDto(
                assessmentResult,
                assessmentResult.Assessment.AssessmentType
            );

            return assessmentResultDto;
        }

        public async Task<AssessmentSummaryDto> GetAssessmentSummary(int assessmentId)
        {
            var assessment = await _unitOfWork.Assessments.GetAsync(
                x => x.Id == assessmentId,
                includeProperties: [
                    "AssessmentResults.AssessmentResultValues",
                    "AssessmentType.AssessmentMatrix.Elements",
                    "AssessmentType.AssessmentInterpretations",
                    "AssessmentResults.Judge",
                ]
            );

            if (assessment == null)
            {
                throw new Exception($"Assessment with ID {assessmentId} not found.");
            }

            var assessmentSummaryDto = CreateAssessmentSummaryDto(assessment);

            var selfAssessmentResult = assessment.AssessmentResults.FirstOrDefault(x => x.Type == AssessmentResultTypes.SupervisorAssessment);
            if (selfAssessmentResult != null)
            {
                assessmentSummaryDto.SelfAssessmentResultSystemStatus = selfAssessmentResult.SystemStatus;
            }
            else
            {
                assessmentSummaryDto.SelfAssessmentResultSystemStatus = SystemStatuses.NOT_EXIST;
            }

            var supervisorAssessmentResult = assessment.AssessmentResults.FirstOrDefault(x => x.Type == AssessmentResultTypes.SupervisorAssessment);
            if (supervisorAssessmentResult != null)
            {
                assessmentSummaryDto.SupervisorAssessmentResultSystemStatus = supervisorAssessmentResult.SystemStatus;
            }
            else
            {
                assessmentSummaryDto.SupervisorAssessmentResultSystemStatus = SystemStatuses.NOT_EXIST;
            }

            var colleaguesAssessmentResults = assessment.AssessmentResults.Where(x => x.Type == AssessmentResultTypes.ColleagueAssessment).ToList();

            assessmentSummaryDto.AverageSelfValue = CalculateAverage(assessmentSummaryDto.SelfAssessmentResultValues);
            assessmentSummaryDto.AverageSupervisorValue = CalculateAverage(assessmentSummaryDto.SupervisorAssessmentResultValues);

            var assessmentType = assessment.AssessmentType.SystemAssessmentType;
            var completedAssessmentResults = assessment.AssessmentResults.Where(x => x.SystemStatus == SystemStatuses.COMPLETED).ToList();

            assessmentSummaryDto.IsFinalized = IsAssessmentFinalized(assessmentType, completedAssessmentResults);

            ProcessColleaguesResults(colleaguesAssessmentResults, assessmentSummaryDto);
            ProcessCompletedResults(completedAssessmentResults, assessmentSummaryDto);
            ProcessInterpretations(assessment, assessmentSummaryDto);

            return assessmentSummaryDto;
        }

        private AssessmentSummaryDto CreateAssessmentSummaryDto(Assessment assessment)
        {
            return new AssessmentSummaryDto
            {
                AssessmentId = assessment.Id,
                UserId = assessment.UserId,
                SystemAssessmentType = assessment.AssessmentType.SystemAssessmentType,
                MinValue = assessment.AssessmentType.AssessmentMatrix.MinAssessmentMatrixResultValue,
                MaxValue = assessment.AssessmentType.AssessmentMatrix.MaxAssessmentMatrixResultValue,
                RowsWithElements = assessment.AssessmentType.AssessmentMatrix.Elements
                    .Select(element => new AssessmentMatrixElementDto
                    {
                        Column = element.Column,
                        Row = element.Row,
                        Value = element.Value,
                        HtmlClassName = element.HtmlClassName,
                    })
                    .GroupBy(x => x.Row)
                    .OrderBy(x => x.Key)
                    .ToList()
            };
        }

        private IEnumerable<AssessmentResultValueDto> GetAssessmentResultValues(AssessmentResult result)
        {
            if (result == null || result.AssessmentResultValues == null)
            {
                return Enumerable.Empty<AssessmentResultValueDto>();
            }

            return result.AssessmentResultValues.Select(value => new AssessmentResultValueDto
            {
                Value = value.Value,
                AssessmentMatrixRow = value.AssessmentMatrixRow,
            });
        }

        private double CalculateAverage(List<AssessmentResultValueDto> values)
        {
            return values.Count > 0 ? values.Average(x => x.Value) : 0;
        }

        public bool IsAssessmentFinalized(SystemAssessmentTypes? assessmentType, List<AssessmentResult> completedResults)
        {
            if (assessmentType == SystemAssessmentTypes.СorporateСompetencies)
            {
                var selfAssessment = completedResults.FirstOrDefault(x => x.Type == AssessmentResultTypes.SelfAssessment);
                var supervisorAssessment = completedResults.FirstOrDefault(x => x.Type == AssessmentResultTypes.SupervisorAssessment);
                var urpAssessment = completedResults.FirstOrDefault(x => x.Type == AssessmentResultTypes.UrpAssessment);
                var colleaguesAssessments = completedResults.Where(x => x.Type == AssessmentResultTypes.ColleagueAssessment).ToList();

                return selfAssessment != null && supervisorAssessment != null && urpAssessment != null && colleaguesAssessments.Count >= 3;
            }
            else if (assessmentType == SystemAssessmentTypes.ManagementCompetencies)
            {
                var selfAssessment = completedResults.FirstOrDefault(x => x.Type == AssessmentResultTypes.SelfAssessment);
                var supervisorAssessment = completedResults.FirstOrDefault(x => x.Type == AssessmentResultTypes.SupervisorAssessment);

                return selfAssessment != null && supervisorAssessment != null;
            }

            return false;
        }

        private void ProcessColleaguesResults(List<AssessmentResult> completedColleaguesResults, AssessmentSummaryDto assessmentSummaryDto)
        {
            if (completedColleaguesResults.Any())
            {
                foreach (var result in completedColleaguesResults)
                {
                    foreach (var value in result.AssessmentResultValues)
                    {
                        var avgResult = assessmentSummaryDto.ColleaguesAssessmentResultValues
                            .FirstOrDefault(x => x.AssessmentMatrixRow == value.AssessmentMatrixRow);

                        if (avgResult != null)
                        {
                            avgResult.Value += value.Value;
                            assessmentSummaryDto.ColleaguesSumResult += value.Value;
                        }
                        else
                        {
                            assessmentSummaryDto.ColleaguesAssessmentResultValues.Add(new AssessmentResultValueDto
                            {
                                Value = value.Value,
                                AssessmentMatrixRow = value.AssessmentMatrixRow
                            });
                            assessmentSummaryDto.ColleaguesSumResult += value.Value;
                        }
                    }
                }

                // Вычисление среднего значения для коллег
                foreach (var value in assessmentSummaryDto.ColleaguesAssessmentResultValues)
                {
                    var average = Math.Round(value.Value / completedColleaguesResults.Count, 1);
                    value.Value = average;
                    assessmentSummaryDto.AverageColleaguesResult += average;
                }
            }
        }

        private void ProcessCompletedResults(List<AssessmentResult> completedResults, AssessmentSummaryDto assessmentSummaryDto)
        {
            foreach (var result in completedResults)
            {
                foreach (var value in result.AssessmentResultValues)
                {
                    var avgResult = assessmentSummaryDto.AverageValuesByRow
                        .FirstOrDefault(x => x.AssessmentMatrixRow == value.AssessmentMatrixRow);

                    if (avgResult != null)
                    {
                        avgResult.Value += value.Value;
                        assessmentSummaryDto.SumResult += value.Value;
                    }
                    else
                    {
                        assessmentSummaryDto.AverageValuesByRow.Add(new AssessmentResultValueDto
                        {
                            Value = value.Value,
                            AssessmentMatrixRow = value.AssessmentMatrixRow
                        });
                        assessmentSummaryDto.SumResult += value.Value;
                    }
                }
            }

            // Вычисление среднего значения для завершенных оценок
            foreach (var value in assessmentSummaryDto.AverageValuesByRow)
            {
                var average = Math.Round(value.Value / completedResults.Count, 1);
                value.Value = average;
                assessmentSummaryDto.GeneralAverageResult += average;
            }
        }

        private void ProcessInterpretations(Assessment assessment, AssessmentSummaryDto assessmentSummaryDto)
        {
            foreach (var interpretation in assessment.AssessmentType.AssessmentInterpretations)
            {
                var interpretationDto = _mappingService.CreateAssessmentInterpretationDto(interpretation);

                if (assessmentSummaryDto.GeneralAverageResult >= interpretationDto.MinValue && assessmentSummaryDto.GeneralAverageResult <= interpretationDto.MaxValue)
                {
                    assessmentSummaryDto.AverageAssessmentInterpretation = interpretationDto;
                }

                assessmentSummaryDto.AssessmentTypeInterpretations.Add(interpretationDto);
            }
        }

        public async Task<bool> IsActiveAssessment(int judgeId, int judgedId, int? assessmentId)
        {
            var pendingAssessmentResults = await _unitOfWork.AssessmentResults.GetAllAsync(x =>
                x.JudgeId == judgeId &&
                x.Assessment.UserId == judgedId &&
                x.SystemStatus == SystemStatuses.PENDING);

            if (assessmentId.HasValue)
            {
                pendingAssessmentResults = pendingAssessmentResults.Where(x => x.AssessmentId == assessmentId);
            }

            return pendingAssessmentResults.Any();
        }

        public async Task DeleteJudgeForAssessment(int assessmentResultId)
        {
            var assessmentResultToDelete = await _unitOfWork.AssessmentResults.GetAsync(x => x.Id == assessmentResultId);

            if (assessmentResultToDelete == null)
            {
                throw new Exception($"AssessmentResult with ID {assessmentResultId} not found.");
            }

            if (assessmentResultToDelete.SystemStatus == SystemStatuses.COMPLETED)
            {
                throw new Exception($"AssessmentResult with ID {assessmentResultId} has completed status already.");
            }

            _unitOfWork.AssessmentResults.Remove(assessmentResultToDelete);
            await _unitOfWork.CommitAsync();
        }

        public async Task AddJudgeForAssessment(int judgeId, int assessmentId, int assignerId)
        {
            var assessment = await _unitOfWork.Assessments.GetAsync(x => x.Id == assessmentId);
            if (assessment == null)
            {
                throw new Exception($"Assessment with ID {assessmentId} not found.");
            }

            var judge = await _unitOfWork.Users.GetAsync(x => x.Id == judgeId);
            if (judge == null)
            {
                throw new Exception($"Judge with ID {judgeId} not found.");
            }

            var existingAssessmentResult = await _unitOfWork.AssessmentResults.GetAsync(
                x => x.AssessmentId == assessmentId && x.JudgeId == judgeId && x.AssignedBy != null
            );

            if (existingAssessmentResult != null)
            {
                throw new InvalidOperationException($"AssessmentResult with JudgeId {judgeId} and AssessmentId {assessmentId} already exists.");
            }

            var assessmentResultToAdd = new AssessmentResult
            {
                SystemStatus = SystemStatuses.PENDING,
                JudgeId = judgeId,
                AssessmentId = assessmentId,
                Type = AssessmentResultTypes.ColleagueAssessment,
                AssignedBy = assignerId,
            };

            await _unitOfWork.AssessmentResults.AddAsync(assessmentResultToAdd);
            await _unitOfWork.CommitAsync();

            var mail = await _unitOfWork.Mails.GetAsync(x => x.Code == MailCodes.CreateCorporateCompeteciesAssessmentNotification);

            if (mail == null)
            {
                var addressee = new string[] { "ebaturel@mtb.minsk.by" };
                var messageBody = "Не найдено сообщение для отправки оценщикам при назначении";
                var errorMessage = new Message(addressee, "Ошибка веб-приложения", messageBody, "Батурель Евгений Дмитриевич");
                await _emailSender.SendEmailAsync(errorMessage);
            }
            else
            {
                var message = new Message(new string[] { judge.Email }, mail.Title, mail.Body, judge.FullName);
                await _emailSender.SendEmailAsync(message);
            }
        }

        public async Task<int> GetMatrixColumnForAssessmentValue(int value)
        {
            var assessmentRange = await _unitOfWork.AssessmentRanges.GetAsync(x => x.MinRangeValue <= value && value <= x.MaxRangeValue);

            return assessmentRange.ColumnNumber;
        }
    }
}