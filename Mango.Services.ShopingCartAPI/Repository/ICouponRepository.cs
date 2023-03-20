using Mango.Services.ShopingCartAPI.Models.DTOs;

namespace Mango.Services.ShopingCartAPI.Repository
{
    public interface ICouponRepository
    {
        Task<CouponDto> GetCoupon(string couponCode);
    }
}