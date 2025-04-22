using KOP.BLL.Interfaces;
using KOP.Common.Dtos.AssessmentDtos;
using KOP.Common.Enums;
using KOP.DAL.Entities;
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

        public async Task<AssessmentDto?> GetAssessment(int id)
        {
            var assessment = await _unitOfWork.Assessments.GetAsync(x => x.Id == id,
                includeProperties: [
                    "AssessmentType.AssessmentInterpretations",
                    "AssessmentType.AssessmentMatrix.Elements",
                    "AssessmentResults.AssessmentResultValues",
                    "AssessmentResults.Judge",
                    "User" ]);

            if (assessment == null)
            {
                return null;
            }

            var assessmentDto = _mappingService.CreateAssessmentDto(assessment);
            return assessmentDto;
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

            var selfAssessmentResult = assessment.AssessmentResults.FirstOrDefault(x => x.Type == AssessmentResultTypes.SelfAssessment);
            if (selfAssessmentResult != null)
            {
                assessmentSummaryDto.SelfAssessmentResultId = selfAssessmentResult.Id;
                assessmentSummaryDto.SelfAssessmentResultSystemStatus = selfAssessmentResult.SystemStatus;
                assessmentSummaryDto.SelfAssessmentResultValues = GetAssessmentResultValues(selfAssessmentResult).ToList();
            }
            else
            {
                assessmentSummaryDto.SelfAssessmentResultSystemStatus = SystemStatuses.NOT_EXIST;
            }

            var supervisorAssessmentResult = assessment.AssessmentResults.FirstOrDefault(x => x.Type == AssessmentResultTypes.SupervisorAssessment);
            if (supervisorAssessmentResult != null)
            {
                assessmentSummaryDto.SupervisorAssessmentResultSystemStatus = supervisorAssessmentResult.SystemStatus;
                assessmentSummaryDto.SupervisorAssessmentResultValues = GetAssessmentResultValues(supervisorAssessmentResult).ToList();
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

        public async Task<int> GetMatrixColumnForAssessmentValue(int value)
        {
            var assessmentRange = await _unitOfWork.AssessmentRanges.GetAsync(x => x.MinRangeValue <= value && value <= x.MaxRangeValue);

            return assessmentRange.ColumnNumber;
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
            }).ToList();
        }
    }
}