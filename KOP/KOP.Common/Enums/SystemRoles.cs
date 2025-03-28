using System.ComponentModel;

namespace KOP.Common.Enums
{
    public enum SystemRoles
    {
        [Description("Руководитель ССП")]
        Employee = 1,
        [Description("Непосредственный руководитель")]
        Supervisor = 2,
        [Description("Куратор")]
        Curator = 3,
        [Description("УМСТ")]
        Umst = 4,
        [Description("ЦУП")]
        Cup = 5,
        [Description("УРП")]
        Urp = 6,
        [Description("УОП")]
        Uop = 7,
    }
}