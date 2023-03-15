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
                config.CreateMap<CartDetail, CartDetailDto>()
                    .ForMember(dest => dest.CartHeaderDto, opt => opt.MapFrom(src => src.CartHeader))
                    .ForMember(dest => dest.ProductDto, opt => opt.MapFrom(src => src.Product)).ReverseMap();
                config.CreateMap<CartHeader, CartHeaderDto>().ReverseMap();
                config.CreateMap<CartDto, Cart>()
                    .ForMember(dest => dest.CartDetails, opt => opt.MapFrom(src => src.CartDetailDtos))
                    .ForMember(dest => dest.CartHeader, opt => opt.MapFrom(src => src.CartHeaderDto)).ReverseMap();
            });
            return mappingConfig;
        } 
    }
}