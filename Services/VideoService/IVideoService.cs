namespace Streaming.Services;

public interface IVideoService
{
    public Task UploadVideoAsync();
    public Task StreamVideoAsync(string videoName, string partName);
}