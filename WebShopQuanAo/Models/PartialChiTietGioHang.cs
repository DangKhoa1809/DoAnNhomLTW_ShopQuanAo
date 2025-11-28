using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebShopQuanAo.Models
{
    public class PartialChiTietGioHang
    {
        public string MauSac { get; set; }
        public string Size { get; set; }

        public SanPham SanPham { get; set; }

        public int SoLuong { get; set; } = 0;

        public decimal DonGia => SanPham?.Gia ?? 0;

        public decimal ThanhTien => DonGia * SoLuong;

        public string HinhAnh => SanPham?.HinhDaiDien; 
    }

}