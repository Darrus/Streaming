using Microsoft.AspNetCore.Mvc;
using Streaming.Services;

namespace Streaming.Controllers;

public class VideoController(ILogger<VideoController> logger, IVideoService videoService) : BaseController, IVideoController
{
    [HttpGet]
    public async Task<ActionResult> GetVideosAsync()
    {
        throw new NotImplementedException();
    }

    [HttpGet("{videoName}")]
    public async Task<ActionResult> GetVideoAsync(string videoName)
    {
        throw new NotImplementedException();
    }

    [HttpGet("{videoName}/stream/{partName}")]
    public async Task<ActionResult> StreamVideoAsync(string videoName, string partName)
    {
        throw new NotImplementedException();
    }

    [HttpPost]
    public async Task<ActionResult> UploadVideo()
    {
        await videoService.UploadVideoAsync();
        throw new NotImplementedException();
    }
}