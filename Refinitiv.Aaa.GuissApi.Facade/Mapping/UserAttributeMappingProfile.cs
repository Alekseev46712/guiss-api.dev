using Refinitiv.Aaa.GuissApi.Data.Models;
using Refinitiv.Aaa.GuissApi.Interfaces.Models.UserAttribute;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

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
                : src.Name.ToLower(CultureInfo.CurrentCulture)))
            .ForMember(dest => dest.Namespace, opt => opt.MapFrom(src => src.Name == null ? null : src.Name.Split('.')[0]));

            CreateMap<UserAttributeDetails, UserAttribute>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name == null ? null : src.Name.ToLower(CultureInfo.CurrentCulture)))
                .ForMember(dest => dest.UserUuid, opt => opt.MapFrom(src => src.UserUuid == null ? null : src.UserUuid.ToLower(CultureInfo.CurrentCulture)))
            .ReverseMap();
        }
    }
}
