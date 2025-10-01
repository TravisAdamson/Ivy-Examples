using BarcodeStandard;
using SkiaSharp;
using Type = BarcodeStandard.Type;

namespace BarcodelibExample.Apps
{
    [App(icon: Icons.Barcode, title: "BarcodeLib Demo", path: ["Apps"])]
    public sealed class BarcodeLibApp : ViewBase
    {
        private static readonly (string Label, Type Type)[] Symbologies =
        {
       ("UPC‑A", Type.UpcA),
        ("EAN‑13", Type.Ean13),
        ("Code128", Type.Code128),
        ("Code39", Type.Code39),
        ("Interleaved 2 of 5", Type.Interleaved2Of5),
        ("ITF‑14", Type.Itf14)
    };

        public override object? Build()
        {
            var text = UseState("038000356216");
            var typeIndex = UseState(0);
            var includeLabel = UseState(true);

            // fixed barcode size
            const int width = 300;
            const int height = 120;

            var downloadUrl = this.UseDownload(() =>
            {
                if (string.IsNullOrWhiteSpace(text.Value))
                    return Array.Empty<byte>();

                var (_, type) = Symbologies[typeIndex.Value];
                var b = new Barcode { IncludeLabel = includeLabel.Value };
                using var bitmap = b.Encode(type, text.Value, SKColors.Black, SKColors.White, width, height);
                using var data = bitmap.Encode(SKEncodedImageFormat.Png, 100);
                return data.ToArray();
            }, "image/png", "barcode.png");

            var typeItems = Symbologies
                .Select((item, idx) => MenuItem.Default(item.Label).HandleSelect(() => typeIndex.Value = idx))
                .ToArray();

            var typeDropDown = new Button(Symbologies[typeIndex.Value].Label)
                .Primary()
                .Icon(Icons.ChevronDown)
                .WithDropDown(typeItems);

            return Layout.Center()
                | new Card(
                    Layout.Vertical().Gap(6).Padding(3)
                    | Text.H2("Generate a barcode")
                    | Text.Muted("This demo uses the BarcodeLib NuGet package to generate barcodes.")
                    | text.ToInput(placeholder: "Enter the barcode value …")
                    | typeDropDown
                    | new Button(includeLabel.Value ? "Label: ON" : "Label: OFF")
                        .HandleClick(() => includeLabel.Value = !includeLabel.Value)
                    | Text.Muted("Barcode size is fixed at 300×120 pixels.")
                    | new Button("Download").Primary().Icon(Icons.Download).Url(downloadUrl.Value)
                  ).Width(Size.Units(120).Max(900));
        }
    }
}
