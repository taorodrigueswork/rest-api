using Entities.DTO.Request.Person;

namespace Business;
public class PersonBusiness : IBusiness<PersonDTO, PersonEntity>
{
    private readonly IMapper _mapper;

    public PersonBusiness(IMapper mapper)
    {
        _mapper = mapper;
    }

    public PersonEntity Add(PersonDTO entity)
    {
        var person = _mapper.Map<PersonEntity>(entity);

        return person;
    }

    public void Delete(int id)
    {
        throw new NotImplementedException();
    }


    public PersonEntity Update(PersonDTO entity)
    {
        throw new NotImplementedException();
    }
}
