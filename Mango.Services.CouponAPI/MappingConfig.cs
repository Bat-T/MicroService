using AutoMapper;
using Mango.Services.CouponAPI.Models;
using Mango.Services.CouponAPI.Models.DTO;

namespace Mango.Services.CouponAPI
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingconfig = new MapperConfiguration(config =>
            {
                config.CreateMap<Coupon, CouponDTO>();
                config.CreateMap<CouponDTO, Coupon>();
            });
            return mappingconfig;
        }
    }
}
