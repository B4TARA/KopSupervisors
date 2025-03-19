using KOP.Common.Dtos.GradeDtos;
using KOP.Common.Enums;

namespace KOP.WEB.Models.ViewModels.Employee
{
    public class GradeLayoutViewModel
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Position { get; set; }
        public string SubdivisionFromFile { get; set; }
        public string GradeGroup { get; set; }
        public string WorkPeriod { get; set; }
        public DateOnly ContractEndDate { get; set; }
        public string ImagePath { get; set; }
        public GradeDto? LastGradeDto { get; set; }
        public bool IsCorporateCompetenciesFinalized { get; set; }
        public bool IsManagmentCompetenciesFinalized { get; set; }
        public bool ReadyForEmployeeApproval { get; set; }
        public bool ApprovedByEmployee { get; set; }
        public GradeStatuses GradeStatus { get; set; }
    }
}