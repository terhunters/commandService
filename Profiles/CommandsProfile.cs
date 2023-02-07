using AutoMapper;
using CommandService.Dtos;
using CommandService.Models;
using WebApplication3;

namespace CommandService.Profiles
{
    public class CommandsProfile: Profile
    {
        public CommandsProfile()
        {
            CreateMap<Platform, PlatformReadDto>();
            CreateMap<Command, CommandReadDto>();
            CreateMap<CommandCreateDto, Command>();
            CreateMap<PlatformPublishedDto, Platform>()
                .ForMember(
                    x => x.ExternalId,
                    opt => opt.MapFrom(x=> x.Id)
                    );
            CreateMap<GrpcPlatformModel, Platform>()
                .ForMember(dest => dest.ExternalId,
                    opt => opt.MapFrom(src => src.PlatformId))
                .ForMember(dest => dest.Name,
                    opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Commands,
                    opt => opt.Ignore());
        }
    }
}