using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mango.Web.Models
{
    public class CouponDto
    {
        public int Id { get; set; }
        public string CouponCode { get; set; }
        public double Discount { get; set; }
    }
}