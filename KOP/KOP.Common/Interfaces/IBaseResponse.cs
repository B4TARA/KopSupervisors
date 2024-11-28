
using KOP.Common.Enums;

namespace KOP.Common.Interfaces
{
    public interface IBaseResponse<T>
    {
        string? Description { get; }
        StatusCodes StatusCode { get; }
        T? Data { get; }
    }
}