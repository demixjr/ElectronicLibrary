using AutoMapper;
using BLL.dto;
using ElectronicLibrary.Models.RequestModels;
using ElectronicLibrary.Models.ResponseModels;

namespace ElectronicLibrary
{
    public class PLMappingProfile : Profile
    {
        public PLMappingProfile() 
        {
            CreateMap<BookRequestModel, BookDto>();
            CreateMap<BookDto, BookResponseModel>();

            CreateMap<OrderDto, OrderResponseModel>();

            CreateMap<UserRequestModel, UserDto>();
            CreateMap<PrivilegedUserRequestModel, UserDto>();
            CreateMap<UserDto, UserResponseModel>();
        }
    }
}