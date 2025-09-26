using Ivy;
using Ivy.Shared;     // for Colors.*
using Cysharp.Text;

namespace IvyExamples.ZStringDemo.Apps;

[App(icon: Icons.PartyPopper, title: "ZString Demo")]
public class HelloApp : ViewBase
{
    public override object Build()   
    {
        var concatState   = this.UseState<string>();
        var builderState  = this.UseState<string>();
        var preparedState = this.UseState<string>();

        return
            Layout.Center()
            | (
                Layout.Vertical().Gap(10).Padding(2)
                | Text.H2("ZString Demo in Ivy")

                // Concat/Format/Join
                | new Button("Run Concat/Format/Join", () =>
                {
                    var s = ZString.Concat("Hello ", "Ivy ", 2025);
                    s += " | " + ZString.Format("Pi is {0:0.00}", 3.14159);
                    s += " | " + ZString.Join(", ", new[] { "A", "B", "C" });
                    concatState.Value = s;
                })
                | Text.Block(concatState.Value ?? string.Empty).Color(Colors.Green)

                // StringBuilder
                | new Button("Run StringBuilder", () =>
                {
                    using var sb = ZString.CreateStringBuilder();
                    sb.Append("foo");
                    sb.AppendLine(42);
                    sb.AppendFormat("{0} {1:.###}", "bar", 123.456789);
                    builderState.Value = sb.ToString();
                })
                | Text.Block(builderState.Value ?? string.Empty).Color(Colors.Purple)

                // Prepared Format
                | new Button("Run Prepared Format", () =>
                {
                    var prepared = ZString.PrepareUtf16<int, int>("x:{0}, y:{1:000}");
                    preparedState.Value = prepared.Format(10, 20);
                })
                | Text.Block(preparedState.Value ?? string.Empty).Color(Colors.Blue)
              );
    }
}
