using AutoMapper;
using EstabraqTourismAPI.Models;
using EstabraqTourismAPI.DTOs.Contact;
using EstabraqTourismAPI.DTOs.Content;

namespace EstabraqTourismAPI.Helpers;

public class ContentMappingProfile : Profile
{
    public ContentMappingProfile()
    {
        // Contact Message mappings
        CreateMap<ContactMessage, ContactMessageDto>()
            .ForMember(dest => dest.RepliedByUserName, opt => opt.MapFrom(src => src.RepliedByUser != null ? src.RepliedByUser.Name : null));
            
        CreateMap<CreateContactMessageRequestDto, ContactMessage>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => "New"))
            .ForMember(dest => dest.AdminReply, opt => opt.Ignore())
            .ForMember(dest => dest.RepliedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.RepliedByUserId, opt => opt.Ignore())
            .ForMember(dest => dest.RepliedByUser, opt => opt.Ignore());
            
        // Hero Section mappings
        CreateMap<HeroSection, HeroSectionDto>();
        CreateMap<CreateHeroSectionRequestDto, HeroSection>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.BackgroundImage, opt => opt.Ignore())
            .ForMember(dest => dest.BackgroundVideo, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());
            
        CreateMap<UpdateHeroSectionRequestDto, HeroSection>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.BackgroundImage, opt => opt.Ignore())
            .ForMember(dest => dest.BackgroundVideo, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());
            
        // Site Stats mappings
        CreateMap<SiteStats, SiteStatsDto>();
        CreateMap<CreateSiteStatsRequestDto, SiteStats>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Icon, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());
            
        CreateMap<UpdateSiteStatsRequestDto, SiteStats>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Icon, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());
            
        // Contact Info mappings
        CreateMap<ContactInfo, ContactInfoDto>();
        CreateMap<UpdateContactInfoRequestDto, ContactInfo>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Logo, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());
    }
}
