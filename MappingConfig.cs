using AutoMapper;
using BatDongSan_api.Models;
using BatDongSan_api.Models.DTO;

namespace BatDongSan_api
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            CreateMap<Property, PropertyCreateDTO>().ReverseMap();
            CreateMap<Property, PropertyUpdateDTO>().ReverseMap();
        }
    }
}
