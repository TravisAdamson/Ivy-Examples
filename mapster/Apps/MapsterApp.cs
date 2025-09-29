using Ivy.Mapster.Models;
using Mapster;

namespace Ivy.Mapster.Apps
{
    [App(icon: Icons.PersonStanding, title: "Mapster Demo")]
    public class MapsterApp : ViewBase
    {
        public override object? Build()
        {
            var personState = this.UseState<Person?>();
            var dtoState = this.UseState<PersonDto?>();

            // Person -> PersonDto
            var toDtoButton = new Button("Person -> PersonDto")
            {
                OnClick = async (evt) =>
                {
                    if (personState.Value == null)
                    {
                        personState.Value = new Person
                        {
                            FirstName = "Jane",
                            LastName = "Doe",
                            Age = 25
                        };
                    }

                    var dto = personState.Value.Adapt<PersonDto>();
                    dtoState.Value = dto;

                    await ValueTask.CompletedTask;
                }
            };

            // PersonDto -> Person
            var toPersonButton = new Button("PersonDto -> Person")
            {
                OnClick = async (evt) =>
                {
                    if (dtoState.Value == null)
                    {
                        dtoState.Value = new PersonDto
                        {
                            FullName = "John Smith",
                            IsAdult = true
                        };
                    }

                    var person = dtoState.Value.Adapt<Person>();
                    personState.Value = person;

                    await ValueTask.CompletedTask;
                }
            };

            return Layout.Center()
               | new Card(
                       Layout.Vertical().Gap(6)
                       | toDtoButton
                       | Text.Block(personState.Value != null
                           ? $"Person: {personState.Value.FirstName} {personState.Value.LastName}, Age: {personState.Value.Age}"
                           : "Person: <empty>")
                       | toPersonButton
                       | Text.Block(dtoState.Value != null
                           ? $"PersonDto: {dtoState.Value.FullName}, IsAdult: {dtoState.Value.IsAdult}"
                           : "PersonDto: <empty>")
                     )
                   .Width(Size.Units(120).Max(500));
        }
    }
}
