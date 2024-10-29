namespace Streaming.Helpers.VideoTranscoder.Properties;

public record class HLSTranscoderProperties : VideoTranscoderProperties
{
    public required string InputPath { get; init; }
    public required HLSCodec Codec { get; init; }
    public required string StreamUrl { get; init; }

}