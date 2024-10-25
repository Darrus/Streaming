using Streaming.Helpers.VideoTranscoder.Properties;

namespace Streaming.Helpers.VideoTranscoder;

public interface IVideoTranscoder
{
    public Task TranscodeVideoAsync(VideoTranscoderProperties properties);
}