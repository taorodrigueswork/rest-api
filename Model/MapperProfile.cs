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
        //AllowNullCollections = true;
        #region Request DTO to Entity
        // Map from DTO List of Int to Entity Id List
        CreateMap<PersonEntity, int>().ConvertUsing(source => source.Id);
        CreateMap<PersonEntity, PersonDTO>().ReverseMap()
            .ForMember(source => source.Id, opts => opts.Ignore())
            .ForMember(source => source.Days, opts => opts.Ignore());

        CreateMap<DayEntity, DayDTO>().ReverseMap()
            .ForMember(source => source.Id, opts => opts.Ignore())
            .ForMember(source => source.Schedule, opts => opts.Ignore());

        CreateMap<ScheduleEntity, ScheduleDTO>().ReverseMap()
            .ForMember(source => source.Id, opts => opts.Ignore())
            .ForMember(source => source.Created, opts => opts.Ignore());
        #endregion
    }
}