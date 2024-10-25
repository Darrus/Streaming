namespace Streaming.Exceptions;

public static class ErrorCodes
{
    #region 000 - General Exceptions

    public const string E000 = "Unhandled error";
    // FFMPEG_PATH is not set in appsettings.
    public static readonly ErrorCode E001 = new ErrorCode("E100", "FFMPEG_PATH is not set in appsettings.");
    // ASSETS_PATH is not set in appsettings.
    public static readonly ErrorCode E002 = new ErrorCode("E101", "ASSETS_PATH is not set in appsettings.");
    // HOST_URL is not set in appsettings.
    public static readonly ErrorCode E003 = new ErrorCode("E101", "HOST_URL is not set in appsettings.");
    
    #endregion

    #region 100 - System Exceptions

    // Video does not exist
    public static readonly ErrorCode E101 = new ErrorCode("E101", "Video does not exist.");

    #endregion

    #region 200 - Client Exceptions
    #endregion
}