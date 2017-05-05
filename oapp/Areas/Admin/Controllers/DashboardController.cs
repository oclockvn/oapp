using System.Web.Mvc;

namespace oapp.Areas.Admin.Controllers
{
    [RoutePrefix("dashboard")]
    public class DashboardController : Controller
    {
               
        public ActionResult Index()
        {
            return View();
        }
    }
}