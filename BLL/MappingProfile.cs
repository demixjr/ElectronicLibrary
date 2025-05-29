using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DAL.Models;
using BLL.dto;
namespace BLL
{
    public class MappingProfile: Profile
    {
        public MappingProfile() 
        {
            CreateMap<Book, BookDto>();
            CreateMap<BookDto, Book>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());

            CreateMap<User, UserDto>();
            CreateMap<UserDto, User>()
                .ForMember(dest => dest.Order, opt => opt.Ignore());

            CreateMap<Order, OrderDto>().ReverseMap();
        }

    }
}
