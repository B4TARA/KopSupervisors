using KOP.Common.Enums;

namespace KOP.WEB.Models.ViewModels.Admin
{
    public class UserSystemRolesViewModel
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public List<CheckboxRole> CheckboxRoles { get; set; } = new();
    }

    public class CheckboxRole
    {
        public SystemRoles Role { get; set; }
        public bool Checked { get; set; }
    }
}