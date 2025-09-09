using HINOSystem.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;

namespace HINOSystem.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {

        public HomeController()
        {
        }

        [Authorize]
        public IActionResult Index()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Logout()
        {
            try
            {
                //this.fncActionLog("LOGOUT", "OK");

                //string _url = "~/" + this.Systems;
                //if (HttpContext.Session.GetString("HINO").ToString() == "YES") _url = "~/" + this.Systems + "/Hino";


                HttpContext.Session.Clear();
                return Redirect("~/");
            }
            catch (Exception ex)
            {
                HttpContext.Session.Clear();
                return Redirect("~/");
            }

        }
    }

}