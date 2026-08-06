using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GifComposerWpf.Services.Interfaces;
using System.Diagnostics;

namespace GifComposerWpf.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly IFileDialogService _fileDialogService;


       
        public MainViewModel(IFileDialogService fileDialogService)
        {
            _fileDialogService = fileDialogService;
        }

        [RelayCommand]
        private void LoadImages()
        {
            var paths = _fileDialogService.OpenFiles("選擇圖片", "Image|*.png;*.jpg;*.jpeg;*.bmp;*.webp");

            foreach(var p in paths)
            {
                Debug.WriteLine(p);
            }
        }

    }
}
