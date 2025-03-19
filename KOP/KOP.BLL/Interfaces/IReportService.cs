namespace KOP.BLL.Interfaces
{
    public interface IReportService
    {
        Task<byte[]> GenerateGradeWordDocument(int gradeId);
    }
}