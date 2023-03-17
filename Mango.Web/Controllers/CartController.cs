using Mango.Web.Models;
using Mango.Web.Services.IServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Mango.Web.Controllers
{
    public class CartController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICartService _cartService;
        private readonly ICouponService _couponService;

        public CartController(IProductService productService, ICartService cartService, ICouponService couponService)
        {
            _productService = productService;
            _cartService = cartService;
            _couponService = couponService;
        }

        public async Task<IActionResult> Index()
        {
            return View(await LoadCartBasedOnLoggedInUser());
        }

        [HttpPost]
        public async Task<IActionResult> ApplyCoupon(CartDto cartDto)
        {
            var userId = User.Claims.Where(x=>x.Type == "sub")?.FirstOrDefault()?.Value;
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await _cartService.ApplyCoupon<ResponseDto>(cartDto, accessToken);

            if (response != null && response.IsSuccess)
            {
                return RedirectToAction(nameof(Index));
            }
            return View(await LoadCartBasedOnLoggedInUser());
        }

        [HttpPost]
        public async Task<IActionResult> RemoveCoupon(CartDto cartDto)
        {
            var userId = User.Claims.Where(x=>x.Type == "sub")?.FirstOrDefault()?.Value;
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await _cartService.RemoveCoupon<ResponseDto>(cartDto.CartHeaderDto.UserId, accessToken);

            if (response != null && response.IsSuccess)
            {
                return RedirectToAction(nameof(Index));
            }
            return View(await LoadCartBasedOnLoggedInUser());
        }

        public async Task<IActionResult> Remove(int cartDetailId)
        {
            var userId = User.Claims.Where(x=>x.Type == "sub")?.FirstOrDefault()?.Value;
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await _cartService.RemoveFromCartAsync<ResponseDto>(cartDetailId, accessToken);

            if (response != null && response.IsSuccess)
            {
                return RedirectToAction(nameof(Index));
            }
            return View(await LoadCartBasedOnLoggedInUser());
        }

        public async Task<IActionResult> CheckOut()
        {
            return View(await LoadCartBasedOnLoggedInUser());
        }

        private async Task<CartDto> LoadCartBasedOnLoggedInUser()
        {
            var userId = User.Claims.Where(x=>x.Type == "sub")?.FirstOrDefault()?.Value;
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await _cartService.GetCartByUserIdAsync<ResponseDto>(userId, accessToken);

            CartDto cartDto = new();
            if (response != null && response.IsSuccess)
            {
                cartDto = JsonConvert.DeserializeObject<CartDto>(Convert.ToString(response.Result));
            }

            if (cartDto.CartHeaderDto != null)
            {
                if (!string.IsNullOrEmpty(cartDto.CartHeaderDto.CouponCode))
                {
                    var couponRes = await _couponService.GetCoupon<ResponseDto>(cartDto.CartHeaderDto.CouponCode, accessToken);
                    if (couponRes != null && couponRes.IsSuccess)
                    {
                        var couponObj = JsonConvert.DeserializeObject<CouponDto>(Convert.ToString(couponRes.Result));
                        cartDto.CartHeaderDto.DiscountTotal = couponObj.Discount;
                    }
                }

                foreach (var detail in cartDto.CartDetailDtos)
                {
                    cartDto.CartHeaderDto.OrderTotal += (detail.ProductDto.Price * detail.Count);
                }
                cartDto.CartHeaderDto.OrderTotal -= cartDto.CartHeaderDto.DiscountTotal;
            }
            return cartDto;
        }
    }
}