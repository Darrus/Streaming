using Microsoft.AspNetCore.Mvc;
using Streaming.Constants;
using Streaming.Models.Response;
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
    public async Task<ActionResult<ResponseVideoDetail>> GetVideoAsync(string videoName)
    {
        return new ResponseVideoDetail{Name = "video.mp4", StreamFileName = "video.m3u8"};
    }

    [HttpGet("{videoName}/{partName}")]
    public async Task<FileContentResult> StreamVideoAsync(string videoName, string partName)
    {
        byte[] fileBytes = await videoService.StreamVideoBytesAsync(videoName, partName);
        return File(fileBytes, "video/mp4", partName);
    }

    [HttpPost]
    public async Task<ActionResult> UploadVideoAsync()
    {
        await videoService.UploadVideoAsync();
        return Ok();
    }
}