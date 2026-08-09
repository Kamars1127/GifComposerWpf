using GifComposerWpf.Models;

namespace GifComposerWpf.Services.Interfaces
{
    public interface IImageService
    {
        /// <summary>
        /// 取得圖片資訊
        /// </summary>
        /// <param name="fullPath">圖片路徑</param>
        /// <returns></returns>
        ImageModel GetImageInfo(string fullPath, int decodePixelWidth);
    }
}
