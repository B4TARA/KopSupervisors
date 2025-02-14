using System.Security.Claims;
using KOP.BLL.Interfaces;
using KOP.WEB.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StatusCodes = KOP.Common.Enums.StatusCodes;

namespace KOP.WEB.Controllers
{
    public class ReportController : Controller
    {
        private readonly ISupervisorService _supervisorService;

        public ReportController(ISupervisorService supervisorService)
        {
            _supervisorService = supervisorService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetGradeReportLayout()
        {
            try
            {
                var supervisorId = Convert.ToInt32(User.FindFirstValue("Id"));
                var getSubordinateEmployeesRes = await _supervisorService.GetSubordinateUsers(supervisorId);

                if (!getSubordinateEmployeesRes.HasData)
                {
                    return View("Error", new ErrorViewModel
                    {
                        StatusCode = getSubordinateEmployeesRes.StatusCode,
                        Message = getSubordinateEmployeesRes.Description
                    });
                }

                var subordinateUsersWithGrades = getSubordinateEmployeesRes.Data.Where(x => x.Grades.Any()).ToList();

                return View("GradeReportLayout", subordinateUsersWithGrades);
            }
            catch
            {
                return View("Error", new ErrorViewModel
                {
                    StatusCode = StatusCodes.InternalServerError,
                    Message = "An unexpected error occurred. Please try again later."
                });
            }
        }

        //[HttpPost]
        //[Authorize(Roles = "Urp, Umst, Uprb, Uko, SupervisorUdpo, SupervisorUprb, SupervisorUko")]
        //public async Task<JsonResult> SaveReport1(string viewDate)
        //{
        //    try
        //    {
        //        int serviceNumber = Convert.ToInt32(User.FindFirst("service_number").Value);


        //        var response = await _reportService.SaveReport1(viewDate, serviceNumber);


        //        Response.StatusCode = (int)response.StatusCode;


        //        if (response.StatusCode != Domain.Enums.StatusCodes.OK)
        //        {
        //            return Json("Упс... Что-то пошло не так: " + response.Description);
        //        }


        //        return Json(response.Data);
        //    }


        //    catch (Exception ex)
        //    {
        //        Response.StatusCode = 400;


        //        return Json("Упс... Что-то пошло не так : " + ex.Message);
        //    }
        //}
    }
}
