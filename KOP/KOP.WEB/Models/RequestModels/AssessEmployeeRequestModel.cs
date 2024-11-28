namespace KOP.WEB.Models.RequestModels
{
    public class AssessEmployeeRequestModel
    {
        public List<string> resultValues { get; set; } = new();
        public int assessmentResultId { get; set; }
    }
}