using System.Text;
using Streaming.Exceptions;
using Streaming.Helpers.VideoTranscoder;
using Streaming.Helpers.VideoTranscoder.Properties;

namespace Streaming.Services;

public class VideoService : IVideoService
{
    readonly ILogger<VideoService> logger;
    readonly IVideoTranscoder transcoder;
    readonly string VIDEO_PATH;
    readonly string HOST_URL;

    public VideoService(ILogger<VideoService> logger, IConfiguration configuration)
    {
        string? ffmpegPath = configuration.GetValue<string>("FFMPEG_PATH");
        if(String.IsNullOrEmpty(ffmpegPath)) {
            throw new AppException(ErrorCodes.E001);
        }

        string? assetsPath = configuration.GetValue<string>("ASSETS_PATH");
        if(String.IsNullOrEmpty(assetsPath)) {
            throw new AppException(ErrorCodes.E002);
        }

        string? hostUrl = configuration.GetValue<string>("HOST_URL");
        if(String.IsNullOrEmpty(hostUrl)) {
            throw new AppException(ErrorCodes.E003);
        }

        HOST_URL = hostUrl;
        VIDEO_PATH = $"{assetsPath}\\Videos";
        transcoder = new HLSTranscoder(ffmpegPath);
        this.logger = logger;
    }

    public async Task StreamVideoAsync(string videoName, string partName)
    {
        throw new NotImplementedException();
    }

    public async Task UploadVideoAsync()
    {
        string videoName = "video.mp4";
        await transcoder.TranscodeVideoAsync(
            new HLSTranscoderProperties{
                HostUrl = HOST_URL,
                InputPath = $"{VIDEO_PATH}\\{videoName}",
                OutputPath = $"{VIDEO_PATH}\\converted",
                Codec = HLSCodec.HLS_1080P 
            }
        );
        throw new NotImplementedException();
    }
}