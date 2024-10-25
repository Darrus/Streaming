namespace Streaming.Models.Response;

public record class ResponseVideoDetail
{
    public required string Name { get; init; }
    public required string StreamFileName { get; init; }    
}