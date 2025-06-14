﻿using KOP.BLL.Interfaces;
using KOP.Common.Dtos.AssessmentDtos;
using KOP.Common.Dtos.AssessmentResultDtos;
using KOP.Common.Dtos.GradeDtos;
using KOP.Common.Dtos.UserDtos;
using KOP.Common.Enums;
using KOP.DAL;
using KOP.DAL.Entities;
using KOP.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace KOP.BLL.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAssessmentService _assessmentService;

        public UserService(ApplicationDbContext context, IUnitOfWork unitOfWork, IAssessmentService assessmentService)
        {
            _context = context;
            _unitOfWork = unitOfWork;
            _assessmentService = assessmentService;
        }

        public async Task<UserDto> GetUser(int userId)
        {
            var user = await _context.Users
               .AsNoTracking()
               .Where(u => u.Id == userId)
               .Select(u => new UserDto
               {
                   Id = u.Id,
                   FullName = u.FullName,
                   Position = u.Position,
                   SubdivisionFromFile = u.SubdivisionFromFile,
                   GradeGroup = u.GradeGroup,
                   WorkPeriod = u.GetWorkPeriod,
                   ContractEndDate = u.GetContractEndDate,
                   LastGrade = u.Grades
                       .Select(g => new GradeDto
                       {
                           Id = g.Id,
                           Period = $"{g.StartDate.ToString("dd.MM.yyyy")} - {g.EndDate.ToString("dd.MM.yyyy")}",
                           IsPending = g.GradeStatus == GradeStatuses.PENDING,
                           IsProjectsFinalized = g.IsProjectsFinalized,
                           IsStrategicTasksFinalized = g.IsStrategicTasksFinalized,
                           IsKpisFinalized = g.IsKpisFinalized,
                           IsMarksFinalized = g.IsMarksFinalized,
                           IsQualificationFinalized = g.IsQualificationFinalized,
                           IsValueJudgmentFinalized = g.IsValueJudgmentFinalized,
                           IsCorporateCompetenciesFinalized = g.IsCorporateCompetenciesFinalized,
                           IsManagmentCompetenciesFinalized = g.IsManagmentCompetenciesFinalized,
                           GradeStatus = g.GradeStatus,
                       })
                       .FirstOrDefault(),
               })
               .FirstOrDefaultAsync();

            if (user == null)
                throw new Exception($"User with ID {userId} not found.");

            return user;
        }

        public async Task<List<UserDto>> GetAllUsers()
        {
            var users = await _context.Users
                .AsNoTracking()
                .Where(u => !u.IsDismissed)
                .Select(user => new UserDto
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Position = user.Position,
                    SubdivisionFromFile = user.SubdivisionFromFile,
                })
                .OrderBy(x => x.FullName)
                .ToListAsync();

            return users;
        }

        public async Task<List<UserDto>> GetUsersWithAnyPendingGrade()
        {
            var users = await _context.Users
                .AsNoTracking()
                .Where(u => u.Grades.Any(g => g.SystemStatus == SystemStatuses.PENDING) && !u.IsDismissed)
                .Select(user => new UserDto
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Position = user.Position,
                    SubdivisionFromFile = user.SubdivisionFromFile,
                })
                .OrderBy(u => u.FullName)
                .ToListAsync();

            return users;
        }

        public async Task ApproveGrade(int gradeId)
        {
            var grade = await _context.Grades
                .FirstOrDefaultAsync(g => g.Id == gradeId);

            if (grade == null)
                throw new KeyNotFoundException($"Grade with ID {gradeId} not found.");

            grade.GradeStatus = GradeStatuses.APPROVED_BY_EMPLOYEE;
            await _context.SaveChangesAsync();
        }



        public async Task<List<AssessmentDto>> GetLastGradeAssessmentsForUser(int userId)
        {

            var user = await _unitOfWork.Users.GetAsync(x => x.Id == userId, includeProperties: ["Grades.Assessments.AssessmentType"]);

            if (user == null)
                throw new Exception($"User with ID {userId} not found.");

            var lastGrade = user.Grades.OrderByDescending(x => x.Number).FirstOrDefault();

            if (lastGrade == null)
                return new List<AssessmentDto>();

            var assessmentDtoList = lastGrade.Assessments
                .Select(assessment => new AssessmentDto
                {
                    Id = assessment.Id,
                    AssessmentTypeName = assessment.AssessmentType.Name
                })
                .ToList();

            return assessmentDtoList;
        }

        public async Task<List<GradeDto>> GetGradesForUser(int userId)
        {
            var grades = await _context.Grades
                .AsNoTracking()
                .Where(g => g.UserId == userId)
                .Select(g => new GradeDto
                {
                    Id = g.Id,
                    UserFullName = g.User.FullName,
                    Number = g.Number,
                    Period = $"{g.StartDate.ToString("dd.MM.yyyy")} - {g.EndDate.ToString("dd.MM.yyyy")}",
                    DateOfCreation = g.DateOfCreation.ToString("dd.MM.yyyy"),
                })
                .OrderBy(g => g.Number)
                .ToListAsync();

            return grades;
        }

        public async Task<List<AssessmentResultDto>> GetColleaguesAssessmentResultsForAssessment(int userId)
        {
            var dtos = new List<AssessmentResultDto>();
            var colleaguesAssessmentResultsForAssessment = await _unitOfWork.AssessmentResults
                .GetAllAsync(x => x.JudgeId == userId && x.Assessment.UserId != userId && x.SystemStatus == SystemStatuses.PENDING && x.AssignedBy.HasValue);

            foreach (var assessmentResult in colleaguesAssessmentResultsForAssessment)
            {
                var assessment = await _unitOfWork.Assessments.GetAsync(x => x.Id == assessmentResult.AssessmentId, includeProperties:
                [
                        "User",
                        "AssessmentResults.AssessmentResultValues",
                        "AssessmentType.AssessmentMatrix.Elements"
                ]);

                var assessmentResultDto = new AssessmentResultDto
                {
                    Id = assessmentResult.Id,
                    SystemStatus = assessmentResult.SystemStatus,
                    Type = assessmentResult.Type,
                    Sum = assessmentResult.AssessmentResultValues.Sum(x => x.Value),
                    TypeName = assessment.AssessmentType.Name,
                    MinValue = assessment.AssessmentType.AssessmentMatrix.MinAssessmentMatrixResultValue,
                    MaxValue = assessment.AssessmentType.AssessmentMatrix.MaxAssessmentMatrixResultValue,
                };

                assessmentResultDto.Judge = new UserExtendedDto
                {
                    Id = userId,
                };

                assessmentResultDto.Judged = new UserExtendedDto
                {
                    Id = assessment.UserId,
                    ImagePath = assessmentResult.Assessment.User.ImagePath,
                    FullName = assessmentResult.Assessment.User.FullName,
                };

                foreach (var value in assessmentResult.AssessmentResultValues)
                {
                    assessmentResultDto.Values.Add(new AssessmentResultValueDto
                    {
                        Value = value.Value,
                        AssessmentMatrixRow = value.AssessmentMatrixRow,
                    });
                }

                foreach (var element in assessment.AssessmentType.AssessmentMatrix.Elements)
                {
                    assessmentResultDto.Elements.Add(new AssessmentMatrixElementDto
                    {
                        Column = element.Column,
                        Row = element.Row,
                        Value = element.Value,
                        HtmlClassName = element.HtmlClassName
                    });
                }

                assessmentResultDto.ElementsByRow = assessmentResultDto.Elements
                    .OrderBy(x => x.Column)
                    .GroupBy(x => x.Row)
                    .OrderBy(x => x.Key)
                    .ToList();

                dtos.Add(assessmentResultDto);
            }

            return dtos;
        }

        public async Task AssessUser(AssessUserDto assessUserDto)
        {


            var assessmentResult = await _unitOfWork.AssessmentResults.GetAsync(x => x.Id == assessUserDto.AssessmentResultId,
                includeProperties: [
                    "Judge",
                    "Assessment.AssessmentResults",
                    "Assessment.AssessmentType",
            ]);

            if (assessmentResult == null)
            {
                throw new Exception($"AssessmentResult with ID {assessUserDto.AssessmentResultId} not found.");
            }

            assessmentResult.ResultDate = DateOnly.FromDateTime(DateTime.Today);
            assessmentResult.SystemStatus = SystemStatuses.COMPLETED;

            for (int i = 0; i < assessUserDto.ResultValues.Count; i++)
            {
                assessmentResult.AssessmentResultValues.Add(new AssessmentResultValue
                {
                    Value = Convert.ToInt32(assessUserDto.ResultValues[i]),
                    AssessmentMatrixRow = (i + 1),
                });
            }

            _unitOfWork.AssessmentResults.Update(assessmentResult);

            if (assessmentResult.Type == AssessmentResultTypes.UrpAssessment)
            {
                var otherUrpAssessmentResults = assessmentResult.Assessment.AssessmentResults.Where(x =>
                    x.AssessmentId == assessmentResult.AssessmentId &&
                    x.Id != assessmentResult.Id &&
                    x.Type == AssessmentResultTypes.UrpAssessment)
                    .ToList();

                foreach (var urpAssessmentResult in otherUrpAssessmentResults)
                {
                    _unitOfWork.AssessmentResults.Remove(urpAssessmentResult);
                }
            }

            var assessmentType = assessmentResult.Assessment.AssessmentType.SystemAssessmentType;
            var completedAssessmentResults = assessmentResult.Assessment.AssessmentResults.Where(x => x.SystemStatus == SystemStatuses.COMPLETED).ToList();

            var IsFinalized = _assessmentService.IsAssessmentFinalized(assessmentType, completedAssessmentResults);

            if (IsFinalized)
            {
                var grade = await _unitOfWork.Grades.GetAsync(x => x.Id == assessmentResult.Assessment.GradeId);

                if (grade == null)
                {
                    throw new Exception($"Grade with ID {assessmentResult.Assessment.GradeId} not found.");
                }
                else if (assessmentType == SystemAssessmentTypes.ManagementCompetencies)
                {
                    grade.IsManagmentCompetenciesFinalized = true;
                }
                else if (assessmentType == SystemAssessmentTypes.СorporateСompetencies)
                {
                    grade.IsCorporateCompetenciesFinalized = true;
                }

                _unitOfWork.Grades.Update(grade);
            }

            _unitOfWork.AssessmentResults.Update(assessmentResult);
            await _unitOfWork.CommitAsync();
        }


        public bool CanChooseJudges(List<string> userRoles, AssessmentDto assessmentDto)
        {
            var isUserInRole = userRoles.Any(role => role == "Urp" || role == "Supervisor");
            var isAssessmentTypeCorporate = assessmentDto.SystemAssessmentType == SystemAssessmentTypes.СorporateСompetencies;
            var isStatusPending = assessmentDto.SystemStatus == SystemStatuses.PENDING;
            var completedJudgesCount = assessmentDto.CompletedAssessmentResults.Count(x => x.AssignedBy.HasValue);

            return isUserInRole && isAssessmentTypeCorporate && isStatusPending && completedJudgesCount < 3;
        }





        public async Task<User?> GetFirstSupervisorForUser(int userId)
        {
            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                throw new Exception($"User with ID {userId} not found.");

            var parentSubdivision = await _context.Subdivisions
                .AsNoTracking()
                .Include(s => s.Parent)
                .FirstOrDefaultAsync(s => s.Id == user.ParentSubdivisionId);

            if (parentSubdivision == null)
                return null;

            var supervisor = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.SystemRoles.Contains(SystemRoles.Supervisor) && u.SubordinateSubdivisions.Contains(parentSubdivision));

            if (supervisor != null)
                return supervisor;

            var rootSubdivision = parentSubdivision.Parent;

            while (rootSubdivision != null)
            {
                supervisor = await _context.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.SystemRoles.Contains(SystemRoles.Supervisor) && u.SubordinateSubdivisions.Contains(rootSubdivision));

                if (supervisor != null)
                    return supervisor;

                rootSubdivision = rootSubdivision.Parent;
            }

            return null;
        }
    }
}