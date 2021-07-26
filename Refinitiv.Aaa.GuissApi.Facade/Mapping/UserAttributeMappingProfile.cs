using Refinitiv.Aaa.GuissApi.Data.Models;
using Refinitiv.Aaa.GuissApi.Interfaces.Models.UserAttribute;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Refinitiv.Aaa.GuissApi.Facade.Mapping
{
    [ExcludeFromCodeCoverage]
    class UserAttributeMappingProfile : AutoMapper.Profile
    {
        public UserAttributeMappingProfile()
        {
            CreateMap<UserAttributeDb, UserAttribute>()
            .ReverseMap()
            .ForMember(m => m.SearchName, opt => opt.MapFrom(obj => obj.Name.ToLower()));

            CreateMap<UserAttributeDetails, UserAttribute>()
            .ReverseMap();
        }
    }
}
