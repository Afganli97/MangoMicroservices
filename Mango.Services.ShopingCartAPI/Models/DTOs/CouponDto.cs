namespace Mango.Services.ShopingCartAPI.Models.DTOs
{
    public class CouponDto
    {
        public int Id { get; set; }
        public string CouponCode { get; set; }
        public double Discount { get; set; }
    }
}