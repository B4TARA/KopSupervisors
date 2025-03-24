using KOP.BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace KOP.WEB.Controllers
{
    public class GradeController : Controller
    {
        private readonly IGradeService _gradeService;
        private readonly IUserService _userService;
        private readonly IAssessmentService _assessmentService;
        private readonly ILogger<GradeController> _logger;

        public GradeController(IGradeService gradeService, IUserService userService, IAssessmentService assessmentService, ILogger<GradeController> logger)
        {
            _gradeService = gradeService;
            _userService = userService;
            _assessmentService = assessmentService;
            _logger = logger;
        }


    }
}