using KOP.Common.Dtos.UserDtos;

namespace KOP.Common.Dtos
{
    public class SubdivisionDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<UserDto> Users { get; set; } = new();
        public List<SubdivisionDto> Children { get; set; } = new();
        public bool IsRoot { get; set; }
    }
}