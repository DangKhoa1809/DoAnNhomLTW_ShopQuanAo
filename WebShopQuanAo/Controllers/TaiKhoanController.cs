using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebShopQuanAo.Models;

namespace WebShopQuanAo.Controllers
{
    public class TaiKhoanController : Controller
    {
        QL_ShopQuanAoEntities data = new QL_ShopQuanAoEntities();

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ViewBag.MenuCha = data.DanhMucs.Where(d => d.MaDanhMucCha == null).ToList();
            base.OnActionExecuting(filterContext);
        }

        //Đăng nhập
        public ActionResult DangNhap()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DangNhap(DangNhapViewModel model)
        {
            ViewBag.MenuCha = data.DanhMucs.Where(d => d.MaDanhMucCha == null).ToList();

            if (ModelState.IsValid)
            {
                var khachHang = data.KhachHangs.FirstOrDefault(k => k.Email == model.Email && k.MatKhau == model.Password);

                if (khachHang != null)
                {
                    Session["MaKH"] = khachHang.MaKH;
                    Session["HoTenKH"] = khachHang.HoTenKH;

                    return RedirectToAction("TrangChu", "SanPham");
                }

                var nhanVien = data.NhanViens.FirstOrDefault(nv => nv.Email == model.Email && nv.MatKhau == model.Password);
                if (nhanVien != null)
                {
                    Session["MaNV"] = nhanVien.MaNV;
                    Session["HoTenNV"] = nhanVien.HoTenNV;
                    Session["LoaiNV"] = nhanVien.LoaiNV;

                    return RedirectToAction("TrangChu", "SanPham");
                }

                ModelState.AddModelError("", "Email hoặc mật khẩu không đúng");
            }

            return View(model);
        }


        //Đăng ký
        [HttpGet]
        public ActionResult DangKy()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DangKy(DangKyViewModel model)
        {
            if (ModelState.IsValid)
            {
                var khachHang = data.KhachHangs.FirstOrDefault(k => k.Email == model.Email);
                if (khachHang != null)
                {
                    ModelState.AddModelError("Email", "Email này đã được đăng ký");
                    return View(model);
                }

                KhachHang kh = new KhachHang
                {
                    HoTenKH = model.HoTen,
                    Email = model.Email,
                    MatKhau = model.MatKhau,
                    SDT = model.SDT,
                    GioiTinh = "Khác",   
                    DiaChi = "",          
                    SoThich = ""         
                };

                data.KhachHangs.Add(kh);
                data.SaveChanges(); 

                TempData["ThongBao"] = "Đăng ký thành công! Vui lòng đăng nhập.";
                return RedirectToAction("DangNhap");
            }

            return View(model);
        }

        // Đăng xuất
        public ActionResult DangXuat()
        {
            Session.Clear();
            return RedirectToAction("TrangChu", "SanPham");
        }

        //Giải phóng tài nguyên
        protected override void Dispose(bool disposing)
        {
            if (disposing)
                data.Dispose();
            base.Dispose(disposing);
        }
    }
}