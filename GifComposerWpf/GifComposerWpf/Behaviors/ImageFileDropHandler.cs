using GifComposerWpf.Services.Interfaces;
using GifComposerWpf.ViewModels;
using GongSolutions.Wpf.DragDrop;
using System.Windows;


namespace GifComposerWpf.Behaviors
{
    public class ImageFileDropHandler : IDropTarget
    {
        private readonly MainViewModel _mainVM;
        private readonly IImageService _imageService;

        /// <summary>
        /// 初始化圖片檔案拖放處理器
        /// </summary>
        /// <param name="mainVM">主畫面 ViewModel，用於更新拖曳狀態與加入圖片。</param>
        /// <param name="imageService">圖片服務，用於判斷拖入的檔案是否為支援的圖片格式。</param>
        public ImageFileDropHandler(MainViewModel mainVM, IImageService imageService)
        {
            _mainVM = mainVM;
            _imageService = imageService;
        }

        /// <summary>
        /// 當檔案拖曳至 DropTarget 上方時執行。
        /// </summary>
        /// <param name="dropInfo">GongSolutions 提供的拖放資訊</param>
        public void DragOver(IDropInfo dropInfo)
        {
            //確認拖曳內容為檔案，並取得檔案路徑。
            if (dropInfo.Data is not DataObject dataObject || !dataObject.GetDataPresent(DataFormats.FileDrop) ||
                dataObject.GetData(DataFormats.FileDrop) is not string[] files)
            {
                return;
            }

            //判斷拖入的檔案中是否至少包含一個支援的圖片格式。
            var hasSupportedImage = files.Any(_imageService.IsSupportedImage);

            if (!hasSupportedImage)
            {
                SetInvalidDrop(dropInfo);
                return;
            }

            // 紀錄目前有有效圖片拖曳至 DropTarget。
            _mainVM.IsImageDragOver = true;

            //顯示目前允許以 Copy 方式進行拖放。
            dropInfo.Effects = DragDropEffects.Copy;
        }

        /// <summary>
        /// 當使用者將檔案放置至 DropTarget 時執行。
        /// </summary>
        /// <param name="dropInfo">GongSolutions 提供的拖放資訊</param>
        public void Drop(IDropInfo dropInfo)
        {
            // Drop 完成後恢復拖曳狀態。
            _mainVM.IsImageDragOver = false;

            // 確認拖曳資料為 WPF DataObject。
            if (dropInfo.Data is not DataObject dataObject) return;

            // 確認拖曳內容包含檔案。
            if (!dataObject.GetDataPresent(DataFormats.FileDrop)) return;

            // 取得拖曳進來的檔案路徑。
            if (dataObject.GetData(DataFormats.FileDrop) is not string[] files) return;

            // 將檔案交由 MainViewModel 統一處理，
            _mainVM.AddImages(files);
        }

        /// <summary>
        /// 設定目前拖放資料為無效狀態。
        /// </summary>
        /// <param name="dragInfo">GongSolutions 提供的拖放資訊</param>
        private void SetInvalidDrop(IDropInfo dropInfo)
        {
            // 取消圖片拖曳 Hover 狀態。
            _mainVM.IsImageDragOver = false;

            // 禁止目前的拖放操作。
            dropInfo.Effects = DragDropEffects.None;
        }
    }
}
