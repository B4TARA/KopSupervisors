using KOP.Common.Dtos;
using KOP.Common.Dtos.GradeDtos;
using KOP.Common.Interfaces;

namespace KOP.BLL.Interfaces
{
    public interface IReportService
    {
        Task<IBaseResponse<List<UserSummaryDto>>> GetSubordinateUsersWithGrade(int supervisorId);
        Task<IBaseResponse<List<GradeSummaryDto>>> GetEmployeeGrades(int employeeId);
        Task<IBaseResponse<byte[]>> GenerateGradeWordDocument(int gradeId);
    }
}