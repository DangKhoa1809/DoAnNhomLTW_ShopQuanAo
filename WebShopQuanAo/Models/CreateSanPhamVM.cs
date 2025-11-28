using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebShopQuanAo.Models
{
	public class CreateSanPhamVM
	{
        [Required(ErrorMessage = "Vui lòng nhập tên sản phẩm")]
        public string TenSP { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập giá sản phẩm")]
        public decimal Gia { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn danh mục")]
        public int? MaDanhMuc { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập chất liệu")]
        public string ChatLieu { get; set; }

        public string Form { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mô tả chi tiết")]
        public string MoTaChiTiet { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số lượng tồn")]
        public int SoLuongTon { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ảnh đại diện")]
        public HttpPostedFileBase HinhDaiDien { get; set; }

        public HttpPostedFileBase HinhHover { get; set; }
        public HttpPostedFileBase HinhPhu1 { get; set; }
        public HttpPostedFileBase HinhPhu2 { get; set; }
        public HttpPostedFileBase HinhPhu3 { get; set; }

        public bool HienThi { get; set; }
    }
}