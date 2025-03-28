namespace KOP.WEB.Models.RequestModels
{
    public class AssessUserRequestModel
    {
        public List<string> resultValues { get; set; } = new();
        public int assessmentResultId { get; set; }
    }
}