[App(icon: Icons.PartyPopper, title: "NAudio Demo")]
public class NAudioApp : ViewBase
{
    public override object? Build()
    {
        return Layout.Vertical().Gap(4).Padding(3)
               | new Card(Layout.Vertical().Gap(4).Padding(3)
                     | Text.H2("NAudio Demo")
                     | Text.Muted("NAudio is a library for audio processing.")
               )
                    .Width(Size.Full());
    }
}