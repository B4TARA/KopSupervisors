using KOP.BLL.Interfaces;
using KOP.Common.Dtos.GradeDtos;
using KOP.DAL;
using KOP.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace KOP.BLL.Services
{
    public class QualificationService : IQualificationService
    {
        private readonly ApplicationDbContext _context;

        public QualificationService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<QualificationDto> GetQualificationForGrade(int gradeId)
        {
            var qualification = await _context.Qualifications
                .AsNoTracking()
                .Select(q => new QualificationDto
                {
                    Id = q.Id,
                    GradeId = q.GradeId,
                    CurrentStatusDate = q.CurrentStatusDate,
                    CurrentExperienceYears = q.CurrentExperienceYears,
                    CurrentExperienceMonths = q.CurrentExperienceMonths,
                    CurrentJobStartDate = q.CurrentJobStartDate,
                    CurrentJobPositionName = q.CurrentJobPositionName,
                    EmploymentContarctTerminations = q.EmploymentContarctTerminations,
                    QualificationResult = q.QualificationResult,
                    IsFinalized = q.Grade.IsQualificationFinalized,
                    Conclusion = q.Grade.QualificationConclusion,

                    PreviousJobs = q.PreviousJobs
                       .Select(pj => new PreviousJobDto
                       {
                           Id = pj.Id,
                           StartDate = pj.StartDate,
                           EndDate = pj.EndDate,
                           OrganizationName = pj.OrganizationName,
                           PositionName = pj.OrganizationName
                       })
                       .OrderBy(pj => pj.StartDate)
                       .ToList(),

                    HigherEducations = q.HigherEducations
                        .Select(he => new HigherEducationDto
                        {
                            Id = he.Id,
                            Education = he.Education,
                            Speciality = he.Speciality,
                            QualificationName = he.QualificationName,
                            StartDate = he.StartDate,
                            EndDate = he.EndDate
                        })
                        .OrderBy(he => he.StartDate)
                        .ToList(),
                })
                .FirstOrDefaultAsync(q => q.GradeId == gradeId);

            if (qualification == null)
                throw new KeyNotFoundException($"Qualification with Grade ID {gradeId} not found.");

            return qualification;
        }

        public async Task EditQualification(QualificationDto qualificationDto)
        {
            var qualification = await _context.Qualifications
                .Include(q => q.Grade)
                .Include(q => q.PreviousJobs)
                .Include(q => q.HigherEducations)
                .FirstOrDefaultAsync(q => q.Id == qualificationDto.Id);

            if (qualification == null)
                throw new KeyNotFoundException($"Qualification with ID {qualificationDto.Id} not found.");

            qualification.CurrentStatusDate = qualificationDto.CurrentStatusDate;
            qualification.CurrentExperienceYears = qualificationDto.CurrentExperienceYears;
            qualification.CurrentExperienceMonths = qualificationDto.CurrentExperienceMonths;
            qualification.CurrentJobStartDate = qualificationDto.CurrentJobStartDate;
            qualification.CurrentJobPositionName = qualificationDto.CurrentJobPositionName ?? string.Empty;
            qualification.EmploymentContarctTerminations = qualificationDto.EmploymentContarctTerminations ?? string.Empty;
            qualification.QualificationResult = qualificationDto.QualificationResult ?? string.Empty;
            qualification.Grade.IsQualificationFinalized = qualificationDto.IsFinalized;
            qualification.Grade.QualificationConclusion = qualificationDto.Conclusion ?? string.Empty;

            qualification.PreviousJobs = qualificationDto.PreviousJobs
               .Select(pj => new PreviousJob
               {
                   StartDate = pj.StartDate,
                   EndDate = pj.EndDate,
                   OrganizationName = pj.OrganizationName ?? string.Empty,
                   PositionName = pj.OrganizationName ?? string.Empty
               })
               .ToList();

            qualification.HigherEducations = qualificationDto.HigherEducations
                .Select(he => new HigherEducation
                {
                    Education = he.Education ?? string.Empty,
                    Speciality = he.Speciality ?? string.Empty,
                    QualificationName = he.QualificationName ?? string.Empty,
                    StartDate = he.StartDate,
                    EndDate = he.EndDate
                })
                .ToList();

            await _context.SaveChangesAsync();
        }

        public async Task DeletePreviousJob(int id)
        {
            var previousJobToDelete = await _context.PreviousJobs
                .FirstOrDefaultAsync(x => x.Id == id);

            if (previousJobToDelete == null)
                throw new Exception($"PreviousJob with ID {id} not found.");

            _context.PreviousJobs.Remove(previousJobToDelete);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteHigherEducation(int id)
        {
            var higherEducationToDelete = await _context.HigherEducations
                .FirstOrDefaultAsync(x => x.Id == id);

            if (higherEducationToDelete == null)
                throw new Exception($"HigherEducation with ID {id} not found.");

            _context.HigherEducations.Remove(higherEducationToDelete);
            await _context.SaveChangesAsync();
        }
    }
}