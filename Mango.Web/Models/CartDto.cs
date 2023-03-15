using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mango.Web.Models
{
    public class CartDto
    {
        public CartHeaderDto CartHeaderDto { get; set; }
        public IEnumerable<CartDetailDto> CartDetailDtos { get; set; }
    }
}