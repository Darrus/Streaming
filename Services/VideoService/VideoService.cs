
using Streaming.Helpers;

namespace Streaming.Services;

public class VideoService(ILogger<VideoService> logger, IVideoTranscoder transcoder) : IVideoService
{
    public async Task StreamVideoAsync()
    {
        logger.LogInformation("StreamVideoAsync");
        await transcoder.TranscodeVideoAsync(null);
    }
}