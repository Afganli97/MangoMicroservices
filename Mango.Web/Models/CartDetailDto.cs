using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mango.Web.Models
{
    public class CartDetailDto
    {
        public int Id { get; set; }
        public int CartHeaderId { get; set; }
        public CartHeaderDto CartHeaderDto { get; set; }
        public int ProductId { get; set; }
        public ProductDto ProductDto { get; set; }
        public int Count { get; set; }
    }
}