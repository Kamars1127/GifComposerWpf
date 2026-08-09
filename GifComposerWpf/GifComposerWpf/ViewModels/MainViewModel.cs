using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GifComposerWpf.Behaviors;
using GifComposerWpf.Services.Interfaces;
using GongSolutions.Wpf.DragDrop;
using System.Collections.ObjectModel;

namespace GifComposerWpf.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly IFileDialogService _fileDialogService;
        private readonly IImageService _imageService;

        public IDropTarget ImageItemDropHandler { get; } = new ImageItemDropHandler();

        [ObservableProperty]
        public partial ObservableCollection<ImageItemViewModel> Images { get; set; } = new();

        public MainViewModel(IFileDialogService fileDialogService, IImageService imageService)
        {
            _fileDialogService = fileDialogService;
            _imageService = imageService;
        }

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

        [RelayCommand]
        private void ClearAllImages()
        {
            Images.Clear();
        }

       
    }
}
