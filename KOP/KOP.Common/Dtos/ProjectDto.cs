﻿namespace KOP.Common.Dtos.GradeDtos
{
    public class ProjectDto
    {
        public int? Id { get; set; }
        public string UserRole { get; set; }
        public string Name { get; set; }
        public string Stage { get; set; }
        public DateTime StartDateTime { get; set; } = DateTime.Today;
        public DateOnly StartDate => DateOnly.FromDateTime(StartDateTime);
        public DateTime EndDateTime { get; set; } = DateTime.Today;
        public DateOnly EndDate => DateOnly.FromDateTime(EndDateTime);
        public string SuccessRate { get; set; }
        public string AverageKpi { get; set; }
        public string SP { get; set; }
    }
}