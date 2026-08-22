using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GifComposerWpf.Models;
using System.Windows.Media.Imaging;

namespace GifComposerWpf.ViewModels
{
    public partial class ImageItemViewModel : ObservableObject
    {
        public ImageModel ImageMD { get; } = new();

        public int Order
        {
            get => ImageMD.Order;
            set
            {
                if (value == ImageMD.Order) return;

                ImageMD.Order = value;
                OnPropertyChanged();
            }
        }
        
        public string Name
        {
            get => ImageMD.Name;
            set
            {
                if(value == ImageMD.Name) return;

                ImageMD.Name = value;
                OnPropertyChanged();
            }
        }

        public string Path
        {
            get => ImageMD.Path;
            set
            {
                if (value == ImageMD.Path) return;

                ImageMD.Path = value;
                OnPropertyChanged();
            }
        }

        public int Width
        {
            get => ImageMD.Width;
            set
            {
                if (value == ImageMD.Width) return;

                ImageMD.Width = value;
                OnPropertyChanged();
            }
        }
      
        public int Height
        {
            get => ImageMD.Height;
            set
            {
                if (value == ImageMD.Height) return;

                ImageMD.Height = value;
                OnPropertyChanged();
            }
        }

        public BitmapImage? Thumbnail
        {
            get => ImageMD.Thumbnail;
            set
            {
                if (value == ImageMD.Thumbnail) return;

                ImageMD.Thumbnail = value;
                OnPropertyChanged();
            }
        }

        public int Delay
        {
            get => ImageMD.Delay;
            set
            {
                if (value == ImageMD.Delay) return;

                ImageMD.Delay = value;
                OnPropertyChanged();
            }
        }

        [ObservableProperty]
        private bool isLockDelayInput;


        [RelayCommand]
        private void lockDelayInput()
        {
            IsLockDelayInput = !IsLockDelayInput;
        }

        public ImageItemViewModel(ImageModel model)
        {
            ImageMD = model;
        }
    }
}
