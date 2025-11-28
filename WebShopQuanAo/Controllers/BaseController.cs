using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebShopQuanAo.Models;

namespace WebShopQuanAo.Controllers
{
    public class BaseController : Controller
    {
        // GET: Base
        QL_ShopQuanAoEntities data = new QL_ShopQuanAoEntities();

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ViewBag.MenuCha = data.DanhMucs.Where(d => d.MaDanhMucCha == null).ToList();
            base.OnActionExecuting(filterContext);
        }
    }
}