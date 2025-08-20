using AutoMapper;
using EstabraqTourismAPI.Models;
using EstabraqTourismAPI.DTOs;

namespace EstabraqTourismAPI.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Basic mappings that exist
        CreateMap<User, object>();
        CreateMap<Category, object>();
        CreateMap<Trip, object>();
        CreateMap<Booking, object>();
        CreateMap<ContactMessage, object>();
        
        // Reverse mappings
        CreateMap<object, User>();
        CreateMap<object, Category>();
        CreateMap<object, Trip>();
        CreateMap<object, Booking>();
        CreateMap<object, ContactMessage>();
    }
}
