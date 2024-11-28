using StatusCodes = KOP.Common.Enums.StatusCodes;

namespace KOP.WEB.Models.ViewModels
{
    public class ErrorViewModel
    {
        public StatusCodes StatusCode { get; set; }
        public string? Message { get; set; }
    }
}