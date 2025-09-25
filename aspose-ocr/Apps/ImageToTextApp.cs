using Ivy.Aspose.OCR.Examples.Connections.OCR;

namespace Ivy.Aspose.OCR.Examples.Apps;

[App(icon: Icons.FileImage, path: ["Apps"])]
public class ImageToTextApp : ViewBase
{
    public override object? Build()
    {
        var outputText = this.UseState<string>("");
        var client = UseService<IClientProvider>();

        var error = UseState<string?>(() => null);
        var ocrService = UseService<IOCRService>();
        var files = UseState<FileInput?>(() => null);

        var uploadUrl = this.UseUpload(
            fileBytes =>
            {
                if (fileBytes.Length > 1 * 1024 * 1024) // 1MB limit
                {
                    client.Toast("File size must be less than 1MB", "Validation Error");
                    error.Set("File size must be less than 1MB");
                    return;
                }

                error.Set((string?)null);
                // Process uploaded file bytes
                Console.WriteLine($"Received {fileBytes.Length} bytes");
            },
            "image/jpeg",
            "uploaded-image"
        );

        return Layout.Vertical(
            Text.H1("Convert Image to text online").Color(Colors.Green),
            Text.Block("Free OCR software to convert images or screenshots to text online"),
            error.Value != null
                ? new Callout(error.Value, variant: CalloutVariant.Error)
                : null,
            files.ToFileInput(uploadUrl, "Upload Image").Accept(".jpg,.jpeg,.png"),
            new Button("Recognize", _ =>
            {
                if (error.Value == null && files.Value != null)
                {
                    var file = files.Value;

                    // TODO: file.Content is getting null or empty
                    using var ms = new MemoryStream(file.Content);
                    outputText.Value = ocrService.ExtractText(ms);

                    Console.WriteLine($"extracted Text: {outputText.Value}");
                }
            }),
            Text.Block("Output Text:"),
            new ObservableView<string>(outputText)
            ).Align(Align.Center);
    }
}
