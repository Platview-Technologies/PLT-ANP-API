using AutoMapper;
using Entities.Models;
using Entities.SystemModel;
using Microsoft.AspNetCore.Routing.Constraints;
using Shared.DTOs;
using Shared.DTOs.Request;
using Shared.DTOs.Response;
using Utilities.Utilities;

namespace PLT_ANP_API.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<DealRequestDto, DealsModel>()
                .ForMember(dest => dest.ContactEmail, opt => opt.MapFrom(src => Helper.ConvertToString(src.ContactEmail)))
                .ForMember(dest => dest.CCEmails, opt => opt.MapFrom(src => src.CCEmails != null ? Helper.ConvertToString(src.CCEmails) : null));
            CreateMap<DealUpdateDto, DealsModel>()
                .ForMember(dest => dest.ContactEmail, opt => opt.MapFrom(src => Helper.ConvertToString(src.ContactEmail)))
                .ForMember(dest => dest.CCEmails, opt => opt.MapFrom(src => src.CCEmails != null ? Helper.ConvertToString(src.CCEmails) : null));
            CreateMap<DealsModel, DealResponseDto>()
                .ForMember(dest => dest.ContactEmail, opt => opt.MapFrom(src => Helper.ConvertToList(src.ContactEmail)))
                .ForMember(dest => dest.CCEmails, opt => opt.MapFrom(src => src.CCEmails != null ? Helper.ConvertToList(src.CCEmails) : null));
            CreateMap<DealsModel, DealRenewalResponseDto>()
                .ForMember(dest => dest.ContactEmail, opt => opt.MapFrom(src => Helper.ConvertToList(src.ContactEmail)))
                .ForMember(dest => dest.CCEmails, opt => opt.MapFrom(src => src.CCEmails != null ? Helper.ConvertToList(src.CCEmails) : null));
            CreateMap<UserAdminRegistrationDto, UserModel>();
            CreateMap<TempUserModel, UserToReturnDto>();
            CreateMap<NormalUserRegistrationDto, UserModel>();
            CreateMap<UserModel, GetUserResponseDto>();
            CreateMap<TempUserModel, UserToReturnUserDto>();
            CreateMap<DealsModel, PaginatedDealResponseDto>()
                .ForMember(dest => dest.ContactEmail, opt => opt.MapFrom(src => Helper.ConvertToList(src.ContactEmail)))
                .ForMember(dest => dest.CCEmails, opt => opt.MapFrom(src => src.CCEmails != null ? Helper.ConvertToList(src.CCEmails) : null));
            CreateMap<NotificationModel, NotificationResponseDto>()
                .ForMember(dest => dest.EmailAddresses, opt => opt.MapFrom(src => Helper.ConvertToList(src.Emailaddresses)))
                .ForMember(dest => dest.CCEmails, opt => opt.MapFrom(src => src.CCEmails != null ? Helper.ConvertToList(src.CCEmails) : null));
            CreateMap<NotificationModel, NotificationDto>()
                .ForMember(dest => dest.EmailAddresses, opt => opt.MapFrom(src => Helper.ConvertToList(src.Emailaddresses)))
                .ForMember(dest => dest.CCEmails, opt => opt.MapFrom(src => src.CCEmails != null ? Helper.ConvertToList(src.CCEmails) : null));
            CreateMap<RenewalsModel, RenewalResponseDto>();
            CreateMap<RenewalsModel, RenewalDealResponseDto>();
            CreateMap<RenewalRequestDto, RenewalsModel>()
                .AfterMap((src, dest, context) =>
                {
                    if (context.Items["deal"] is DealsModel deal)
                    {
                        dest.ExpectedRenewalDate = deal.RenewalDate;
                        dest.PrevCommencementDate = deal.CommencementDate;
                        dest.PrevExpiryDate = deal.ExpiryDate;
                        dest.ValueBeforeRenewal = deal.Value;
                        // Map other properties as needed
                    }
                });
        }
    }
}
