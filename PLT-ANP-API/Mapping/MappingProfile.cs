using AutoMapper;
using Entities.Models;
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
        }
    }
}
