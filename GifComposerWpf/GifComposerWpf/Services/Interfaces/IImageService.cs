using GifComposerWpf.Models;
using System.Windows.Media.Imaging;

namespace GifComposerWpf.Services.Interfaces
{
    public interface IImageService
    {
        /// <summary>
        /// 取得圖片資訊
        /// </summary>
        /// <param name="imagePath">圖片路徑</param>
        /// <returns></returns>
        ImageModel GetImageInfo(string imagePath, int decodePixelWidth);

        /// <summary>
        /// 檢查檔案類型是否為支援的圖片
        /// </summary>
        /// <param name="imagePath"></param>
        /// <returns></returns>
        bool IsSupportedImage(string imagePath);

        /// <summary>
        /// 載入指定圖片，並依指定寬度進行解碼。
        /// </summary>
        /// <param name="imagePath">圖片路徑</param>
        /// <param name="decodePixelWidth">圖片解碼寬度，預設為 1024 像素。</param>
        /// <returns></returns>
        BitmapImage LoadImage(string imagePath, int decodePixelWidth = 1024);

        /// <summary>
        /// 產生 Gif
        /// </summary>
        /// <param name="images">圖片資訊集合</param>
        /// <param name="options">Gif 輸出設定</param>
        void CreateGif(IEnumerable<ImageModel> images, GifOutputOptionsModel options);
    }
}
