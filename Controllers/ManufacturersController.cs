using Microsoft.AspNetCore.Mvc;
using Officel.Models;
using System.Data;

namespace Officel.Controllers
{
    public class ManufacturersController : Controller
    {
        private readonly NorthwindContext _context;

        public ManufacturersController(NorthwindContext context)
        {
            _context = context;
        }

        public class global
        {
            public static string? StartDateString { get; set; }

            public static string? EndDateString { get; set; }

            public static DateTime Now { get { return DateTime.Now; } }

            public static DateTime SomeDay { get { return new DateTime(1996, 06, 14); } }

            public static DateTime StartDate { get; set; }

            public static DateTime EndDate { get; set; }
        }

        public IActionResult Homepage()
        {
            #region 日期參數
            DateTime MonthBegin = global.Now.AddDays(1 - global.Now.Day);
            DateTime LastMonthEnd = global.Now.AddDays(1 - global.Now.Day).AddDays(-1);
            DateTime LastMonthBegin = global.Now.AddDays(1 - global.Now.Day).AddMonths(-1);
            #endregion 調用NW的最早一日

            #region 當月訂單數
            string TotalOrders = (from someone in _context.Orders
                                  where someone.OrderDate <= global.Now &&  someone.OrderDate >= global.SomeDay
                                  select someone.OrderDate).Count().ToString("N0");
            #endregion 借用NW最早一日

            #region 當月出貨數
            string TotalShipped = (from someone in _context.Orders
                                   where someone.OrderDate<=global.Now && someone.OrderDate>= global.SomeDay && someone.ShippedDate != null
                                   select someone.OrderDate).Count().ToString("N0");
            #endregion 借用NW最早一日

            #region 當月總收入
            string TotalThisMonth = (from someone in _context.Orders
                           from something in _context.OrderDetails
                           where someone.OrderDate <= global.Now &&  global.SomeDay <= someone.OrderDate && someone.OrderId == something.OrderId
                           select (something.Quantity * something.UnitPrice)).Sum().ToString("C0");
            #endregion 借用NW最早一日

            #region 上月訂單數
            string LastMonthTotalOrders = (from someone in _context.Orders
                                  where someone.OrderDate <= global.Now && someone.OrderDate >= global.SomeDay
                                  select someone.OrderDate).Count().ToString("N0");
            #endregion 借用NW最早一日

            #region 上月出貨數
            string LastMonthTotalShipped = (from someone in _context.Orders
                                   where someone.OrderDate <= global.Now && someone.OrderDate >= global.SomeDay && someone.ShippedDate != null
                                   select someone.OrderDate).Count().ToString("N0");
            #endregion 借用NW最早一日

            #region 上月總收入
            string TotalLastMonth = (from someone in _context.Orders
                                     from something in _context.OrderDetails
                                     where someone.OrderDate <= global.Now && global.SomeDay <= someone.OrderDate && someone.OrderId == something.OrderId
                                     select (something.Quantity * something.UnitPrice)).Sum().ToString("C0");
            #endregion 借用NW最早一日

            #region 待處理訂單
            var Waitinglist = from OD in _context.OrderDetails
                              join O in _context.Orders on OD.OrderId equals O.OrderId
                              join P in _context.Products on OD.ProductId equals P.ProductId
                              where O.ShippedDate == null
                              orderby O.OrderDate
                              select new WaitingLists
                              {
                                  OrderDate = O.OrderDate.ToString(),
                                  OrderID = O.OrderId,
                                  Products = P.ProductName
                              };
            #endregion 直接使用Model

            #region 熱門評論
            var Comment = from someone in _context.Employees
                          orderby someone.HireDate descending
                          select new RecentlyComment
                          {
                              CommentDate = someone.HireDate.ToString(),
                              Content = someone.Title
                          };
            #endregion 預計也是使用Model

            #region 熱門商品
            var Hotsale = from someone in _context.OrderDetails
                          join something in _context.Products on someone.ProductId equals something.ProductId
                          select new { something, someone } into tempTable
                          group tempTable by tempTable.something.ProductName into TempTable
                          select new HotSales
                          {
                              ProductName = TempTable.FirstOrDefault().something.ProductName,
                              Quentity = TempTable.Sum(x => x.someone.Quantity)
                          };
            Hotsale = Hotsale.OrderByDescending(x => x.Quentity);
            #endregion 預計也是使用Model

            #region 建立Model存放資料
            Models.HomepageModels ManufacturerHomePage = new Models.HomepageModels
            {
                //類別賦值-當月訂單
                ThisMonthOrdersTotal = TotalOrders,
                //類別賦值-當月出貨
                ThisMonthShippedTotal = TotalShipped,
                //類別賦值-當月收入
                ThisMonthReveune = TotalThisMonth,
                //類別賦值-上月訂單
                LastMonthOrdersTotal = LastMonthTotalOrders,
                //類別賦值-上月出貨
                LastMonthShippedTotal = LastMonthTotalShipped,
                //類別賦值-上月收入
                LastMonthReveune = TotalLastMonth,

                //類別賦值-待處理訂單-取前五
                WaitingList = Waitinglist.Take(5).ToList(),

                //類別賦值-熱門評論-取前十
                RecentlyComments = Comment.Take(10).ToList(),

                //類別賦值-熱門商品-取前十
                HotSale = Hotsale.Take(10).ToList()
            };
            #endregion

            return View(ManufacturerHomePage);
        }

        public IActionResult Revenue()
        {
            #region 日期設定
            global.StartDateString = HttpContext.Request.Query["StartDate"];
            global.EndDateString = HttpContext.Request.Query["EndDate"];
            if (global.StartDateString is null && global.EndDateString is null)
            {
                global.StartDate = global.Now.AddMonths(-3);
                global.EndDate = global.Now;
            } else { 
                global.StartDate = Convert.ToDateTime(global.StartDateString);
                global.EndDate = Convert.ToDateTime(global.EndDateString);
            }
            ViewData["Start"] = global.StartDate.ToString("yyyy-MM-dd");
            ViewData["End"]= global.EndDate.ToString("yyyy-MM-dd");
            #endregion 日期預設跟GET接收

            #region 期間帳務
            var Period = from someone in _context.OrderDetails
                           join something in _context.Orders on someone.OrderId equals something.OrderId
                           where something.OrderDate >= global.StartDate && something.OrderDate <= global.EndDate
                           select someone.UnitPrice * someone.Quantity;
            //期間結餘
            ViewData["Revenue"] = Period.Sum().ToString("C0");

            // 期間收入
            ViewData["Income"] = Period.Where(Money => Money >= 0).Sum().ToString("C0");

            //期間支出
            ViewData["Expenses"] = Period.Where(Money => Money < 0).Sum().ToString("C0");
            #endregion

            JsonData();
            return View();
        }

        public JsonResult JsonData()
        {
            var somebody = from someone in _context.OrderDetails
                           join something in _context.Orders on someone.OrderId equals something.OrderId
                           join someelse in _context.Products on someone.ProductId equals someelse.ProductId
                           join somemore in _context.Categories on someelse.CategoryId equals somemore.CategoryId
                           where something.OrderDate >= global.StartDate && something.OrderDate <= global.Now && someone.Quantity > 100
                           orderby something.OrderDate
                           select new RevenueSearch
                           {
                               orderDate = something.OrderDate.ToString(),
                               name = someelse.ProductName,
                               categories = somemore.CategoryName,
                               id = something.OrderId,
                               quentity = someone.Quantity,
                               price = someone.UnitPrice,
                               total = decimal.Round(someone.Quantity * someone.UnitPrice, 2)
                           };
            return Json(somebody.ToList<RevenueSearch>());
        }
    }
}
