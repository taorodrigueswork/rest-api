using Entities.DTO.Request.Person;
using Microsoft.Extensions.Logging;
using Persistence.Interfaces;

namespace Business;
public class PersonBusiness : IBusiness<PersonDTO, PersonEntity>
{
    private readonly IMapper _mapper;
    private readonly ILogger<PersonBusiness> _logger;
    private readonly IGenericRepository<PersonEntity> _repository;

    public PersonBusiness(IMapper mapper, ILogger<PersonBusiness> logger, IGenericRepository<PersonEntity> repository)
    {
        _mapper = mapper;
        _logger = logger;
        _repository = repository;
    }

    public PersonEntity Add(PersonDTO entity)
    {
        var person = _mapper.Map<PersonEntity>(entity);

        _logger.LogInformation($"Added person with name {person.Name} and ID {person.Id}");

        return person;
    }

    public void Delete(int id)
    {
        _logger.LogInformation("Log message generated with INFORMATION severity level.");
        _logger.LogWarning("Log message generated with WARNING severity level.");
        _logger.LogError("Log message generated with ERROR severity level.");
        _logger.LogCritical("Log message log generated with CRITICAL severity level.");
    }


    public PersonEntity Update(PersonDTO entity)
    {
        throw new NotImplementedException();
    }
}
