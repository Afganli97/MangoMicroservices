using Mango.Services.ShopingCartAPI.Models.DTOs;
using Newtonsoft.Json;

namespace Mango.Services.ShopingCartAPI.Repository
{
    public class CouponRepository : ICouponRepository
    {
        private readonly HttpClient _client;

        public CouponRepository(HttpClient client)
        {
            _client = client;
        }

        public async Task<CouponDto> GetCoupon(string couponCode)
        {
            var responseJson = await _client.GetAsync("/api/coupon/" + couponCode);
            var apiContent = await responseJson.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<ResponseDto>(apiContent);
            if (response.IsSuccess)
            {
                return JsonConvert.DeserializeObject<CouponDto>(Convert.ToString(response.Result));
            }
            return new CouponDto();
        }
    }
}