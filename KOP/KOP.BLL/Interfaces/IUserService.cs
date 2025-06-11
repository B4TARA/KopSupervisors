using KOP.Common.Dtos.AssessmentDtos;
using KOP.Common.Dtos.AssessmentResultDtos;
using KOP.Common.Dtos.GradeDtos;
using KOP.Common.Dtos.UserDtos;
using KOP.DAL.Entities;

namespace KOP.BLL.Interfaces
{
    public interface IUserService
    {
        Task<UserDto> GetUser(int userId);
        Task<List<UserDto>> GetAllUsers();
        Task<List<UserDto>> GetUsersWithAnyPendingGrade();


        Task<List<GradeDto>> GetGradesForUser(int userId);

        Task<List<AssessmentDto>> GetLastGradeAssessmentsForUser(int userId);

        Task<List<AssessmentResultDto>> GetColleaguesAssessmentResultsForAssessment(int userId);


        bool CanChooseJudges(List<string> userRoles, AssessmentDto assessmentDto);


        Task<User?> GetFirstSupervisorForUser(int userId);


        Task AssessUser(AssessUserDto assessUserDto);
        Task ApproveGrade(int gradeId);
    }
}