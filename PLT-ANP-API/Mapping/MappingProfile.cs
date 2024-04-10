using AutoMapper;
using Entities.Models;
using Entities.SystemModel;
using Shared.DTOs.Request;
using Shared.DTOs.Response;

namespace PLT_ANP_API.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<DealRequestDto, DealsModel>();
            CreateMap<DealUpdateDto, DealsModel>();
            CreateMap<DealsModel, DealResponseDto>();
            CreateMap<UserAdminRegistrationDto, UserModel>();
            CreateMap<TempUserModel, UserToReturnDto>();
            CreateMap<NormalUserRegistrationDto, UserModel>();
            CreateMap<UserModel, GetUserResponseDto>();
            CreateMap<TempUserModel, UserToReturnUserDto>();
            CreateMap<DealsModel, PaginatedDealResponseDto>();
            CreateMap<NotificationModel, NotificationResponseDto>();
        }
    }
}
