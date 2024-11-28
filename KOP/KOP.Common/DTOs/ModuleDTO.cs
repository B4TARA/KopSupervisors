namespace KOP.Common.DTOs
{
    public class ModuleDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<EmployeeDTO> Employees { get; set; } = new();
        public List<ModuleDTO> Children { get; set; } = new();
        public bool IsRoot { get; set; }
    }
}