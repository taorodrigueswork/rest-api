using Entities.DTO.Request.Person;
using Microsoft.Extensions.Logging;
using Persistence.Interfaces;

namespace Business;
public class PersonBusiness : IBusiness<PersonDto, PersonEntity>
{
    private readonly IMapper _mapper;
    private readonly ILogger<PersonBusiness> _logger;
    private readonly IPersonRepository _personRepository;

    public PersonBusiness(IMapper mapper, ILogger<PersonBusiness> logger, IPersonRepository personRepository)
    {
        _mapper = mapper;
        _logger = logger;
        _personRepository = personRepository;
    }

    public async Task<PersonEntity> Add(PersonDto personDto)
    {
        var personEntity = _mapper.Map<PersonEntity>(personDto);
        var person = await _personRepository.InsertAsync(personEntity);

        _logger.LogInformation($"Added person {person}");

        return person;
    }

    public async Task Delete(int id)
    {
        var person = await _personRepository.FindByIdAsync(id);

        ArgumentNullException.ThrowIfNull(person, $"The person with id {id} was not found.");

        _logger.LogInformation($"Deleted person.", person);
        await _personRepository.DeleteAsync(person);
    }

    public async Task<PersonEntity?> GetById(int id)
    {
        return await _personRepository.FindByIdAsync(id);
    }

    public async Task<PersonEntity?> Update(int id, PersonDto personDTO)
    {
        var person = await _personRepository.FindByIdAsync(id);

        ArgumentNullException.ThrowIfNull(person, $"The person with id {id} was not found.");

        // Update person properties from DTO
        person = _mapper.Map<PersonEntity>(personDTO);
        person.Id = id;

        await _personRepository.UpdateAsync(person);

        _logger.LogInformation($"Updated person with name {person.Name} and ID {person.Id}", person);

        return person;
    }
}
