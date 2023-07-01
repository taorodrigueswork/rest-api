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
        CreateMap<int, PersonEntity>().ForMember(dest => dest.Id, opts => opts.MapFrom(src => src));

        CreateMap<PersonEntity, PersonDTO>().ReverseMap()
            .ForMember(source => source.Id, opts => opts.Ignore())
            .ForMember(source => source.Days, opts => opts.Ignore());

        CreateMap<ScheduleEntity, ScheduleDTO>().ReverseMap()
    .ForMember(source => source.Id, opts => opts.Ignore())
    .ForMember(source => source.Name, opts => opts.MapFrom(src => src.Name))
    .ForMember(source => source.Created, opts => opts.Ignore());

        CreateMap<DayEntity, DayDTO>().ReverseMap()
            .ForMember(source => source.Id, opts => opts.Ignore())
            .ForPath(source => source.Schedule.Id, opts => opts.MapFrom(src => src.ScheduleId));


        #endregion
    }
}