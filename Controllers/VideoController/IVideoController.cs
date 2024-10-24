using Microsoft.AspNetCore.Mvc;

namespace Streaming.Controllers;

public interface IVideoController
{
    public Task<ActionResult> GetVideosAsync();
    public Task<ActionResult> GetVideoAsync(string videoName);
    public Task<ActionResult> StreamVideoAsync(string videoName);
}