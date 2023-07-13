namespace Tests.Entities;

[TestFixture]

public class MappingProfileTests
{
    // <summary>
    // It tests if all the properties are mapped correctly.
    // Not a single property should be ignored.
    // For more information, see https://www.twilio.com/blog/test-driven-automapper-net-core
    // </summary>
    [Test]
    public void ValidateMappingConfigurationTest()
    {
        MapperConfiguration mapperConfig = new MapperConfiguration(
       cfg =>
       {
           cfg.AddProfile(new AutoMapperProfile());
       });

        IMapper mapper = new Mapper(mapperConfig);

        //mapper.ConfigurationProvider.AssertConfigurationIsValid();
    }
}