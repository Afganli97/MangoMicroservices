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
        public async Task<bool> ClearCart(string userId)
        {
            var cartHeaderFromDb = await _context.CartHeaders.FirstOrDefaultAsync(x=>x.UserId == userId);
            if (cartHeaderFromDb != null)
            {
                _context.CartDetails.RemoveRange(_context.CartDetails.Where(x=>x.CartHeaderId == cartHeaderFromDb.Id));
                _context.CartHeaders.Remove(cartHeaderFromDb);
                await  _context.SaveChangesAsync(); 
                return true;
            }
            return false;
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

            var cartHeaderFromDb = await _context.CartHeaders.AsNoTracking().FirstOrDefaultAsync(c => c.UserId == cart.CartHeader.UserId);

            if (cartHeaderFromDb == null)
            {
                _context.CartHeaders.Add(cart.CartHeader);
                await _context.SaveChangesAsync();
                cart.CartDetails.FirstOrDefault().CartHeaderId = cart.CartHeader.Id;
                cart.CartDetails.FirstOrDefault().Product = null;
                _context.CartDetails.Add(cart.CartDetails.FirstOrDefault());
                await _context.SaveChangesAsync();
            }
            else
            {
                var cartDetailsFromDb = await _context.CartDetails.AsNoTracking().FirstOrDefaultAsync(
                    c => c.ProductId == cart.CartDetails.FirstOrDefault().ProductId && 
                    c.CartHeaderId == cartHeaderFromDb.Id);

                if (cartDetailsFromDb == null)
                {
                    cart.CartDetails.FirstOrDefault().CartHeaderId = cartHeaderFromDb.Id;
                    cart.CartDetails.FirstOrDefault().Product = null;
                    _context.CartDetails.Add(cart.CartDetails.FirstOrDefault());
                    await _context.SaveChangesAsync();
                }
                else
                {
                    cart.CartDetails.FirstOrDefault().Product = null;
                    cart.CartDetails.FirstOrDefault().Count += cartDetailsFromDb.Count;
                    _context.CartDetails.Update(cart.CartDetails.FirstOrDefault());
                    await _context.SaveChangesAsync(); 
                }
            }
            return _mapper.Map<CartDto>(cart);
        }

        public async Task<CartDto> GetCartByUserId(string userId)
        {
            Cart cart = new()
            {
                CartHeader = await _context.CartHeaders.FirstOrDefaultAsync(x=>x.UserId == userId)
            };
            cart.CartDetails = _context.CartDetails.Where(c => c.CartHeaderId == cart.CartHeader.Id).Include(x => x.Product);

            return _mapper.Map<CartDto>(cart);
        }

        public async Task<bool> RemoveFromCart(int cartDetailId)
        {
            try
            {
                var cartDetail = await _context.CartDetails.FindAsync(cartDetailId);

                int totalCountCartItems = _context.CartDetails.Where(x => x.CartHeaderId  == cartDetail.CartHeaderId).Count();

                _context.CartDetails.Remove(cartDetail);

                if (totalCountCartItems == 1)
                {
                    var cartHeaderToRemove = await _context.CartHeaders.FindAsync(cartDetail.CartHeaderId);
                    _context.CartHeaders.Remove(cartHeaderToRemove);
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}