using Ivy.Mapster.Models;
using Mapster;

namespace Ivy.Mapster.Mapping
{
    public class MapsterConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Person, PersonDto>()
                .Map(dest => dest.FullName, src => $"{src.FirstName} {src.LastName}")
                .Map(dest => dest.IsAdult, src => src.Age >= 18);
        }
    }
}
