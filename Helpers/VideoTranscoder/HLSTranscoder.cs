using System.Diagnostics;
using System.Text;
using Streaming.Helpers.VideoTranscoder.Properties;

namespace Streaming.Helpers.VideoTranscoder;

/**
The frames in your H.264 video are grouped into units called GOPs (Group Of Pictures). Inside these GOPs frames are classified into three types:

I-frame: frame that stores the whole picture
P-frame: frame that stores only the changes between the current picture and previous ones
B-frame: frame that stores differences with previous or future pictures

Additionally, I-frames can be classified as IDR frames and non-IDR frames. 
The difference is that frames following an IDR frame cannot reference any frame that comes before the IDR frame, 
while in the case of a non-IDR frame there are no limitations.

Every GOP starts with an I-frame, also called a keyframe, but may contain more than one. 
To create further confusion, a GOP can start with an IDR frame or with a non-IDR frame. 
This means that frames in the GOP can sometimes refer to previous GOPs (in this case the GOP is said to be "open"), and sometimes not (in this case it's closed).

It's common to see the structure of a GOP represented as in this example: IBBBPBBBPBBBI. 
Here the length of the the GOP is 12 frames, with 3 B-frames between each P-frame.

Now your questions:

- keyint specifies the maximum length of the GOP, so the maximum interval between each keyframe, 
which remember that can be either an IDR frame or a non-IDR frame. 
I'm not completely sure but I think that by default ffmpeg will require every I-frame to be an IDR frame, 
so in practice you can use the terms IDR frame and I-frame interchangeably

- min-keyint specifies the minimum length of the GOP. 
This is because the encoder might decide that it makes sense to add a keyframe before the keyint value, so you can put a limit

- no-scenecut When the encoder determines that there's been a scene cut, 
it may decide to insert an additional I-frame. 
The issue is that I-frames are very expensive if compared to other frame types, so when encoding for streaming you want to disable it.

@source https://video.stackexchange.com/questions/24680/what-is-keyint-and-min-keyint-and-no-scenecut
**/
public class HLSTranscoder() : IVideoTranscoder
{
    /**
     * ih = input height
     * -1/-2 = allows to autoscale width keeping the aspect ratio
    **/
    readonly string[] SCALE_ARGUMENTS = [
        "scale='-2:min(480, ih)'", // HLSCodec.HLS_480P
        "scale='-2:min(720, ih)'", // HLSCodec.HLS_720P
        "scale='-2:min(1080, ih)'" // HLSCodec.HLS_1080P
    ];

    /**
     * -c:v:0           = codec:video:stream
     * -b:v:0           = bitrate:video:stream
     * -g               = keyframe interval
     * -keyint_min      = minimum length of GOP
     * -sc_threshold    = disable insertion of additional I-frame, equivalent to no-scenecut but codec neutral
    **/
    readonly string[] VIDEO_ENCODING_ARGUMENTS = [
        "-c:v:0 libx264 -b:v:0 1m -preset slow -g 48 -sc_threshold 0 -keyint_min 48", // HLSCodec.HLS_480P
        "-c:v:0 libx264 -b:v:0 3m -preset slow -g 48 -sc_threshold 0 -keyint_min 48", // HLSCodec.HLS_720P
        "-c:v:0 libx264 -b:v:0 5m -preset slow -g 48 -sc_threshold 0 -keyint_min 48" // HLSCodec.HLS_1080P
    ];

    /**
     * -c:a:0 = codec:audio:stream
     * -b:a:0 = birate:audio:stream
     * -ac    = audio channel
    **/
    readonly string[] AUDIO_ENCODING_ARGUMENTS = [
        "-c:a:0 aac -b:a:0 48k -ac 2", // HLSCodec.HLS_480P
        "-c:a:0 aac -b:a:0 96k -ac 2", // HLSCodec.HLS_720P
        "-c:a:0 aac -b:a:0 96k -ac 2" // HLSCodec.HLS_1080P
    ];

    // -hls_base_url is optional, hls.js is smart enough to grab based on the same url as the m3u8
    readonly string HLS_ARGUMENTS = "-hls_time 10 -hls_list_size 0 -f hls";
    readonly string HLS_ADAPTIVE_ARGUMENTS = "-hls_time 10 -hls_list_size 0 -f hls -master_pl_name {0}.m3u8 -var_stream_map \"v:0,a:0 v:1,a:1 v:2,a:2\" {1}_%v.m3u8";

    public async Task TranscodeVideoAsync(VideoTranscoderProperties properties)
    {
        HLSTranscoderProperties? hlsProps = properties as HLSTranscoderProperties;
        if(hlsProps is null) {
            throw new ArgumentException("Provide a valid HLSTranscoderProperties.");
        }

        string fileName = Path.GetFileName(hlsProps.InputPath);
        if(String.IsNullOrEmpty(fileName)) {
            throw new ArgumentNullException("File is not found.");
        }

        string fullOutputPath = $"{hlsProps.OutputPath}\\{fileName}";
        Directory.CreateDirectory(fullOutputPath);

        string arguments = BuildHLSArguments(hlsProps, fileName, fullOutputPath);
        Console.WriteLine(arguments);

        Process process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        process.OutputDataReceived += (sender, args) => {
            Console.WriteLine(args.Data);
        };
         process.ErrorDataReceived += (sender, args) => {
            Console.WriteLine(args.Data);
        };
        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
        await process.WaitForExitAsync();
    }

    private string BuildHLSArguments(HLSTranscoderProperties hlsProps, string fileName, string fullOutputPath)
    {
        string fileNameWithoutExtension = $"{fileName.Substring(0, fileName.LastIndexOf("."))}";
        List<string> arguments = [$"-i \"{hlsProps.InputPath}\""];

        if(hlsProps.Codec == HLSCodec.HLS_ADAPTIVE) {
            string[] varIn = ["v1", "v2", "v3"];
            string[] varOut = ["v1Out", "v2Out", "v3Out"];
            arguments.Add($"-filter_complex \"[0:v]split=3[{varIn[0]}][{varIn[1]}][{varIn[2]}];");

            for(int i = 0; i < SCALE_ARGUMENTS.Length; ++i) {
                arguments.Add($"[{varIn[i]}]{SCALE_ARGUMENTS[i]}[{varOut[i]}];");
            }
            arguments.Add("\"");

            for(int i = 0; i < VIDEO_ENCODING_ARGUMENTS.Length; ++i) {
                arguments.Add($"-map \"[{varOut[i]}]\" {VIDEO_ENCODING_ARGUMENTS[i].Replace("v:0", $"v:{i}")}");
            }

            // a:0 is actually short for 0:a:0 where [input_file_index]:[stream_type]:[stream_index]
            for(int i = 0; i < AUDIO_ENCODING_ARGUMENTS.Length; ++i) {
                arguments.Add($"-map a:0 {AUDIO_ENCODING_ARGUMENTS[i].Replace("a:0", $"a:{i}")}");
            }

            arguments.Add(String.Format(HLS_ADAPTIVE_ARGUMENTS, fileNameWithoutExtension, $"\"{fullOutputPath}\\{fileNameWithoutExtension}\""));
        } else {
            int codecIndex = (int)hlsProps.Codec;
            arguments.Add($"-vf \"{SCALE_ARGUMENTS[codecIndex]}\"");
            arguments.Add(VIDEO_ENCODING_ARGUMENTS[codecIndex]);
            arguments.Add(AUDIO_ENCODING_ARGUMENTS[codecIndex]);
            arguments.Add(HLS_ARGUMENTS);
            arguments.Add($"\"{fullOutputPath}\\{fileNameWithoutExtension}.m3u8\"");
        }

        StringBuilder sb = new StringBuilder();
        sb.AppendJoin(" ", arguments);
        return sb.ToString();
    }
}