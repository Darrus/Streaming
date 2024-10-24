using System.Diagnostics;
using Streaming.Constants;
using Streaming.Exceptions;
using Streaming.Helpers;

public class VideoTranscoder(ILogger<VideoTranscoder> logger, IConfiguration configuration) : IVideoTranscoder
{
    public async Task TranscodeVideoAsync(FileStream file)
    {
        string filename = "video.mp4";
        try
        {
            string FFMPEG_PATH = GetFFMPEGPath();
            string OUTPUT_PATH = $"{GetOutputVideoPath()}\\{filename}";
            string inputFilePath = $".\\Assets\\Videos\\{filename}";

            logger.LogInformation(FFMPEG_PATH);

            Directory.CreateDirectory(OUTPUT_PATH);
            logger.LogInformation(OUTPUT_PATH);
            string arguments = $"-i \"{inputFilePath}\" -codec: copy -hls_time 10 -hls_list_size 0 -f hls \"{OUTPUT_PATH}\\video.m3u8\"";

            Process process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = FFMPEG_PATH,
                    Arguments = arguments,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            process.Start();
            await process.WaitForExitAsync();
        }
        catch(AppException e)
        {
            logger.LogError(e.ErrorMessage);
        }
        catch(Exception e)
        {
            logger.LogError(e.ToString());
        }
    }

    private string GetFFMPEGPath()
    {
        string? FFMPEG_PATH = configuration.GetValue<string>("FFMPEG_PATH");
        if(FFMPEG_PATH == null)
        {
            throw new AppException(ErrorCodes.E101);
        }

        return FFMPEG_PATH;
    }

    private string GetOutputVideoPath()
    {
        string? VIDEO_PATH = configuration.GetValue<string>("VIDEO_PATH");
        if(VIDEO_PATH == null)
        {;
            throw new AppException(ErrorCodes.E101);
        }

        return VIDEO_PATH;
    }
}