namespace XnbReader.MonoGameShims;

/// <summary>
/// Represents a video.
/// </summary>
public record Video(string FileName, TimeSpan Duration, int Width, int Height, float Fps, VideoSoundtrackType VideoSoundtrackType)
{
    [ReaderConstructor]
    public Video(string fileName, float durationMilliseconds, int width, int height, float fps, VideoSoundtrackType videoSoundtrackType): this(fileName, TimeSpan.FromMilliseconds(durationMilliseconds), width, height, fps, videoSoundtrackType) { }
}

/// <summary>
/// Type of sounds in a video
/// </summary>
public enum VideoSoundtrackType
{
    /// <summary>
    /// This video contains only music.
    /// </summary>
    Music,

    /// <summary>
    /// This video contains only dialog.
    /// </summary>
    Dialog,

    /// <summary>
    /// This video contains music and dialog.
    /// </summary>
    MusicAndDialog,
}