using KOP.Common.DTOs;

namespace KOP.WEB.Models.ViewModels.Supervisor
{
    public class SubordinatesViewModel
    {
        public List<ModuleDTO> Modules { get; set; } = new();
    }
}