using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mango.Services.ShopingCartAPI.Models
{
    public class Cart
    {
        public CartHeader CartHeader { get; set; }
        public List<CartDetail> CartDetails { get; set; }
    }
}