using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mango.Services.ShopingCartAPI.Models.DTOs;
using Mango.Services.ShopingCartAPI.Repository;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.ShopingCartAPI.Controllers
{
    [ApiController]
    [Route("api/cart")]
    public class CartController : ControllerBase
    {
         private readonly ICartRepository _repository;
         protected ResponseDto _response;

        public CartController(ICartRepository repository)
        {
            _repository = repository;
            _response = new();
        }

        [HttpGet("GetCart/{userId}")]
        public async Task<object> GetCart(string userId)
        {
            try
            {
                CartDto cartDto = await _repository.GetCartByUserId(userId);
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
                CartDto cartDto = await _repository.CreateUpdateCartDto(newCartDto);
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
                CartDto cartDto = await _repository.CreateUpdateCartDto(newCartDto);
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
                bool isSuccess = await _repository.RemoveFromCart(cartId);
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
                bool isSuccess = await _repository.ClearCart(userId);
                _response.Result = isSuccess;
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