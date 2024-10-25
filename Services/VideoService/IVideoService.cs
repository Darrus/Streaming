namespace Streaming.Services;

public interface IVideoService
{
    public Task UploadVideoAsync();
    public Task<byte[]> StreamVideoBytesAsync(string videoName, string partName);
}