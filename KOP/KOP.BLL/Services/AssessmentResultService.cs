using KOP.BLL.Interfaces;
using KOP.Common.Dtos.AssessmentDtos;
using KOP.Common.Dtos.AssessmentResultDtos;
using KOP.Common.Dtos.UserDtos;
using KOP.Common.Enums;
using KOP.DAL;
using KOP.DAL.Entities;
using KOP.EmailService;
using Microsoft.EntityFrameworkCore;

namespace KOP.BLL.Services
{
    public class AssessmentResultService : IAssessmentResultService
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailSender _emailSender;

        public AssessmentResultService(ApplicationDbContext context, IEmailSender emailSender)
        {
            _context = context;
            _emailSender = emailSender;
        }

        public async Task<AssessmentResultDto?> GetAssessmentResult(int judgeId, int assessmentId)
        {
            var assessmentResultDto = await _context.AssessmentResults
                .Where(x => x.AssessmentId == assessmentId && x.JudgeId == judgeId)
                .Select(ar => new AssessmentResultDto
                {
                    Id = ar.Id,
                    AssessmentTypeId = ar.Assessment.AssessmentTypeId,
                    SystemStatus = ar.SystemStatus,
                    Type = ar.Type,
                    Sum = ar.AssessmentResultValues.Sum(value => value.Value),
                    AssignedBy = ar.AssignedBy,
                    Judge = new UserExtendedDto
                    {
                        Id = ar.Judge.Id,
                        FullName = ar.Judge.FullName,
                        SystemRoles = ar.Judge.SystemRoles,
                        ImagePath = ar.Judge.ImagePath,
                    },
                    Judged = new UserExtendedDto
                    {
                        Id = ar.Assessment.User.Id,
                        FullName = ar.Assessment.User.FullName,
                    },
                    Values = ar.AssessmentResultValues.Select(value => new AssessmentResultValueDto
                    {
                        Value = value.Value,
                        AssessmentMatrixRow = value.AssessmentMatrixRow,
                    }).ToList(),
                    Elements = ar.Assessment.AssessmentType.AssessmentMatrix.Elements.Select(element => new AssessmentMatrixElementDto
                    {
                        Column = element.Column,
                        Row = element.Row,
                        Value = element.Value,
                        HtmlClassName = element.HtmlClassName,
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            if (assessmentResultDto == null)
            {
                return null;
            }

            // Дополнительные вычисления
            var assessmentType = await _context.AssessmentTypes
                .Include(at => at.AssessmentInterpretations)
                .FirstOrDefaultAsync(at => at.Id == assessmentResultDto.AssessmentTypeId);

            if (assessmentType != null)
            {
                var assessmentInterpretation = assessmentType.AssessmentInterpretations
                    .FirstOrDefault(x => x.MinValue <= assessmentResultDto.Sum && x.MaxValue >= assessmentResultDto.Sum);

                if (assessmentInterpretation != null)
                {
                    assessmentResultDto.HtmlClassName = assessmentInterpretation.HtmlClassName;
                }

                assessmentResultDto.MaxValue = assessmentType.AssessmentMatrix.MaxAssessmentMatrixResultValue;
                assessmentResultDto.MinValue = assessmentType.AssessmentMatrix.MinAssessmentMatrixResultValue;
            }

            // Группировка элементов по строкам
            assessmentResultDto.ElementsByRow = assessmentResultDto.Elements
                .OrderBy(x => x.Column)
                .GroupBy(x => x.Row)
                .OrderBy(x => x.Key)
                .ToList();

            return assessmentResultDto;
        }
        public async Task<AssessmentResultDto?> GetManagementSelfAssessmentResultForGrade(int gradeId)
        {
            var assessmentResult = await _context.AssessmentResults
                   .Where(ar => ar.Assessment.GradeId == gradeId
                       && ar.Assessment.AssessmentType.SystemAssessmentType == SystemAssessmentTypes.ManagementCompetencies
                       && ar.Type == AssessmentResultTypes.SelfAssessment)
                   .Select(ar => new AssessmentResultDto
                   {
                       Id = ar.Id,
                       Sum = ar.AssessmentResultValues.Sum(value => value.Value),
                   })
                   .FirstOrDefaultAsync();

            return assessmentResult;
        }
        public async Task<AssessmentResultDto?> GetManagementSupervisorAssessmentResultForGrade(int gradeId)
        {
            var assessmentResult = await _context.AssessmentResults
               .Where(ar => ar.Assessment.GradeId == gradeId
                   && ar.Assessment.AssessmentType.SystemAssessmentType == SystemAssessmentTypes.ManagementCompetencies
                   && ar.Type == AssessmentResultTypes.SupervisorAssessment)
               .Select(ar => new AssessmentResultDto
               {
                   Id = ar.Id,
                   Sum = ar.AssessmentResultValues.Sum(value => value.Value),
               })
               .FirstOrDefaultAsync();

            return assessmentResult;
        }

        public async Task CreatePendingColleagueAssessmentResult(int judgeId, int assessmentId, int assignerId)
        {
            var judge = await _context.Users.FirstOrDefaultAsync(x => x.Id == judgeId);

            if (judge == null)
            {
                throw new Exception($"Judge with ID {judgeId} not found.");
            }

            var assessment = await _context.Assessments.FirstOrDefaultAsync(x => x.Id == assessmentId);

            if (assessment == null)
            {
                throw new Exception($"Assessment with ID {assessmentId} not found.");
            }

            var newAssessmentResult = new AssessmentResult
            {
                SystemStatus = SystemStatuses.PENDING,
                JudgeId = judgeId,
                AssessmentId = assessmentId,
                Type = AssessmentResultTypes.ColleagueAssessment,
                AssignedBy = assignerId,
            };

            await CreateAssessmentResult(newAssessmentResult);

            var mailTitle = "<div>\r\n    Вы выбраны в качестве оценщика корпоративных компетенций. \r\n<div>Проведите оценку корпоративных компетенций в личном кабинете в ближайшие 3 дня.</div></div>";
            var mailBody = "КОП. Оцените корпоративные компетенции";
            var message = new Message([judge.Email], mailBody, mailTitle, judge.FullName);
            await _emailSender.SendEmailAsync(message);
        }
        public async Task DeletePendingAssessmentResult(int id)
        {
            var assessmentResult = await _context.AssessmentResults.FirstOrDefaultAsync(x => x.Id == id);

            if (assessmentResult == null)
            {
                throw new Exception($"Assessment with ID {id} not found.");
            }
            else if (assessmentResult.SystemStatus != SystemStatuses.PENDING)
            {
                throw new Exception($"AssessmentResult with ID {id} has completed status already.");
            }

            _context.AssessmentResults.Remove(assessmentResult);
            await _context.SaveChangesAsync();
        }
        public async Task CreateAssessmentResult(AssessmentResult assessmentResult)
        {
            if (assessmentResult == null)
                throw new ArgumentNullException(nameof(assessmentResult), "Assessment result cannot be null.");

            var dbAssessmentResult = await _context.AssessmentResults
                .FirstOrDefaultAsync(x => x.AssessmentId == assessmentResult.AssessmentId && x.JudgeId == assessmentResult.JudgeId && x.Type == assessmentResult.Type);

            if (dbAssessmentResult != null)
                throw new InvalidOperationException($"AssessmentResult with JudgeId {assessmentResult.JudgeId} and AssessmentId {assessmentResult.AssessmentId} already exists.");

            await _context.AssessmentResults.AddAsync(assessmentResult);
            await _context.SaveChangesAsync();
        }
    }
}