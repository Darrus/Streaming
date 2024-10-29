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
        transcoder = new HLSTranscoder();
        this.logger = logger;
    }

    public async Task<byte[]> StreamVideoBytesAsync(string videoName, string partName)
    {
        string path = $"{VIDEO_PATH}\\converted\\{videoName}\\{partName}";
        if(!File.Exists(path)) {
            throw new AppException(ErrorCodes.E101);
        }

        return await File.ReadAllBytesAsync(path);
    }

    public async Task UploadVideoAsync()
    {
        // TODO: Remove temporary variable
        string videoName = "video.mp4";

        await transcoder.TranscodeVideoAsync(
            new HLSTranscoderProperties{
                InputPath = $"{VIDEO_PATH}\\{videoName}",
                OutputPath = $"{VIDEO_PATH}\\converted",
                Codec = HLSCodec.HLS_ADAPTIVE 
            }
        );
    }
}