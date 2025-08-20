using AutoMapper;
using EstabraqTourismAPI.Models;
using EstabraqTourismAPI.DTOs.Booking;

namespace EstabraqTourismAPI.Helpers;

public class BookingMappingProfile : Profile
{
    public BookingMappingProfile()
    {
        CreateMap<Booking, BookingDto>()
            .ForMember(dest => dest.TripTitle, opt => opt.MapFrom(src => src.Trip.Title))
            .ForMember(dest => dest.TripTitleAr, opt => opt.MapFrom(src => src.Trip.TitleAr))
            .ForMember(dest => dest.TripLocation, opt => opt.MapFrom(src => src.Trip.Location))
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User != null ? src.User.Name : null));
            
        CreateMap<CreateBookingRequestDto, Booking>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.BookingReference, opt => opt.Ignore())
            .ForMember(dest => dest.TotalPrice, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => "Pending"))
            .ForMember(dest => dest.AdminNotes, opt => opt.Ignore())
            .ForMember(dest => dest.BookingDate, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.Trip, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore());
    }
}
