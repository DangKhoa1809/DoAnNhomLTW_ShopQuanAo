using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebShopQuanAo.Models
{
	public class EditSanPhamVM
	{
        public long MaSP { get; set; }

        [Required(ErrorMessage = "Tên sản phẩm không được để trống")]
        public string TenSP { get; set; }

        [Required(ErrorMessage = "Danh mục không được để trống")]
        public int? MaDanhMuc { get; set; }

        [Required(ErrorMessage = "Giá không được để trống")]
        public decimal Gia { get; set; }

        [Required(ErrorMessage = "Số lượng tồn không được để trống")]
        public int SoLuongTon { get; set; }

        public string ChatLieu { get; set; }
        public string Form { get; set; }
        public string MoTaChiTiet { get; set; }
        public bool HienThi { get; set; }

        public string HinhDaiDien { get; set; }

        public HttpPostedFileBase HinhDaiDienFile { get; set; }
        public HttpPostedFileBase HinhHover { get; set; }
        public HttpPostedFileBase HinhPhu1 { get; set; }
        public HttpPostedFileBase HinhPhu2 { get; set; }
        public HttpPostedFileBase HinhPhu3 { get; set; }

    }
}