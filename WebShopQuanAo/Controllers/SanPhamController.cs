using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;
using WebShopQuanAo.Models;

namespace WebShopQuanAo.Controllers
{
    public class SanPhamController : Controller
    {
        QL_ShopQuanAoEntities data = new QL_ShopQuanAoEntities();

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ViewBag.MenuCha = data.DanhMucs.Where(d => d.MaDanhMucCha == null).OrderBy(d => d.MaDanhMuc).ToList();
            base.OnActionExecuting(filterContext);
        }

        //Tìm kiếm
        public ActionResult TimKiem(string keyword)
        {
            if (string.IsNullOrEmpty(keyword))
            {
                return RedirectToAction("DanhSachSanPham");
            }

            var list = data.SanPhams.Where(sp => sp.TenSP.Contains(keyword)).OrderByDescending(sp => sp.NgayTao).ToList();

            ViewBag.Keyword = keyword;
            ViewBag.TitlePage = "Kết quả tìm kiếm cho: " + keyword;

            return View("DanhSachSanPham", list);
        }


        // Trang chủ
        public ActionResult TrangChu()
        {
            ViewBag.HangMoi = LaySanPhamMoi(5);
            ViewBag.HangBanChay = LayHangBanChay(5);
            ViewBag.DanhSachAo = LaySanPhamTheoDanhMucCha(1, 5);
            ViewBag.DanhSachQuan = LaySanPhamTheoDanhMucCha(2, 5);
            ViewBag.DanhSachPhuKien = LaySanPhamTheoDanhMucCha(3, 5);

            return View();
        }

        #region Helper Methods

        private List<SanPham> LaySanPhamMoi(int take)
        {
            return data.SanPhams.OrderByDescending(sp => sp.NgayTao).Take(take).ToList();
        }

        private List<SanPham> LayHangBanChay(int take)
        {
            return data.ChiTietDonHangs.GroupBy(ct => ct.MaSP).Select(g => new { MaSP = g.Key, SoLuong = g.Sum(x => x.SoLuong) }).OrderByDescending(x => x.SoLuong).Join(data.SanPhams, g => g.MaSP, sp => sp.MaSP, (g, sp) => sp).Take(take).ToList();
        }

        private List<SanPham> LaySanPhamTheoDanhMucCha(int maDanhMucCha, int take)
        {
            var danhMucCon = data.DanhMucs.Where(d => d.MaDanhMucCha == maDanhMucCha).Select(d => d.MaDanhMuc).ToList();

            danhMucCon.Add(maDanhMucCha); 

            return data.SanPhams.Where(sp => sp.MaDanhMuc.HasValue && danhMucCon.Contains(sp.MaDanhMuc.Value)).OrderByDescending(sp => sp.NgayTao).Take(take).ToList();
        }

        private List<SanPham> LaySanPhamTheoSlug(string slug)
        {
            if (slug == "tat-ca") return data.SanPhams.ToList();
            if (slug == "hang-moi") return LaySanPhamMoi(int.MaxValue);
            if (slug == "hang-ban-chay") return LayHangBanChay(int.MaxValue);

            var dm = data.DanhMucs.FirstOrDefault(d => d.Slug == slug);
            if (dm == null) return data.SanPhams.ToList();

            var danhMucCon = data.DanhMucs.Where(d => d.MaDanhMucCha == dm.MaDanhMuc).Select(d => d.MaDanhMuc).ToList();
            danhMucCon.Add(dm.MaDanhMuc);

            return data.SanPhams.Where(sp => sp.MaDanhMuc.HasValue && danhMucCon.Contains(sp.MaDanhMuc.Value)).ToList();
        }

        #endregion

        // Trang danh sách sản phẩm
        public ActionResult DanhSachSanPham(string slug = "tat-ca", string sort = "default")
        {
            var list = LaySanPhamTheoSlug(slug);

            switch (sort)
            {
                case "price_asc":
                    list = list.OrderBy(sp => sp.Gia).ToList();
                    break;
                case "price_desc":
                    list = list.OrderByDescending(sp => sp.Gia).ToList();
                    break;
                case "name_asc":
                    list = list.OrderBy(sp => sp.TenSP).ToList();
                    break;
                case "name_desc":
                    list = list.OrderByDescending(sp => sp.TenSP).ToList();
                    break;
                case "newest":
                    list = list.OrderByDescending(sp => sp.NgayTao).ToList();
                    break;
                case "oldest":
                    list = list.OrderBy(sp => sp.NgayTao).ToList();
                    break;
                case "best_selling":
                    list = LayHangBanChay(int.MaxValue);
                    break;
                default:
                    break;
            }

            int total = list.Count;
            int hienTai = 30;

            if (Request["take"] != null)
            {
                int.TryParse(Request["take"], out hienTai);
            }

            hienTai = Math.Min(hienTai, total);
            ViewBag.HienXemThem = hienTai < total;
            ViewBag.SoLuongHienTai = hienTai;
            ViewBag.Slug = slug;

            if (slug == "tat-ca")
            {
                ViewBag.TitlePage = "Tất cả sản phẩm";
                ViewBag.Banner = "/Images/Banners/tat-ca-san-pham-banner.jpg";
            }
            else if (slug == "hang-moi")
            {
                ViewBag.TitlePage = "Hàng mới";
                ViewBag.Banner = "/Images/Banners/hang-moi-banner.jpg";
            }
            else if (slug == "hang-ban-chay")
            {
                ViewBag.TitlePage = "Hàng bán chạy";
                ViewBag.Banner = "/Images/Banners/hang-ban-chay-banner.jpg";
            }
            else
            {
                var dm = data.DanhMucs.FirstOrDefault(d => d.Slug == slug);
                ViewBag.TitlePage = dm?.TenDanhMuc ?? "Sản phẩm";
                ViewBag.Banner = "/Images/Banners/" + (dm?.Banner ?? "banner-tatca.jpg");
            }

            return View(list.Take(hienTai).ToList());
        }

        //Chi tiết sản phẩm
        public ActionResult ChiTietSanPham(long id)
        {
            using (var db = new QL_ShopQuanAoEntities())
            {
                var sp = db.SanPhams.Include("BienTheSanPhams").Include("HinhAnhSanPhams").Include("DanhMuc").FirstOrDefault(x => x.MaSP == id);

                if (sp == null)
                    return HttpNotFound();

                var nhomKhongCoSize = new List<string>
                {
                    "Nón", "Balo - Túi xách", "Ví", "Mắt kính"
                };

                bool coSize = !nhomKhongCoSize.Contains(sp.DanhMuc.TenDanhMuc);

                ViewBag.CoSize = coSize;

                if (coSize)
                {
                    var mauSac = sp.BienTheSanPhams.Select(x => x.MauSac).Distinct().ToList();

                    var firstColor = mauSac.FirstOrDefault();
                    var sizeTheoMau = sp.BienTheSanPhams.Where(x => x.MauSac == firstColor).Select(x => x.Size).Distinct().ToList();

                    ViewBag.MauSac = mauSac;
                    ViewBag.SizeTheoMau = sizeTheoMau;
                }
                else
                {
                    ViewBag.MauSac = sp.BienTheSanPhams.Select(x => x.MauSac).Distinct().ToList();

                    ViewBag.SizeTheoMau = new List<string>(); 
                }

                var spLienQuan = db.SanPhams.Include("HinhAnhSanPhams").Where(x => x.MaDanhMuc == sp.MaDanhMuc && x.MaSP != id).OrderByDescending(x => x.NgayTao).Take(10).ToList();

                ViewBag.SP_LienQuan = spLienQuan;

                return View(sp);
            }
        }

        public JsonResult GetSizeTheoMau(long maSP, string mau)
        {
            using (var db = new QL_ShopQuanAoEntities())
            {
                var sizes = db.BienTheSanPhams.Where(x => x.MaSP == maSP && x.MauSac == mau).Select(x => x.Size).Distinct().ToList();

                return Json(sizes, JsonRequestBehavior.AllowGet);
            }
        }

        //Giải phóng tài nguyên
        protected override void Dispose(bool disposing)
        {
            if (disposing) data.Dispose();
            base.Dispose(disposing);
        }
    }
}