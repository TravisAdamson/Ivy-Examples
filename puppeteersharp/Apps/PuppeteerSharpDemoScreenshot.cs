using System.Buffers.Text;

namespace PuppeteerSharpDemo
{
    [App(icon: Icons.Image)]
    public class PuppeteerSharpDemoScreenshot : ViewBase
    {
        private IState<string> url = null!;
        private IState<string?> screenshotPath = null!;
        private IState<bool> isLoading = null!;

        public override object? Build()
        {
            // initialize states
            url = this.UseState("");
            screenshotPath = this.UseState<string?>();
            isLoading = this.UseState(false);


            return new Card()
                .Title("Website Screenshot")
                .Description("Enter a URL and capture a screenshot using PuppeteerSharp.")
                | Layout.Vertical(
                RenderUrlInput(),
                RenderCaptureButton(),
                RenderStatus()
            );
        }

        private IView RenderUrlInput() =>
            Layout.Vertical(
                Text.Muted("Website URL"),
                url.ToTextInput(placeholder: "https://example.com").Width(Size.Full())
            );

        private IWidget RenderCaptureButton()
        {
            var client = this.UseService<IClientProvider>();

            return new Button("Capture Screenshot", async _ =>
            {
                await CaptureScreenshot();
                client.OpenUrl(screenshotPath.Value);
            })
            .Icon(Icons.Camera)
            .Variant(ButtonVariant.Primary)
            .Width(Size.Full());
        }

        private IView RenderStatus() =>
            isLoading.Value
                ? Text.Muted("Capturing screenshot...")
                : Text.Muted("");


        // --- Action ---

        private async Task CaptureScreenshot()
        {
            var inputUrl = (url.Value ?? "").Trim();
            if (string.IsNullOrEmpty(inputUrl))
                return;

            isLoading.Set(true);

            try
            {
                using var browser = await Puppeteer.LaunchAsync(new LaunchOptions { Headless = true });
                using var page = await browser.NewPageAsync();
                await page.GoToAsync(inputUrl);

                var folder = System.IO.Path.Combine(AppContext.BaseDirectory, "Assets", "screenshots");
                Directory.CreateDirectory(folder);
                var fileName = $"screenshot-{Guid.NewGuid().ToString("N")}.png";
                var path = System.IO.Path.Combine(folder, fileName);

                var bytes = await page.ScreenshotDataAsync(new ScreenshotOptions { FullPage = true });
                File.WriteAllBytes(path, bytes);   // <-- actually writes the file

                var downloadUrl = $"http://localhost:5010/assets/screenshots/{fileName}?download=1";
                screenshotPath.Set(downloadUrl);
                

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                screenshotPath.Set("");
            }
            finally
            {
                isLoading.Set(false);
            }
        }
    }
}

