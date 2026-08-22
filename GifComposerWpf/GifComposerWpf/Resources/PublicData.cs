
using GifComposerWpf.Models;

namespace GifComposerWpf.Resources
{
    public static class PublicData
    {
        public static IReadOnlyList<SizeModel> PresetSizeList { get; } =
        [
            new(){Width = 320, Height = 240},
            new(){Width = 640, Height = 360},
            new(){Width = 800, Height = 600},
            new(){Width = 1024, Height = 768},
            new(){Width = 1280, Height = 720},
            new(){Width = 1920, Height = 1080},
        ];

        /// <summary>
        /// 圖片檔案類型
        /// </summary>
        public static string ImageFileType { get; } = "Image|*.png;*.jpg;*.jpeg;*.bmp;*.webp";

        public static string GifFileType { get; } = "GIF|*.gif";
    }

    /// <summary>
    /// Gif 尺寸模式
    /// </summary>
    public enum GifSizeMode
    {
        FirstImage,
        Preset,
        Custom
    }

    /// <summary>
    /// gif 循環模式
    /// </summary>
    public enum GifLoopMode
    {
        Infinite,
        Custom
    }
}
