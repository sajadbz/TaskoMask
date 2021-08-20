﻿using AutoMapper;
using TaskoMask.Application.Core.Dtos.Users;
using TaskoMask.Domain.Entities;
using TaskoMask.Application.Mapper.MappingActions;

namespace TaskoMask.Application.Mapper.Profiles
{
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile()
        {
            CreateMap<User, UserBasicInfoDto>()
              .AfterMap<UserMappingAction>();

            CreateMap<User, UserInputDto>()
                .AfterMap<UserMappingAction>();

            CreateMap<UserBasicInfoDto, UserInputDto>();
        }
    }
}
