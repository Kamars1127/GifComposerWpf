using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GifComposerWpf.Behaviors;
using GifComposerWpf.Services.Interfaces;
using GongSolutions.Wpf.DragDrop;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;

namespace GifComposerWpf.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly IFileDialogService _fileDialogService;
        private readonly IImageService _imageService;

        public IDropTarget ImageItemDropHandler { get; } = new ImageItemDropHandler();

        /// <summary>
        /// 圖片列表
        /// </summary>
        [ObservableProperty]
        public partial ObservableCollection<ImageItemViewModel> Images { get; set; } = new();

        /// <summary>
        /// 選取的圖片
        /// </summary>
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(RemoveSelectedImageCommand))]
        private ImageItemViewModel? selectedImage;
       

        /// <summary>
        /// 載入圖片
        /// </summary>
        [RelayCommand]
        private void LoadImages()
        {
            var paths = _fileDialogService.OpenFiles("選擇圖片", "Image|*.png;*.jpg;*.jpeg;*.bmp;*.webp");

            var order = Images.Count();

            foreach (var p in paths)
            {
                order++;
                var model = _imageService.GetImageInfo(p, 64);
                model.Order = order;

                Images.Add(new ImageItemViewModel(model));
            }
        }

        /// <summary>
        /// 刪除列表中全部圖片
        /// </summary>
        [RelayCommand(CanExecute=nameof(CanClearImages))]
        private void ClearAllImages()
        {
            Images.Clear();
        }

        /// <summary>
        /// 刪除選取的圖片
        /// </summary>
        [RelayCommand(CanExecute = nameof(CanRemoveSelectedImage))]
        private void RemoveSelectedImage()
        {
            if (SelectedImage is null) return;

            Images.Remove(SelectedImage);

            for(int i = 0; i < Images.Count; i++)
            {
                Images[i].Order = i + 1;
            }
        }

        /// <summary>
        /// 控制啟用刪除全部圖片按鈕
        /// </summary>
        /// <returns></returns>
        private bool CanClearImages()
        {
            return Images.Count > 0;
        }

        /// <summary>
        /// 控制啟用刪除選取圖片按鈕
        /// </summary>
        /// <returns></returns>
        private bool CanRemoveSelectedImage()
        {
            return SelectedImage is not null;
        }

        public MainViewModel(IFileDialogService fileDialogService, IImageService imageService)
        {
            _fileDialogService = fileDialogService;
            _imageService = imageService;

            Images.CollectionChanged += Images_CollectionChanged;
        }

        /// <summary>
        /// 當 mages.Count 改變時，通知 Command 重新執行 CanExecute
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Images_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            ClearAllImagesCommand.NotifyCanExecuteChanged();
        }
    }
}
