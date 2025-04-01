using System;
using API.DTOs;
using API.Entities;
using API.Extensions;
using AutoMapper;

namespace API.Helpers;

//automapper automatically converts AppUser instances into MemberDto instances
public class AutoMapperProfiles : Profile
{
    public AutoMapperProfiles()
    {
        //this defines the mapping between the source and destination types
        //source type is AppUser and destination type is MemberDto
        CreateMap<AppUser, MemberDto>()
            .ForMember(d => d.Age, o => o.MapFrom(s => s.DateOfBirth.CalculateAge()))
            .ForMember(d => d.PhotoUrl, o => o.MapFrom(s => s.Photos.FirstOrDefault(x => x.IsMain)!.Url));
        CreateMap<Photo, PhotoDto>();
    }
}
