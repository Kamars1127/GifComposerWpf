using CommunityToolkit.Mvvm.ComponentModel;
using GifComposerWpf.Models;
using System.Windows.Media.Imaging;

namespace GifComposerWpf.ViewModels
{
    public partial class ImageItemViewModel : ObservableObject
    {
        private readonly ImageModel imageMD = new();

        public int Order
        {
            get => imageMD.Order;
            set
            {
                if (value == imageMD.Order) return;

                imageMD.Order = value;
                OnPropertyChanged();
            }
        }
        
        public string Name
        {
            get => imageMD.Name;
            set
            {
                if(value == imageMD.Name) return;

                imageMD.Name = value;
                OnPropertyChanged();
            }
        }

        public string Path
        {
            get => imageMD.Path;
            set
            {
                if (value == imageMD.Path) return;

                imageMD.Path = value;
                OnPropertyChanged();
            }
        }

        public int Width
        {
            get => imageMD.Width;
            set
            {
                if (value == imageMD.Width) return;

                imageMD.Width = value;
                OnPropertyChanged();
            }
        }
      
        public int Height
        {
            get => imageMD.Height;
            set
            {
                if (value == imageMD.Height) return;

                imageMD.Height = value;
                OnPropertyChanged();
            }
        }

        public BitmapImage? Thumbnail
        {
            get => imageMD.Thumbnail;
            set
            {
                if (value == imageMD.Thumbnail) return;

                imageMD.Thumbnail = value;
                OnPropertyChanged();
            }
        }

        public ImageItemViewModel(ImageModel model)
        {
            imageMD = model;
        }
    }
}
