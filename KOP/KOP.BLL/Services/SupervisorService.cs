using KOP.BLL.Interfaces;
using KOP.Common.Dtos;
using KOP.Common.Dtos.UserDtos;
using KOP.Common.Enums;
using KOP.DAL.Entities;
using KOP.DAL.Interfaces;

namespace KOP.BLL.Services
{
    public class SupervisorService : ISupervisorService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMappingService _mappingService;
        private readonly IAssessmentService _assessmentService;

        public SupervisorService(IUnitOfWork unitOfWork, IMappingService mappingService, IAssessmentService assessmentService)
        {
            _unitOfWork = unitOfWork;
            _mappingService = mappingService;
            _assessmentService = assessmentService;
        }

        private async Task<List<UserExtendedDto>> GetUsers(Subdivision subdivision)
        {
            var subordinateUserDtoList = new List<UserExtendedDto>();

            foreach (var user in subdivision.Users.Where(x => x.SystemRoles.Contains(SystemRoles.Employee)))
            {
                var userDto = _mappingService.CreateUserDto(user);

                subordinateUserDtoList.Add(userDto);
            }

            foreach (var childSubdivision in subdivision.Children)
            {
                var csubordinateUsersFromChild = await GetUsers(childSubdivision);

                subordinateUserDtoList.AddRange(csubordinateUsersFromChild);
            }

            return subordinateUserDtoList;
        }
        public async Task<List<SubdivisionDto>> GetSubordinateSubdivisions(int supervisorId)
        {
            var supervisor = await _unitOfWork.Users.GetAsync(x => x.Id == supervisorId,
                includeProperties: new string[]{
                    "SubordinateSubdivisions.Users.Grades",
                    "SubordinateSubdivisions.Children.Users.Grades",
            });

            if (supervisor == null)
            {
                throw new Exception($"Supervisor with ID {supervisorId} not found.");
            }

            var subordinateSubdivisionDtoList = new List<SubdivisionDto>();

            foreach (var subdivision in supervisor.SubordinateSubdivisions)
            {
                var subdivisionDto = await ProcessSubdivision(subdivision);
                if (subdivisionDto != null)
                {
                    subordinateSubdivisionDtoList.Add(subdivisionDto);
                }
            }

            return subordinateSubdivisionDtoList;
        }
        private async Task<SubdivisionDto?> ProcessSubdivision(Subdivision subdivision)
        {
            var subdivisionDto = new SubdivisionDto
            {
                Id = subdivision.Id,
                Name = subdivision.Name,
                Users = new List<UserExtendedDto>(),
                Children = new List<SubdivisionDto>()
            };

            if (subdivision.NestingLevel == 1)
            {
                subdivisionDto.IsRoot = true;
            }

            foreach (var user in subdivision.Users)
            {
                var userDto = _mappingService.CreateUserDto(user);

                subdivisionDto.Users.Add(userDto);

                if (userDto.LastGrade == null)
                {
                    continue;
                }

                //foreach (var dto in userDto.LastGrade.AssessmentDtoList)
                //{
                //    var assessmentSummaryDto = await _assessmentService.GetAssessmentSummary(dto.Id);

                //    if (dto.SystemAssessmentType == SystemAssessmentTypes.СorporateСompetencies)
                //    {
                //        userDto.LastGrade.IsCorporateCompetenciesFinalized = assessmentSummaryDto.IsFinalized;
                //    }
                //    else if (dto.SystemAssessmentType == SystemAssessmentTypes.ManagementCompetencies)
                //    {
                //        userDto.LastGrade.IsManagmentCompetenciesFinalized = assessmentSummaryDto.IsFinalized;
                //    }
                //}
            }

            foreach (var child in subdivision.Children)
            {
                var childSubdivisionDto = await ProcessSubdivision(child);
                if (childSubdivisionDto != null)
                {
                    subdivisionDto.Children.Add(childSubdivisionDto);
                }
            }

            return subdivisionDto.Users.Count > 0 || subdivisionDto.Children.Count > 0 ? subdivisionDto : null;
        }

        public async Task<List<UserReducedDto>> GetSubordinateUsersSummariesHasGrade(int supervisorId)
        {
            var supervisor = await _unitOfWork.Users.GetAsync(
                x => x.Id == supervisorId,
                includeProperties:
                [
                    "SubordinateSubdivisions.Users.Grades",
                    "SubordinateSubdivisions.Children.Users.Grades",
                ]
            );

            if (supervisor == null)
            {
                throw new Exception($"User with ID {supervisorId} not found.");
            }

            var allSubordinateUsers = new List<UserReducedDto>();

            foreach (var subdivision in supervisor.SubordinateSubdivisions)
            {
                var subordinateUsers = await GetSubordinateUsersSummariesHasGrade(subdivision);

                allSubordinateUsers.AddRange(subordinateUsers);
            }

            return allSubordinateUsers;
        }
        private async Task<List<UserReducedDto>> GetSubordinateUsersSummariesHasGrade(Subdivision subdivision)
        {
            var subordinateUsers = new List<UserReducedDto>();

            foreach (var user in subdivision.Users.Where(x => x.SystemRoles.Contains(SystemRoles.Employee) && x.Grades.Any()))
            {
                subordinateUsers.Add(new UserReducedDto
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Position = user.Position,
                    SubdivisionFromFile = user.SubdivisionFromFile,
                });
            }

            foreach (var childSubdivision in subdivision.Children)
            {
                var subordinateUsersFromChildSubdivision = await GetSubordinateUsersSummariesHasGrade(childSubdivision);

                subordinateUsers.AddRange(subordinateUsersFromChildSubdivision);
            }

            return subordinateUsers;
        }

        public async Task ApproveGrade(int gradeId)
        {
            var grade = await _unitOfWork.Grades.GetAsync(x => x.Id == gradeId, includeProperties: "Assessments.AssessmentResults");

            if (grade == null)
            {
                throw new Exception($"Grade with ID {gradeId} not found.");
            }

            grade.GradeStatus = GradeStatuses.COMPLETED;
            grade.SystemStatus = SystemStatuses.COMPLETED;

            foreach (var assessment in grade.Assessments)
            {
                var pendingAssessmentResults = assessment.AssessmentResults.Where(x => x.SystemStatus == SystemStatuses.PENDING);
                foreach (var result in pendingAssessmentResults)
                {
                    _unitOfWork.AssessmentResults.Remove(result);
                }

                assessment.SystemStatus = SystemStatuses.COMPLETED;
            }

            _unitOfWork.Grades.Update(grade);
            await _unitOfWork.CommitAsync();
        }
    }
}