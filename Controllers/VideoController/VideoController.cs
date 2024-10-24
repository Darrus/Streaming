using Microsoft.AspNetCore.Mvc;
using Streaming.Services;

namespace Streaming.Controllers;

public class VideoController(ILogger<VideoController> logger, IVideoService videoService) : BaseController, IVideoController
{
    [HttpGet]
    public async Task<ActionResult> GetVideosAsync()
    {
        logger.LogInformation("GetVideosAsync");
        return Ok();
    }

    [HttpGet("{videoName}")]
    public async Task<ActionResult> GetVideoAsync(string videoName)
    {
        logger.LogInformation("GetVideoAsync");
        return Ok();
    }

    [HttpGet("{videoName}/stream")]
    public async Task<ActionResult> StreamVideoAsync(string videoName)
    {
        logger.LogInformation("StreamVideoAsync");
        await videoService.StreamVideoAsync();
        return Ok();
    }
}