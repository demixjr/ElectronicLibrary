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
            CreateMap<Book, BookDto>().ReverseMap();
            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<Order, OrderDto>().ReverseMap();
        }

    }
}
