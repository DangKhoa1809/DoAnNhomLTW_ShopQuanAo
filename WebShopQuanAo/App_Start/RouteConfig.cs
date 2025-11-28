using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace WebShopQuanAo
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "DanhMuc",
                url: "danh-muc/{slug}",
                defaults: new { controller = "SanPham", action = "DanhSachSanPham", slug = UrlParameter.Optional }
            );

            // ⭐ ROUTE CHO HÀNG MỚI (nếu bạn muốn)
            routes.MapRoute(
                name: "HangMoi",
                url: "hang-moi",
                defaults: new { controller = "SanPham", action = "DanhSachSanPham", slug = "hang-moi" }
            );

            routes.MapRoute(
                name: "HangBanChay",
                url: "hang-ban-chay",
                defaults: new { controller = "SanPham", action = "DanhSachSanPham", slug = "hang-ban-chay" }
            );

            // ⭐ ROUTE CHO TẤT CẢ SẢN PHẨM
            routes.MapRoute(
                name: "TatCa",
                url: "tat-ca",
                defaults: new { controller = "SanPham", action = "DanhSachSanPham", slug = "tat-ca" }
            );

            routes.MapRoute(
                name: "Admin",
                url: "Admin/{action}/{id}",
                defaults: new { controller = "Admin", action = "TrangChuAdmin", id = UrlParameter.Optional }
            );


            // ⭐ ROUTE MẶC ĐỊNH (PHẢI ĐỂ CUỐI)
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "SanPham", action = "TrangChu", id = UrlParameter.Optional }
            );

        }
    }
}
