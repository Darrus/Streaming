using Microsoft.AspNetCore.Mvc;
using Streaming.Models.Response;

namespace Streaming.Controllers;

public interface IVideoController
{
    public Task<ActionResult> GetVideosAsync();
    public Task<ActionResult<ResponseVideoDetail>> GetVideoAsync(string videoName);
    public Task<ActionResult> UploadVideoAsync();
    public Task<FileContentResult> StreamVideoAsync(string videoName, string partName);
}