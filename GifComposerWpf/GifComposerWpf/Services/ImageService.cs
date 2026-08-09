using GifComposerWpf.Models;
using GifComposerWpf.Services.Interfaces;
using System.IO;
using System.Windows.Media.Imaging;

namespace GifComposerWpf.Services
{
    public class ImageService : IImageService
    {
        public ImageModel GetImageInfo(string fullPath, int decodePixelWidth)
        {
            /*--- 讀取圖片資訊 ---*/
            var bitmap = new BitmapImage();
            bitmap.BeginInit(); //開始設定這個 BitmapImage
            bitmap.CacheOption = BitmapCacheOption.OnLoad; //載入時就把圖片資料讀進記憶體
            bitmap.UriSource = new Uri(fullPath); //指定要讀取哪一張圖片
            bitmap.EndInit(); //表示設定完成，開始依照剛才的設定載入圖片。
            bitmap.Freeze();

            /*--- 建立縮圖 ---*/
            var thumbnail = new BitmapImage();
            thumbnail.BeginInit();
            thumbnail.CacheOption = BitmapCacheOption.OnLoad;
            thumbnail.DecodePixelWidth = decodePixelWidth;
            thumbnail.UriSource = new Uri(fullPath);
            thumbnail.EndInit();
            thumbnail.Freeze(); //設成不可修改

            return new ImageModel
            {
                Name = Path.GetFileName(fullPath),
                Path = fullPath,
                Width = bitmap.PixelWidth,
                Height = bitmap.PixelHeight,
                Thumbnail = thumbnail,
            };

            throw new NotImplementedException();
        }
    }
}
