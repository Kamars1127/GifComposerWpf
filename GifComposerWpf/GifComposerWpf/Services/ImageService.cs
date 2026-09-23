using GifComposerWpf.Models;
using GifComposerWpf.Services.Interfaces;
using ImageMagick;
using System.IO;
using System.Windows.Media.Imaging;

namespace GifComposerWpf.Services
{
    public class ImageService : IImageService
    {
        /// <summary>
        /// 支援的圖片副檔名。
        /// </summary>
        private static readonly string[] SupportedExtensions = { ".jpg", ".jpeg", ".png", ".bmp", ".webp" };


        public ImageModel GetImageInfo(string imagePath, int decodePixelWidth)
        {
            /*--- 讀取圖片資訊 ---*/
            var bitmap = new BitmapImage();
            bitmap.BeginInit(); 
            bitmap.CacheOption = BitmapCacheOption.OnLoad; 
            bitmap.UriSource = new Uri(imagePath); 
            bitmap.EndInit(); //表示設定完成，開始依照剛才的設定載入圖片。
            bitmap.Freeze();

            /*--- 建立縮圖 ---*/
            var thumbnail = LoadImage(imagePath, decodePixelWidth);

            return new ImageModel
            {
                Name = Path.GetFileName(imagePath),
                Path = imagePath,
                Width = bitmap.PixelWidth,
                Height = bitmap.PixelHeight,
                Thumbnail = thumbnail,
            };

            throw new NotImplementedException();
        }


        public bool IsSupportedImage(string fullPath)
        {
            if(!File.Exists(fullPath)) return false;

            var extension = Path.GetExtension(fullPath);

            //檢查 extension 是否存在於 SupportedExtensions，而且比較時不區分大小寫。
            return SupportedExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase);
        }

        
        public BitmapImage LoadImage(string imagePath, int decodePixelWidth = 1024)
        {
            var bitmap = new BitmapImage(); //建立一個新的 BitmapImage 物件，此時還沒有真正載入圖片。

            bitmap.BeginInit(); //初始化 BitmapImage

            /*
             * 圖片會在 EndInit() 時完整載入到記憶體。
             * 使用 OnLoad 後，圖片載入完成就不需要持續占用原始檔案，
             * 可避免圖片被 WPF 鎖住而無法刪除或修改。
             */
            bitmap.CacheOption = BitmapCacheOption.OnLoad;

            bitmap.UriSource = new Uri(imagePath); //指定要載入哪一張圖片。
            bitmap.DecodePixelWidth = decodePixelWidth; //指定圖片解碼寬度，並維持原始圖片比例，以降低記憶體使用量。
            bitmap.EndInit(); //結束初始化，並依照上述設定完成圖片載入。

            bitmap.Freeze(); //將 BitmapImage 設為唯讀，避免後續修改並提升使用效率。

            return bitmap;
        }

        public void CreateGif(IEnumerable<ImageModel> images, GifOutputOptionsModel options)
        {
            using var collection = new MagickImageCollection();

            foreach(var img in images)
            {
                var image = new MagickImage(img.Path);

                //將圖片調整為輸出設定的寬度與高度
                image.Resize(
                    (uint)options.Width,
                    (uint)options.Height);

                /*設定此張圖片在 Gif 中的顯示時間
                 *AnimationDelay 的單位是 1/100 秒（10 ms）
                 *因 img.Delay 是以毫秒(ms)儲存，所以需要除以 10
                 */
                image.AnimationDelay = (uint)(img.Delay / 10);

                //播放下一幀前，清除目前這一幀
                image.GifDisposeMethod = GifDisposeMethod.Background;

                collection.Add(image);
            }

            /*設定 Gif 的循環次數
             * 0 => 無限循環
             * 1~ => 播放次數
             */
            collection[0].AnimationIterations = (uint)options.LoopCount;

            collection.Write(options.OutputPath, MagickFormat.Gif);
        }
    }
}
