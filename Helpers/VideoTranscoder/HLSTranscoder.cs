using System.Diagnostics;
using System.Text;
using Streaming.Helpers.VideoTranscoder.Properties;

namespace Streaming.Helpers.VideoTranscoder;

public class HLSTranscoder(string FFMPEG_PATH) : IVideoTranscoder
{
    /**
     * -c:v = codec:video
     * -b:v = bitrate:video
    **/
    readonly Dictionary<HLSCodec, string> FFMPEG_ARGUMENTS = new(){
        {HLSCodec.HLS_480P, "-c:v libx264 -b:v 1m -hls_time 10 -hls_list_size 0 -f hls -hls_base_url \"{0}\""},
        {HLSCodec.HLS_720P, "-c:v libx264 -b:v 3m -hls_time 10 -hls_list_size 0 -f hls -hls_base_url \"{0}\""},
        {HLSCodec.HLS_1080P, "-c:v libx264 -b:v 5m -hls_time 10 -hls_list_size 0 -f hls -hls_base_url \"{0}\""}
    };

    public async Task TranscodeVideoAsync(VideoTranscoderProperties properties)
    {
        HLSTranscoderProperties? hlsProps = properties as HLSTranscoderProperties;
        if(hlsProps is null) {
            throw new ArgumentException("Provide a valid HLSTranscoderProperties.");
        }

        string fileName = Path.GetFileName(hlsProps.InputPath);
        if(String.IsNullOrEmpty(fileName)) {
            throw new ArgumentNullException("File is not found.");
        }

        string fullOutputPath = $"{hlsProps.OutputPath}\\{fileName}";
        string newFileName = $"{fileName.Substring(0, fileName.LastIndexOf("."))}.m3u8";

        Directory.CreateDirectory(fullOutputPath);

        string[] arguments = [
            $"-i \"{hlsProps.InputPath}\"", // Input path
            String.Format(FFMPEG_ARGUMENTS[hlsProps.Codec], $"\"{hlsProps.HostUrl}/Video/{fileName}/\""), // Codec arguments
            $"\"{fullOutputPath}\\{newFileName}\"" // Output path
        ];
        StringBuilder sb = new StringBuilder();
        sb.AppendJoin(" ", arguments);

        Process process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = FFMPEG_PATH,
                Arguments = sb.ToString(),
                // RedirectStandardOutput = true,
                // RedirectStandardError = true,
                UseShellExecute = true,
                CreateNoWindow = true
            }
        };
        process.Start();
        await process.WaitForExitAsync();
    }
}