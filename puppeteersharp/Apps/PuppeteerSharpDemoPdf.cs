using PuppeteerSharp;
using PuppeteerSharp.Media;

namespace PuppeteerSharpDemo.Apps
{
    [App(icon: Icons.FileText)]
    public class PuppeteerSharpDemoPdf : ViewBase
    {
        private IState<string> url = null!;
        private IState<string> message = null!;
        private IState<bool> isLoading = null!;

        public override object? Build()
        {
            url = this.UseState("");
            message = this.UseState("No PDF generated yet.");
            isLoading = this.UseState(false);

            return new Card()
                .Title("Website PDF Generator")
                .Description("Generate a PDF from a website using PuppeteerSharp.")
            | Layout.Vertical(
                RenderUrlInput(),
                RenderGenerateButton(),
                new Separator(),
                RenderStatus()
            );
        }

        private IView RenderUrlInput() =>
            Layout.Vertical(
                Text.Muted("Website URL"),
                url.ToTextInput(placeholder: "https://example.com").Width(Size.Full())
            );

        private IWidget RenderGenerateButton() =>
            new Button("Generate PDF", async _ => await GeneratePdf())
                .Icon(Icons.FileText)
                .Variant(ButtonVariant.Primary)
                .Width(Size.Full());

        private IView RenderStatus() =>
            isLoading.Value
                ? Text.Muted("Generating PDF...")
                : Text.Muted(message.Value);

        private async Task GeneratePdf()
        {
            var target = (url.Value ?? "").Trim();
            if (string.IsNullOrEmpty(target))
            {
                message.Set("Please enter a valid URL.");
                return;
            }

            isLoading.Set(true);

            try
            {
                await new BrowserFetcher().DownloadAsync();
                using var browser = await Puppeteer.LaunchAsync(new LaunchOptions { Headless = true });
                using var page = await browser.NewPageAsync();
                await page.GoToAsync(target);

                var folder = System.IO.Path.Combine(AppContext.BaseDirectory, "Assets", "pdfs");
                Directory.CreateDirectory(folder);

                var fileName = $"page-{Guid.NewGuid():N}.pdf";
                var path = System.IO.Path.Combine(folder, fileName);

                await page.PdfAsync(path, new PdfOptions
                {
                    Format = PaperFormat.A4,
                    PrintBackground = true
                });

                message.Set($"PDF saved as: {path}");
            }
            catch (Exception ex)
            {
                message.Set($"Error: {ex.Message}");
            }
            finally
            {
                isLoading.Set(false);
            }
        }
    }
}
