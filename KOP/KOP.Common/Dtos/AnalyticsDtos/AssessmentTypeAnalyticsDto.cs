namespace KOP.Common.Dtos.AnalyticsDtos
{
    public class AssessmentTypeAnalyticsDto
    {
        public string TypeName { get; set; }
        public List<string> LabelsArray { get; set; } = new();
        public List<double> SelfDataArray { get; set; } = new();
        public List<double> SupervisorDataArray { get; set; } = new();
        public List<double> ColleaguesDataArray { get; set; } = new();
        public double GeneralAvgValue { get; set; }
        public double SelfAvgValue { get; set; }
        public double SupervisorAvgValue { get; set; }
        public double ColleaguesAvgValue { get; set; }
    }
}