namespace KOP.BLL.Interfaces
{
    public interface IReportService
    {
        Task<byte[]> GenerateGradesReport(int gradeId);
        Task<byte[]> GenerateUpcomingGradesReport(int supervisorId);
    }
}