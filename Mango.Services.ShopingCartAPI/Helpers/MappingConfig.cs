using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Mango.Services.ShopingCartAPI.Models;
using Mango.Services.ShopingCartAPI.Models.DTOs;

namespace Mango.Services.ShopingCartAPI.Helpers
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(config =>{
                config.CreateMap<ProductDto, Product>().ReverseMap();
                config.CreateMap<CartDetail, CartDetailDto>().ReverseMap();
                config.CreateMap<CartHeader, CartHeaderDto>().ReverseMap();
                config.CreateMap<Cart, CartDto>().ReverseMap();
            });
            return mappingConfig;
        } 
    }
}