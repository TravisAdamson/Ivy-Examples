using NAudio.Wave;
using NAudio.Wave.SampleProviders;

[App(icon: Icons.PartyPopper, title: "NAudio Demo")]
public class NAudioApp : ViewBase
{
    public override object? Build()
    {
        var fileInputState = UseState<FileInput?>((FileInput?)null);
        var selected = fileInputState.Value?.Name;

        var uploadedAudioBytes = UseState<byte[]?>((byte[]?)null);
        var resultingAudioBytes = UseState<byte[]?>((byte[]?)null);

        var uploadUrl = this.UseUpload(
            fileBytes =>
            {
                Console.WriteLine($"Upload triggered with {fileBytes?.Length ?? 0} bytes");
                try
                {
                    uploadedAudioBytes.Value = fileBytes;
                    resultingAudioBytes.Value = null;
                    Console.WriteLine($"Successfully set uploaded audio bytes: {fileBytes?.Length ?? 0}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Upload error: {ex.Message}");
                    uploadedAudioBytes.Value = null;
                }
            },
            "audio/*",
            "uploaded-audio"
        );

        var downloadUrl = this.UseDownload(() =>
        {
            // var bytes = resultingAudioBytes.Value;

            var bytes = GenerateTone(440, 2);
            return bytes;
        }, "audio/*", "audio.wav");


        return Layout.Vertical().Gap(4).Padding(3)
               | new Card(Layout.Vertical().Gap(4).Padding(3)
                    | Text.H2("NAudio Demo")
                    | Text.Muted("NAudio is a library for audio processing.")
                    | (Layout.Horizontal().Width(Size.Full())

                        // Upload section
                        // | fileInputState.ToFileInput(uploadUrl, "Choose wave file to convert to MP3")
                        // | Text.Large(selected)

                        // Download section
                        | new Button("Download").Primary().Url(downloadUrl.Value).Icon(Icons.Download)
                    )
               )
                    .Width(Size.Full());
    }

    private static byte[] GenerateTone(int frequency, int durationSeconds = 2)
    {
        Console.WriteLine($"[GenerateTone] Starting generation of tone at {frequency}Hz for {durationSeconds} seconds");
        try
        {
            var waveFormat = new WaveFormat(44100, 16, 1);

            var signalGenerator = new SignalGenerator()
            {
                Type = SignalGeneratorType.Sin,
                Frequency = frequency,
                Gain = 0.2
            }.Take(TimeSpan.FromSeconds(durationSeconds));

            using (var outputStream = new MemoryStream())
            {
                var waveProvider = new SampleToWaveProvider16(signalGenerator);

                using (var writer = new WaveFileWriter(outputStream, waveFormat))
                {
                    int totalBytes = waveFormat.AverageBytesPerSecond * durationSeconds;
                    byte[] buffer = new byte[totalBytes];
                    waveProvider.Read(buffer, 0, totalBytes);
                    writer.Write(buffer, 0, totalBytes);
                }

                return outputStream.ToArray();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[GenerateTone] ERROR: {ex.Message}");
            return Array.Empty<byte>();
        }
    }
}