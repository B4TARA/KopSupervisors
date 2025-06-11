using KOP.Common.Dtos;
using KOP.Common.Dtos.AssessmentDtos;
using KOP.Common.Dtos.AssessmentResultDtos;

namespace KOP.WEB.Models.ViewModels.Supervisor
{
    public class EmployeeAssessmentViewModel
    {
        public AssessmentDto Assessment { get; set; } = new();
        public AssessmentResultDto? SupervisorAssessmentResult { get; set; }
        public List<CandidateForJudgeDto> CandidatesForJudges { get; set; }
        public List<CandidateForJudgeDto> ChoosedCandidatesForJudges { get; set; }

        public bool ChooseJudgesAccess {  get; set; }
    }
}