using KOP.Common.Enums;
using KOP.Common.Interfaces;

namespace KOP.Common.DTOs
{
    public class BaseResponse<T> : IBaseResponse<T>
    {
        public string? Description { get; set; }
        public StatusCodes StatusCode { get; set; }
        public T? Data { get; set; }
    }
}