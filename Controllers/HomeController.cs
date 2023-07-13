using Microsoft.AspNetCore.Mvc;
using Officel.Models;
using System.Diagnostics;

namespace Officel.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            //時間控制測試
            //今天宣告
            DateTime Today = DateTime.Now;
            DateTime someday = new DateTime(1996, 7, 4);
            ViewData["Now"] = Today.ToString("yyyy-MM-dd");
            //這個月月初
            ViewData["MonthFirst"] = Today.AddDays(1-Today.Day).ToString("yyyy-MM-dd");
            //上個月月底
            ViewData["MonthLast"] = Today.AddDays(1 - Today.Day).AddDays(-1).ToString("yyyy-MM-dd");
            //上個月月初
            ViewData["LastMonth"] = Today.AddDays(1-Today.Day).AddMonths(-1).ToString("yyyy-MM-dd");
            //某指定數字
            ViewData["SomeDay"] = someday;
            //日期比較
            ViewData["Comparison"] = someday > Today;

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}