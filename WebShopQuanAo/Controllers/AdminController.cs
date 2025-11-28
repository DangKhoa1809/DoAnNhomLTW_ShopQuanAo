using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebShopQuanAo.Models;

namespace WebShopQuanAo.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        QL_ShopQuanAoEntities data = new QL_ShopQuanAoEntities();

        //Trang chủ admin
        public ActionResult TrangChuAdmin()
        {
            return View();
        }

        //Bảng danh sách sản phẩm
        public ActionResult SanPhamAdmin()
        {
            var list = data.SanPhams.Include("DanhMuc").Include("BienTheSanPhams").ToList().Select(sp => new AdminSanPhamVM
               {
                   MaSP = sp.MaSP,
                   TenSP = sp.TenSP,
                   DanhMuc = sp.DanhMuc != null ? sp.DanhMuc.TenDanhMuc : "Không có",
                   Gia = sp.Gia,
                   HienThi = sp.HienThi ?? false,
                   NgayTao = sp.NgayTao ?? DateTime.Now,
                   TongTon = sp.BienTheSanPhams.Sum(bt => bt.SoLuongTon ?? 0),
                   MauSac = sp.BienTheSanPhams.FirstOrDefault()?.MauSac ?? "-",
                   Size = sp.BienTheSanPhams.FirstOrDefault()?.Size ?? "-",
                   HinhDaiDien = sp.HinhDaiDien 
               }).ToList();

            return View(list);
        }

        public ActionResult QuanLy()
        {
            var list = data.SanPhams.Include("DanhMuc").Include("BienTheSanPhams").ToList();

            return View(list);
        }

        //Thêm mới sản phẩm
        public ActionResult Create()
        {
            ViewBag.DanhMuc = new SelectList(data.DanhMucs, "MaDanhMuc", "TenDanhMuc");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreateSanPhamVM model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.DanhMuc = new SelectList(data.DanhMucs, "MaDanhMuc", "TenDanhMuc");
                return View(model);
            }

            if (model.HinhDaiDien == null)
            {
                ModelState.AddModelError("HinhDaiDien", "Hình đại diện là bắt buộc.");
                ViewBag.DanhMuc = new SelectList(data.DanhMucs, "MaDanhMuc", "TenDanhMuc");
                return View(model);
            }

            string fileName = Guid.NewGuid() + System.IO.Path.GetExtension(model.HinhDaiDien.FileName);
            string path = Server.MapPath("~/Images/Products/" + fileName);
            model.HinhDaiDien.SaveAs(path);

            SanPham sp = new SanPham
            {
                TenSP = model.TenSP,
                Gia = model.Gia,
                MaDanhMuc = model.MaDanhMuc,
                ChatLieu = model.ChatLieu,
                Form = model.Form,
                MoTaChiTiet = model.MoTaChiTiet,
                HienThi = model.HienThi,
                NgayTao = DateTime.Now,
                HinhDaiDien = fileName 
            };

            data.SanPhams.Add(sp);
            data.SaveChanges();

            BienTheSanPham bt = new BienTheSanPham
            {
                MaSP = sp.MaSP,
                Size = "Free",
                MauSac = "Default",
                Gia = sp.Gia,
                SoLuongTon = model.SoLuongTon,
                NgayTao = DateTime.Now
            };
            data.BienTheSanPhams.Add(bt);
            data.SaveChanges();

            UploadHinhPhu(sp.MaSP, model.HinhHover, 0);
            UploadHinhPhu(sp.MaSP, model.HinhPhu1, 1);
            UploadHinhPhu(sp.MaSP, model.HinhPhu2, 2);
            UploadHinhPhu(sp.MaSP, model.HinhPhu3, 3);

            return RedirectToAction("SanPhamAdmin");
        }

        private void UploadHinhPhu(long maSP, HttpPostedFileBase file, int thuTu)
        {
            if (file != null)
            {
                var hinhCu = data.HinhAnhSanPhams.FirstOrDefault(h => h.MaSP == maSP && h.ThuTuHinh == thuTu);

                string fileName = Guid.NewGuid() + System.IO.Path.GetExtension(file.FileName);
                string path = Server.MapPath("~/Images/Products/" + fileName);
                file.SaveAs(path);

                if (hinhCu != null)
                {
                    var oldPath = Server.MapPath("~/Images/Products/" + hinhCu.TenHinh);
                    if (System.IO.File.Exists(oldPath))
                        System.IO.File.Delete(oldPath);

                    hinhCu.TenHinh = fileName;
                }
                else
                {
                    HinhAnhSanPham h = new HinhAnhSanPham
                    {
                        MaSP = maSP,
                        TenHinh = fileName,
                        ThuTuHinh = thuTu
                    };
                    data.HinhAnhSanPhams.Add(h);
                }

                data.SaveChanges();
            }
        }

        //Chỉnh sửa sản phẩm
        public ActionResult Edit(long id)
        {
            var sp = data.SanPhams.Include("BienTheSanPhams").FirstOrDefault(x => x.MaSP == id);

            if (sp == null) return HttpNotFound();

            var model = new EditSanPhamVM
            {
                MaSP = sp.MaSP,
                TenSP = sp.TenSP,
                MaDanhMuc = sp.MaDanhMuc,
                Gia = sp.Gia,
                SoLuongTon = sp.BienTheSanPhams.FirstOrDefault()?.SoLuongTon ?? 0,
                ChatLieu = sp.ChatLieu,
                Form = sp.Form,
                MoTaChiTiet = sp.MoTaChiTiet,
                HienThi = sp.HienThi ?? true,
                HinhDaiDien = sp.HinhDaiDien
            };

            ViewBag.DanhMuc = new SelectList(data.DanhMucs, "MaDanhMuc", "TenDanhMuc", sp.MaDanhMuc);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EditSanPhamVM model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.DanhMuc = new SelectList(data.DanhMucs, "MaDanhMuc", "TenDanhMuc", model.MaDanhMuc);
                return View(model);
            }

            var sp = data.SanPhams.FirstOrDefault(x => x.MaSP == model.MaSP);
            if (sp == null) return HttpNotFound();

            sp.TenSP = model.TenSP;
            sp.Gia = model.Gia;
            sp.MaDanhMuc = model.MaDanhMuc;
            sp.ChatLieu = model.ChatLieu;
            sp.Form = model.Form;
            sp.MoTaChiTiet = model.MoTaChiTiet;
            sp.HienThi = model.HienThi;
            sp.NgayCapNhat = DateTime.Now;

            var bienThe = data.BienTheSanPhams.FirstOrDefault(bt => bt.MaSP == model.MaSP);
            if (bienThe != null)
            {
                bienThe.SoLuongTon = model.SoLuongTon;
                bienThe.Gia = model.Gia;
            }

            if (model.HinhDaiDienFile != null)
            {
                var oldPath = Server.MapPath("~/Images/Products/" + sp.HinhDaiDien);
                if (System.IO.File.Exists(oldPath)) System.IO.File.Delete(oldPath);

                string fileName = Guid.NewGuid() + System.IO.Path.GetExtension(model.HinhDaiDienFile.FileName);
                string path = Server.MapPath("~/Images/Products/" + fileName);
                model.HinhDaiDienFile.SaveAs(path);
                sp.HinhDaiDien = fileName;
            }

            UploadHinhPhu(sp.MaSP, model.HinhHover, 0);
            UploadHinhPhu(sp.MaSP, model.HinhPhu1, 1);
            UploadHinhPhu(sp.MaSP, model.HinhPhu2, 2);
            UploadHinhPhu(sp.MaSP, model.HinhPhu3, 3);

            data.SaveChanges();

            return RedirectToAction("SanPhamAdmin");
        }

        //Xóa sản phẩm
        public ActionResult Delete(long id)
        {
            var sp = data.SanPhams.Include("BienTheSanPhams").Include("HinhAnhSanPhams").FirstOrDefault(x => x.MaSP == id);

            if (sp == null) return HttpNotFound();

            if (!string.IsNullOrEmpty(sp.HinhDaiDien))
            {
                var path = Server.MapPath("~/Images/Products/" + sp.HinhDaiDien);
                if (System.IO.File.Exists(path)) System.IO.File.Delete(path);
            }

            foreach (var hinh in sp.HinhAnhSanPhams.ToList())
            {
                var path = Server.MapPath("~/Images/Products/" + hinh.TenHinh);
                if (System.IO.File.Exists(path)) System.IO.File.Delete(path);

                data.HinhAnhSanPhams.Remove(hinh);
            }

            foreach (var bt in sp.BienTheSanPhams.ToList())
            {
                data.BienTheSanPhams.Remove(bt);
            }

            data.SanPhams.Remove(sp);
            data.SaveChanges();

            return RedirectToAction("SanPhamAdmin");
        }
    }
}