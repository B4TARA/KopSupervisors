using KOP.Common.Dtos.GradeDtos;

namespace KOP.BLL.Interfaces
{
    public interface IQualificationService
    {
        Task<QualificationDto> GetQualificationForGrade(int gradeId);
        Task EditQualification(QualificationDto qualificationDto);
        Task DeletePreviousJob(int id);
        Task DeleteHigherEducation(int id);
    }
}