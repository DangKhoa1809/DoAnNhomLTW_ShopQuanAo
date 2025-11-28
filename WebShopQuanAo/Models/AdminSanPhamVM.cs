using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebShopQuanAo.Models
{
	public class AdminSanPhamVM
	{
        public long MaSP { get; set; }
        public string TenSP { get; set; }
        public string DanhMuc { get; set; }
        public decimal Gia { get; set; }
        public bool HienThi { get; set; }
        public DateTime NgayTao { get; set; }

        public string MauSac { get; set; }
        public string Size { get; set; }
        public int TongTon { get; set; }

        public string HinhDaiDien { get; set; } 
    }
}