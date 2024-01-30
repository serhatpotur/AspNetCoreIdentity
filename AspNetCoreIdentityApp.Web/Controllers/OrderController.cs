using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreIdentityApp.Web.Controllers
{
    public class OrderController : Controller
    {

        [Authorize(Policy = "OrderBasicPermission")]

        public IActionResult Index()
        {
            return View();
        }
        [Authorize(Policy = "OrderAdvencedPermission")]

        public IActionResult Add()
        {
            return View();
        }
        [Authorize(Policy = "OrderAdminPermission")]
        public IActionResult Delete()
        {
            return View();
        }
        public IActionResult Update()
        {
            return View();
        }
    }
}
