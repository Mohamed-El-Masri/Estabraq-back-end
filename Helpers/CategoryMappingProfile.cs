using AutoMapper;
using EstabraqTourismAPI.Models;
using EstabraqTourismAPI.DTOs.Category;

namespace EstabraqTourismAPI.Helpers;

public class CategoryMappingProfile : Profile
{
    public CategoryMappingProfile()
    {
        CreateMap<Category, CategoryDto>()
            .ForMember(dest => dest.TripsCount, opt => opt.MapFrom(src => src.Trips.Count));
            
        CreateMap<CreateCategoryRequestDto, Category>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Icon, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Trips, opt => opt.Ignore());
            
        CreateMap<UpdateCategoryRequestDto, Category>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Icon, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Trips, opt => opt.Ignore());
    }
}
