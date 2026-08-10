using System.Windows.Media.Imaging;

namespace GifComposerWpf.Models
{
    public record ImageModel
    {
        /// <summary>
        /// 排序
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// 檔名
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 路徑
        /// </summary>
        public string Path { get; set; } = string.Empty;

        /// <summary>
        /// 原始圖片寬度
        /// </summary>
        public int Width { get; set; }
        
        /// <summary>
        /// 原始圖片高度
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// 縮圖
        /// </summary>
        public BitmapImage? Thumbnail {  get; set; }

        /// <summary>
        /// 延遲時間
        /// </summary>
        public int Delay { get; set; } = 100;
    }
}
