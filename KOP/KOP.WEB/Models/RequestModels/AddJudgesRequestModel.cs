namespace KOP.WEB.Models.RequestModels
{
    public class AddJudgesRequestModel
    {
        public int assessmentId {  get; set; }
        public List<int> judgesIds { get; set; } = new();
    }
}