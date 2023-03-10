using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mango.Service.Identity.DbContexts;
using Mango.Service.Identity.Helpers;
using Mango.Service.Identity.Models;
using Microsoft.AspNetCore.Identity;

namespace Mango.Service.Identity.Initializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DbInitializer(AppDbContext context, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public void Initialize()
        {
            if (_roleManager.FindByNameAsync(SD.Admin).Result == null)
            {
                _roleManager.CreateAsync(new IdentityRole(SD.Admin)).GetAwaiter().GetResult();
            }
        }
    }
}