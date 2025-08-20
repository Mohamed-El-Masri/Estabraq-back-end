using AutoMapper;
using EstabraqTourismAPI.Models;
using EstabraqTourismAPI.DTOs.Trip;

namespace EstabraqTourismAPI.Helpers;

public class TripMappingProfile : Profile
{
    public TripMappingProfile()
    {
        // Trip mappings
        CreateMap<Trip, TripDto>()
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
            .ForMember(dest => dest.CategoryNameAr, opt => opt.MapFrom(src => src.Category.NameAr))
            .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.Images.OrderBy(i => i.SortOrder)))
            .ForMember(dest => dest.Schedule, opt => opt.MapFrom(src => src.Schedule.OrderBy(s => s.SortOrder)))
            .ForMember(dest => dest.IncludedItems, opt => opt.MapFrom(src => src.IncludedItems.OrderBy(i => i.SortOrder)))
            .ForMember(dest => dest.BookingsCount, opt => opt.MapFrom(src => src.Bookings.Count));
            
        CreateMap<Trip, TripSummaryDto>()
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
            .ForMember(dest => dest.CategoryNameAr, opt => opt.MapFrom(src => src.Category.NameAr));
            
        CreateMap<CreateTripRequestDto, Trip>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.MainImage, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Category, opt => opt.Ignore())
            .ForMember(dest => dest.Images, opt => opt.Ignore())
            .ForMember(dest => dest.Bookings, opt => opt.Ignore())
            .AfterMap((src, dest) =>
            {
                // Map schedule items
                foreach (var scheduleDto in src.Schedule)
                {
                    var schedule = new TripSchedule
                    {
                        DayNumber = scheduleDto.DayNumber,
                        Title = scheduleDto.Title,
                        TitleAr = scheduleDto.TitleAr,
                        Description = scheduleDto.Description,
                        DescriptionAr = scheduleDto.DescriptionAr,
                        SortOrder = scheduleDto.SortOrder,
                        TripId = dest.Id
                    };
                    dest.Schedule.Add(schedule);
                }
                
                // Map included items
                foreach (var includedDto in src.IncludedItems)
                {
                    var included = new TripIncluded
                    {
                        Item = includedDto.Item,
                        ItemAr = includedDto.ItemAr,
                        SortOrder = includedDto.SortOrder,
                        TripId = dest.Id
                    };
                    dest.IncludedItems.Add(included);
                }
            });
            
        CreateMap<UpdateTripRequestDto, Trip>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.MainImage, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Category, opt => opt.Ignore())
            .ForMember(dest => dest.Images, opt => opt.Ignore())
            .ForMember(dest => dest.Schedule, opt => opt.Ignore())
            .ForMember(dest => dest.IncludedItems, opt => opt.Ignore())
            .ForMember(dest => dest.Bookings, opt => opt.Ignore());
            
        // Trip Image mappings
        CreateMap<TripImage, TripImageDto>();
        CreateMap<CreateTripImageRequestDto, TripImage>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.ImageUrl, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.TripId, opt => opt.Ignore())
            .ForMember(dest => dest.Trip, opt => opt.Ignore());
            
        // Trip Schedule mappings
        CreateMap<TripSchedule, TripScheduleDto>();
        CreateMap<CreateTripScheduleRequestDto, TripSchedule>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.ImageUrl, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.TripId, opt => opt.Ignore())
            .ForMember(dest => dest.Trip, opt => opt.Ignore());
            
        // Trip Included mappings
        CreateMap<TripIncluded, TripIncludedDto>();
        CreateMap<CreateTripIncludedRequestDto, TripIncluded>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Icon, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.TripId, opt => opt.Ignore())
            .ForMember(dest => dest.Trip, opt => opt.Ignore());
    }
}
