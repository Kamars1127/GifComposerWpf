using GifComposerWpf.Resources;

namespace GifComposerWpf.Models
{
    public class GifOutputOptionsModel
    {

        /// <summary>
        /// 寬
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// 高
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// 輸出路徑
        /// </summary>
        public string OutputPath { get; set; } = string.Empty;

        /// <summary>
        /// 循環次數
        /// </summary>
        public int LoopCount{  get; set;}


    }
}
