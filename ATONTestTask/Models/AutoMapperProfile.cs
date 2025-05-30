﻿using ATONTestTask.Abstractions;
using ATONTestTask.Data.Entites;
using ATONTestTask.ViewModels.Request;
using ATONTestTask.ViewModels.Resposne;
using AutoMapper;

namespace ATONTestTask.Models
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, UserDto>();
            CreateMap<User, UserExtendedDto>();

            CreateMap<CreateUsereDto, User>();
        }
    }
}
