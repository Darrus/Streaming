namespace Streaming.Helpers.VideoTranscoder.Properties;

public abstract record class VideoTranscoderProperties
{
    public required string OutputPath { get; init; }
}