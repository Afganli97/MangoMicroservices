using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mango.AzureBus;
using Mango.Services.ShopingCartAPI.Messages;
using Mango.Services.ShopingCartAPI.Models.DTOs;
using Mango.Services.ShopingCartAPI.Repository;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.ShopingCartAPI.Controllers
{
    [ApiController]
    [Route("api/cart")]
    public class CartController : ControllerBase
    {
        private readonly ICartRepository _cartRepository;
        private readonly ICouponRepository _couponRepository;
        private readonly IMessageBus _messageBus;
        protected ResponseDto _response;

        public CartController(IMessageBus messageBus, ICouponRepository couponRepository, ICartRepository cartRepository)
        {
            _response = new();
            _messageBus = messageBus;
            _couponRepository = couponRepository;
            _cartRepository = cartRepository;
        }

        [HttpGet("GetCart/{userId}")]
        public async Task<object> GetCart(string userId)
        {
            try
            {
                CartDto cartDto = await _cartRepository.GetCartByUserId(userId);
                _response.Result = cartDto;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>(){ ex.ToString() };
            }
            return _response; 
        }

        [HttpPost ("AddCart")]
        public async Task<object> AddCart(CartDto newCartDto)
        {
            try
            {
                CartDto cartDto = await _cartRepository.CreateUpdateCartDto(newCartDto);
                _response.Result = cartDto;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>(){ ex.ToString() };
            }
            return _response; 
        }

        [HttpPost("UpdateCart")]
        public async Task<object> UpdateCart (CartDto newCartDto)
        {
            try
            {
                CartDto cartDto = await _cartRepository.CreateUpdateCartDto(newCartDto);
                _response.Result = cartDto;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>(){ ex.ToString() };
            }
            return _response; 
        }

        [HttpPost("RemoveCart")]
        public async Task<object> RemoveCart ([FromBody]int cartId)
        {
            try
            {
                bool isSuccess = await _cartRepository.RemoveFromCart(cartId);
                _response.Result = isSuccess;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>(){ ex.ToString() };
            }
            return _response; 
        }

        [HttpGet("ClearCart/{userId}")]
        public async Task<object> ClearCart (string userId)
        {
            try
            {
                bool isSuccess = await _cartRepository.ClearCart(userId);
                _response.Result = isSuccess;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>(){ ex.ToString() };
            }
            return _response; 
        }

        [HttpPost("ApplyCoupon")]
        public async Task<object> ApplyCoupon([FromBody] CartDto cartDto)
        {
            try
            {
                bool isSuccess = await _cartRepository.ApplyCoupon(cartDto.CartHeaderDto.UserId, cartDto.CartHeaderDto.CouponCode);
                _response.Result = isSuccess;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>(){ ex.ToString() };
            }
            return _response; 
        }

        [HttpPost("RemoveCoupon")]
        public async Task<object> RemoveCoupon([FromBody] string userId)
        {
            try
            {
                bool isSuccess = await _cartRepository.RemoveCoupon(userId);
                _response.Result = isSuccess;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>(){ ex.ToString() };
            }
            return _response; 
        }

        [HttpPost("Checkout")]
        public async Task<object> Checkout(CheckoutHeaderDto checkoutHeader)
        {
            try
            {
                CartDto cartDto = await _cartRepository.GetCartByUserId(checkoutHeader.UserId);

                if (cartDto == null) return BadRequest();

                if (!string.IsNullOrEmpty(checkoutHeader.CouponCode))
                {
                    CouponDto couponDto = await _couponRepository.GetCoupon(checkoutHeader.CouponCode);
                    if (checkoutHeader.DiscountTotal != couponDto.Discount)
                    {   
                        _response.IsSuccess = false;
                        _response.ErrorMessages = new List<string>(){"Coupon Price has changed, please confirm"};
                        _response.DisplayMessage = "Coupon Price has changed, please confirm";
                        return _response;
                    }
                }

                checkoutHeader.CartDetails = cartDto.CartDetailDtos;

                await _messageBus.PublishMessage(checkoutHeader, Mango.Services.ShopingCartAPI.Helpers.SD.BaseTopic);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>(){ ex.ToString() };
            }
            return _response; 
        }
    }
}