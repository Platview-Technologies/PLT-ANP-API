﻿using AutoMapper;
using Entities.Models;
using Entities.SystemModel;
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
            CreateMap<UserAdminRegistrationDto, UserModel>();
            CreateMap<TempUserModel, UserToReturnDto>();
            CreateMap<NormalUserRegistrationDto, UserModel>();
            CreateMap<UserModel, GetUserResponseDto>();
            CreateMap<TempUserModel, UserToReturnUserDto>();
            CreateMap<DealsModel, PaginatedDealResponseDto>()
                .ForMember(dest => dest.ContactEmail, opt => opt.MapFrom(src => Helper.ConvertToList(src.ContactEmail)))
                .ForMember(dest => dest.CCEmails, opt => opt.MapFrom(src => src.CCEmails != null ? Helper.ConvertToList(src.CCEmails) : null));
            CreateMap<NotificationModel, NotificationResponseDto>();
            CreateMap<NotificationModel, NotificationDto>();
        }
    }
}
