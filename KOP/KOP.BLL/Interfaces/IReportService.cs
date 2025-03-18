using KOP.Common.Dtos;
using KOP.Common.Dtos.GradeDtos;

namespace KOP.BLL.Interfaces
{
    public interface IReportService
    {
        Task<IEnumerable<UserSummaryDto>> GetSubordinateUsersWithGrade(int supervisorId);
        Task<IEnumerable<GradeSummaryDto>> GetEmployeeGrades(int employeeId);
        Task<byte[]> GenerateGradeWordDocument(int gradeId);
    }
}