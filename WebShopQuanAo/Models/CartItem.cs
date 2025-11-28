using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebShopQuanAo.Models
{
    [Serializable]
    public class CartItem
    {
        public long MaSP { get; set; }
        public long? MaBienThe { get; set; }

        public string TenSP { get; set; }
        public string HinhAnh { get; set; }

        public string MauSac { get; set; }
        public string Size { get; set; }

        public decimal DonGia { get; set; }
        public int SoLuong { get; set; }

        public decimal ThanhTien => DonGia * SoLuong;
    }

}