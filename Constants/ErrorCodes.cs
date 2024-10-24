using Streaming.Models;

namespace Streaming.Constants;

public static class ErrorCodes
{
    #region 000 - Unhandled Exceptions

    public static string E000 = "Unhandled error";

    #endregion

    #region 100 - System Exceptions

    public static ErrorCode E100 = new ErrorCode("E100", "FFMPEG_PATH is not set in appsettings.");
    public static ErrorCode E101 = new ErrorCode("E101", "VIDEO_PATH is not set in appsettings.");

    #endregion

    #region 200 - Client Exceptions
    #endregion
}