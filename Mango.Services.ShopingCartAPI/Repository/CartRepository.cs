using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Mango.Services.ShopingCartAPI.DbContexts;
using Mango.Services.ShopingCartAPI.Models;
using Mango.Services.ShopingCartAPI.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.ShopingCartAPI.Repository
{
    public class CartRepository : ICartRepository
    {
        private readonly AppDbContext _context;
        private IMapper _mapper;

        public CartRepository(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<bool> ClearCart(int userId)
        {
            throw new NotImplementedException();
        }

        public async Task<CartDto> CreateUpdateCartDto(CartDto cartDto)
        {
            Cart cart = _mapper.Map<Cart>(cartDto);

            var productInDb = await _context.Products.FirstOrDefaultAsync(p => p.Id ==cartDto.CartDetailDtos.FirstOrDefault().ProductId);
            if (productInDb == null)
            {
                _context.Add(cart.CartDetails.FirstOrDefault().Product);
                await _context.SaveChangesAsync();
            }

            var cartHeaderFromDb = await _context.CartHeaders.FirstOrDefaultAsync(c => c.UserId == cart.CartHeader.UserId);

            if (cartHeaderFromDb == null)
            {
                _context.CartHeaders.Add(cart.CartHeader);
                await _context.SaveChangesAsync();
                cart.CartDetails.FirstOrDefault().CartHeaderId = cart.CartHeader.Id;
                // cart.CartDetails.FirstOrDefault().Product = null;
                _context.CartDetails.Add(cart.CartDetails.FirstOrDefault());
                await _context.SaveChangesAsync();
            }
            else
            {
                var cartDetailsFromDb = await _context.CartDetails.FirstOrDefaultAsync(
                    c => c.ProductId == cart.CartDetails.FirstOrDefault().ProductId && 
                    c.CartHeaderId == cartHeaderFromDb.Id);
            }
        }

        public async Task<CartDto> GetCartByUserId(string userId)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> RemoveFromCart(int cartDetailId)
        {
            throw new NotImplementedException();
        }
    }
}