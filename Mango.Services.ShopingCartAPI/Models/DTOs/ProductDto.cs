using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Mango.Services.ShopingCartAPI.Models.DTOs
{
    public class ProductDto
    {
        public int Id { get; set; }
		public string Name { get; set; }
		public double Price { get; set; }
		public string Description { get; set; }
		public string CategoryName { get; set; }
		public string ImageUrl { get; set; }
    }
}