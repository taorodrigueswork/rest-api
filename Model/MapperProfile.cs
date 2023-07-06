using AutoMapper;

using Entities.DTO.Request.Day;
using Entities.DTO.Request.Person;
using Entities.DTO.Request.Schedule;
using Entities.Entity;

namespace Entities;
public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        #region Request DTO to Entity

        // To Map from DTO List of Int to Entity List we need t o create a mapper between int and PersonEntity
        CreateMap<int, PersonEntity>()
            .ForMember(destination => destination.Id, destination => destination.MapFrom(src => src))
            .ForMember(source => source.Email, destination => destination.Ignore())
            .ForMember(source => source.Name, destination => destination.Ignore())
            .ForMember(source => source.Days, destination => destination.Ignore())
            .ForMember(source => source.Phone, destination => destination.Ignore());

        CreateMap<PersonEntity, PersonDto>().ReverseMap()
            .ForMember(source => source.Id, destination => destination.Ignore())
            .ForMember(source => source.Days, destination => destination.Ignore());

        CreateMap<ScheduleEntity, ScheduleDto>().ReverseMap()
            .ForMember(source => source.Id, destination => destination.Ignore())
            .ForMember(source => source.Name, destination => destination.MapFrom(src => src.Name))
            .ForMember(source => source.Created, destination => destination.Ignore());

        CreateMap<DayEntity, DayDto>().ReverseMap()
            .ForMember(source => source.Id, destination => destination.Ignore())
            .ForPath(source => source.Schedule.Id, destination => destination.MapFrom(src => src.ScheduleId));

        #endregion
    }
}