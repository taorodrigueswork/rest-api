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

    public async Task<PersonEntity> Add(PersonDTO personDTO)
    {
        var person = _mapper.Map<PersonEntity>(personDTO);

        await _repository.InsertAsync(person);

        _logger.LogInformation($"Added person with name {person.Name} and ID {person.Id}");

        return person;
    }

    public async Task<PersonEntity> Delete(int id)
    {
        var person = await _repository.FindByIdAsync(id);

        if (person != null)
        {
            _logger.LogInformation($"Deleted person.", person);
            await _repository.DeleteAsync(person);
        }

        _logger.LogWarning($"The person with id {id} was not found.");
        return person;
    }

    public async Task<PersonEntity> GetById(int id)
    {
        return await _repository.FindByIdAsync(id);
    }

    public async Task<PersonEntity> Update(int personId, PersonDTO personDTO)
    {
        var person = await _repository.FindByIdAsync(personId);

        if (person == null)
        {
            _logger.LogWarning($"The person with id {personId} was not found.");
            return person;
        }

        // Update person properties from DTO
        person = _mapper.Map<PersonEntity>(personDTO);
        person.Id = personId;

        await _repository.UpdateAsync(person);

        _logger.LogInformation($"Updated person with name {person.Name} and ID {person.Id}", person);

        return person;
    }
}
