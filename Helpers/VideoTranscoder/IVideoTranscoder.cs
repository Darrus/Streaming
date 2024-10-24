namespace Streaming.Helpers;

public interface IVideoTranscoder
{
    public Task TranscodeVideoAsync(FileStream? file);
}