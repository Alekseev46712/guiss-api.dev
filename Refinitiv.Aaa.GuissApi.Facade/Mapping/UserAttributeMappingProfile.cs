using Refinitiv.Aaa.GuissApi.Data.Models;
using Refinitiv.Aaa.GuissApi.Interfaces.Models.UserAttribute;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
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
            .ForMember(dest => dest.SearchName, opt => opt.MapFrom(
                src => string.IsNullOrEmpty(src.Name)
                ? string.Empty
                : src.Name.ToLower(CultureInfo.CurrentCulture)));

            CreateMap<UserAttributeDetails, UserAttribute>()
            .ReverseMap();
        }
    }
}
