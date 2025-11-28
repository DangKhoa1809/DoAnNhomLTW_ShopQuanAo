using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebShopQuanAo.Models;

namespace WebShopQuanAo.Controllers
{
    public class GioHangController : Controller
    {
        QL_ShopQuanAoEntities data = new QL_ShopQuanAoEntities();

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ViewBag.MenuCha = data.DanhMucs.Where(d => d.MaDanhMucCha == null).ToList();
            base.OnActionExecuting(filterContext);
        }

        //Thêm giỏ hàng
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult ThemGioHang(long maSP, string mau, string size, int soLuong)
        {
            var sp = data.SanPhams.Find(maSP);
            if (sp == null) return Json(new { status = "error" });

            var bienThe = data.BienTheSanPhams.FirstOrDefault(b => b.MaSP == maSP && b.MauSac == mau);
            string hinh;
            if (bienThe != null)
            {
                var hinhAnh = data.HinhAnhSanPhams.Where(h => h.MaBienThe == bienThe.MaBienThe).OrderBy(h => h.ThuTuHinh).FirstOrDefault();
                hinh = hinhAnh != null ? "/Images/Products/" + hinhAnh.TenHinh : "/Images/Products/" + sp.HinhDaiDien;
            }
            else
            {
                hinh = "/Images/Products/" + sp.HinhDaiDien;
            }

            var cartItem = new CartItem
            {
                MaSP = sp.MaSP,
                MaBienThe = bienThe?.MaBienThe,
                TenSP = sp.TenSP,
                HinhAnh = hinh,
                MauSac = mau,
                Size = size,
                DonGia = sp.Gia,
                SoLuong = soLuong
            };

            var cartSession = Session["Cart"] as List<CartItem> ?? new List<CartItem>();

            var existing = cartSession.FirstOrDefault(x => x.MaSP == maSP && x.MauSac == mau && x.Size == size);
            if (existing != null)
                existing.SoLuong += soLuong;
            else
                cartSession.Add(cartItem);

            Session["Cart"] = cartSession;

            return Json(new
            {
                status = "ok",
                soLuongTong = cartSession.Sum(x => x.SoLuong) 
            });
        }

        //Mua ngay
        public ActionResult MuaNgay(long maSP, string mau, string size, int sl)
        {
            var sp = data.SanPhams.Find(maSP);
            if (sp == null) return RedirectToAction("TrangChu", "Home");

            var bienThe = data.BienTheSanPhams.FirstOrDefault(b => b.MaSP == maSP && b.MauSac == mau);
            string hinh;
            if (bienThe != null)
            {
                var hinhAnh = data.HinhAnhSanPhams
                                .Where(h => h.MaBienThe == bienThe.MaBienThe)
                                .OrderBy(h => h.ThuTuHinh)
                                .FirstOrDefault();
                hinh = hinhAnh != null ? "/Images/Products/" + hinhAnh.TenHinh : "/Images/Products/" + sp.HinhDaiDien;
            }
            else
            {
                hinh = "/Images/Products/" + sp.HinhDaiDien;
            }

            var cartItem = new CartItem
            {
                MaSP = sp.MaSP,
                MaBienThe = bienThe?.MaBienThe,
                TenSP = sp.TenSP,
                HinhAnh = hinh,
                MauSac = mau,
                Size = size,
                DonGia = sp.Gia,
                SoLuong = sl
            };


            var cartSession = Session["Cart"] as List<CartItem>;
            if (cartSession == null)
                cartSession = new List<CartItem>();

            var existing = cartSession.FirstOrDefault(x => x.MaSP == maSP && x.MauSac == mau && x.Size == size);
            if (existing != null)
                existing.SoLuong += sl;  
            else
                cartSession.Add(cartItem);

            Session["Cart"] = cartSession;

            return View("ThanhToan", cartSession);
        }

        //Thanh toán
        public ActionResult ThanhToan()
        {
            var cartSession = Session["Cart"] as List<CartItem>;
            if (cartSession == null || !cartSession.Any())
                return RedirectToAction("TrangChu", "SanPham");

            return View(cartSession);
        }

        [HttpPost]
        public JsonResult CapNhatSoLuong(long maSP, string mau, string size, int soLuong)
        {
            var cart = Session["Cart"] as List<CartItem>;
            if (cart == null)
                return Json(new { success = false });

            var item = cart.FirstOrDefault(x => x.MaSP == maSP && x.MauSac == mau && x.Size == size);

            if (item == null)
                return Json(new { success = false });

            item.SoLuong = soLuong;

            Session["Cart"] = cart;

            return Json(new
            {
                success = true,
                thanhTien = item.ThanhTien,
                totalPrice = cart.Sum(x => x.ThanhTien),
                totalQty = cart.Sum(x => x.SoLuong)
            });
        }


        //Xử lý đặt hàng
        [HttpPost]
        public ActionResult DatHang(string tenKH, string diaChi, string phone)
        {
            var cart = Session["Cart"] as List<CartItem>;
            if (cart == null || !cart.Any())
                return RedirectToAction("TrangChu", "SanPham");

            decimal tongTien = cart.Sum(x => x.ThanhTien);
            string soDon = "DH" + DateTime.Now.ToString("yyyyMMddHHmmss");

            int? maKH = Session["MaKH"] as int?;

            DonHang dh = new DonHang
            {
                MaKH = maKH,      
                MaNV = null,
                SoDon = soDon,
                TrangThai = "Chờ xử lý",
                TongTien = tongTien,
                NgayTao = DateTime.Now,
                NgayCapNhat = DateTime.Now,
                DiaChiGiaoHang = diaChi
            };

            data.DonHangs.Add(dh);
            data.SaveChanges(); 

            foreach (var item in cart)
            {
                ChiTietDonHang ct = new ChiTietDonHang
                {
                    MaDH = dh.MaDH,
                    MaSP = item.MaSP,
                    MaBienThe = item.MaBienThe,
                    SoLuong = item.SoLuong,
                    Gia = item.DonGia,
                    ThanhTien = item.ThanhTien
                };

                data.ChiTietDonHangs.Add(ct);
            }
            data.SaveChanges();

            Session["Cart"] = null;

            return RedirectToAction("HoanTat");
        }


        //Xóa sản phẩm
        [HttpPost]
        public JsonResult XoaSanPham(int maSP, string mau, string size)
        {
            var cart = Session["Cart"] as List<CartItem>;
            if (cart != null)
            {
                var item = cart.FirstOrDefault(x => x.MaSP == maSP && x.MauSac == mau && x.Size == size);
                if (item != null)
                {
                    cart.Remove(item);
                    Session["Cart"] = cart;
                }
            }

            bool cartEmpty = cart == null || !cart.Any();
            int totalQty = cart?.Sum(x => x.SoLuong) ?? 0; 

            return Json(new { success = true, cartEmpty, totalQty });
        }

        //Hoàn tất đặt hàng
        public ActionResult HoanTat()
        {
            return View();
        }
    }
}