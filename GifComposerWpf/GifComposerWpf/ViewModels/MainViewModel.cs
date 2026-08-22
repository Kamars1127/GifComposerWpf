using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GifComposerWpf.Behaviors;
using GifComposerWpf.Models;
using GifComposerWpf.Resources;
using GifComposerWpf.Services.Interfaces;
using GongSolutions.Wpf.DragDrop;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Windows.Media.Imaging;
using System.Windows.Threading;


namespace GifComposerWpf.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        /// <summary>
        /// 播放預覽次數
        /// </summary>
        private int playPreviewCount = 0;

        private readonly IFileDialogService _fileDialogService;
        private readonly IImageService _imageService;
        private readonly IDialogService _dialogService;
        private readonly DispatcherTimer _previewTimer;

        public IDropTarget ImageItemDropHandler { get; }
        public IDropTarget ImageFileDropHandler { get; }


        #region /*--- Property ---*/

        private SizeModel _selectedPresetSize;
        private int _currentPreviewIndex;


        /// <summary>
        /// 圖片列表
        /// </summary>
        [ObservableProperty]
        public partial ObservableCollection<ImageItemViewModel> Images { get; set; } = new();

        /// <summary>
        /// 選取的圖片
        /// </summary>
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(RemoveSelectedImageCommand))] // 當這個屬性發生變化時，自動通知 MoveImageUpCommand 重新判斷 CanExecute。
        [NotifyCanExecuteChangedFor(nameof(MoveImageUpCommand))]
        [NotifyCanExecuteChangedFor(nameof(MoveImageDownCommand))]
        private ImageItemViewModel? selectedImage;

        /// <summary>
        /// 圖片是否正在拖曳經過
        /// </summary>
        [ObservableProperty]
        private bool isImageDragOver;

        /// <summary>
        /// 輸出尺寸模式
        /// </summary>
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsEnablePresetSize))]
        [NotifyPropertyChangedFor(nameof(IsEnableCustomSize))]
        private GifSizeMode outputSizeMode;

        /// <summary>
        /// 輸出的寬度
        /// </summary>
        [ObservableProperty]
        private int outputWidth;

        /// <summary>
        /// 輸出的高度
        /// </summary>
        [ObservableProperty]
        private int outputHeight;


        /// <summary>
        /// 是否啟用常用尺寸選單
        /// </summary>
        public bool IsEnablePresetSize => OutputSizeMode == GifSizeMode.Preset;

        /// <summary>
        /// 是否啟用自訂輸入框
        /// </summary>
        public bool IsEnableCustomSize => OutputSizeMode == GifSizeMode.Custom;

        /// <summary>
        /// 選擇的常用尺寸
        /// </summary>
        [ObservableProperty]
        private SizeModel? selectedPresetSize;


        /// <summary>
        /// 目前預覽圖片的索引值
        /// </summary>
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CurrentPreviewNumber))]
        private int currentPreviewIndex;
      

        /// <summary>
        /// 目前預覽圖片的編號
        /// </summary>
        public int CurrentPreviewNumber => Images.Count == 0 ? 0 : CurrentPreviewIndex + 1;

        /// <summary>
        /// 目前預覽的圖片
        /// </summary>
        [ObservableProperty]
        private BitmapImage? previewImage;

        /// <summary>
        /// 是否正在播放預覽
        /// </summary>
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(PausePreviewCommand))]
        [NotifyCanExecuteChangedFor(nameof(PlayPreviewCommand))]
        private bool isPreviewPlaying;

        /// <summary>
        /// 輸出路徑
        /// </summary>
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(CreateGifCommand))]
        private string outputPath;

        /// <summary>
        /// 輸出循環模式
        /// </summary>
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsEnableCustomLoopCountBox))]
        private GifLoopMode outputLoopMode = GifLoopMode.Custom;

        /// <summary>
        /// 循環次數
        /// </summary>
        [ObservableProperty]
        private int customLoopCount = 1;


        /// <summary>
        /// 是否啟用自訂循環次數輸入框
        /// </summary>
        public bool IsEnableCustomLoopCountBox => OutputLoopMode == GifLoopMode.Custom;

        #endregion

        #region /*--- Observable Property Callbacks ---*/

        partial void OnSelectedPresetSizeChanged(SizeModel? value)
        {
            if (value is null) return;

            OutputWidth = value.Width;
            OutputHeight = value.Height;
        }

        #endregion


        #region /*--- Command ---*/

        /// <summary>
        /// 載入圖片
        /// </summary>
        [RelayCommand]
        private void LoadImages()
        {
            var paths = _fileDialogService.OpenFiles("選擇圖片", PublicData.ImageFileType);

            if (paths.Length <= 0) return;

            foreach (var p in paths)
            {
                var model = _imageService.GetImageInfo(p, 64);
                
                Images.Add(new ImageItemViewModel(model));
            }

            UpdateImageOrder();
        }

        /// <summary>
        /// 刪除列表中全部圖片
        /// </summary>
        [RelayCommand(CanExecute=nameof(HasImages))]
        private void ClearAllImages()
        {
            Images.Clear();

            OutputWidth = 0;
            OutputHeight = 0;
        }

        /// <summary>
        /// 刪除選取的圖片
        /// </summary>
        [RelayCommand(CanExecute = nameof(CanRemoveSelectedImage))]
        private void RemoveSelectedImage()
        {
            if (SelectedImage is null) return;

            Images.Remove(SelectedImage);

            UpdateImageOrder() ;
        }

        /// <summary>
        /// 將圖片上移
        /// </summary>
        [RelayCommand(CanExecute = nameof(CanMoveImageUp))]
        private void MoveImageUp()
        {
            MoveImage(-1);
        }

        /// <summary>
        /// 將圖片下移
        /// </summary>
        [RelayCommand(CanExecute =nameof(CanMoveImageDown))]
        private void MoveImageDown()
        {
            MoveImage(1);
        }

        /// <summary>
        /// 更換尺寸模式
        /// </summary>
        /// <param name="mode"></param>
        [RelayCommand(CanExecute = nameof(HasImages))]
        private void ChangeSizeMode(GifSizeMode mode)
        {
            OutputSizeMode = mode;

            switch (mode)
            {
                case GifSizeMode.FirstImage:
                    {
                        OutputWidth = Images[0].Width;
                        OutputHeight = Images[0].Height;
                    }
                    break;

                case GifSizeMode.Preset:
                    {
                        if(SelectedPresetSize is null) return;

                        OutputWidth = SelectedPresetSize.Width;
                        OutputHeight = SelectedPresetSize.Height;
                    }
                    break;

            }
        }

        [RelayCommand(CanExecute = nameof(CanPlayPreview))]
        private void PlayPreview()
        {
            if (Images.Count == 0) return;

            // 已經播放到最後一張，再按播放就重新從第一張開始
            if (CurrentPreviewIndex >= Images.Count - 1)
            {
                CurrentPreviewIndex = 0;
                playPreviewCount = 0;
            }

            ShowPreviewImage(CurrentPreviewIndex);

            _previewTimer.Start();

            IsPreviewPlaying = true;
        }

        /// <summary>
        /// 暫停預覽
        /// </summary>
        [RelayCommand(CanExecute = nameof(CanPausePreview))]
        private void PausePreview()
        {
            _previewTimer.Stop();

            IsPreviewPlaying = false;
        }

        /// <summary>
        /// 設定輸出路徑
        /// </summary>
        [RelayCommand]
        private void SetOutputPath()
        {
            OutputPath = _fileDialogService.SaveFile();
        }

        /// <summary>
        /// 更換循環模式
        /// </summary>
        /// <param name="mode"></param>
        [RelayCommand]
        private void ChangeLoopMode(GifLoopMode mode)
        {
            OutputLoopMode = mode;
        }

        [RelayCommand(CanExecute = (nameof(CanCreateGif)))]
        private void CreateGif()
        {
            _imageService.CreateGif(Images.Select(x => x.ImageMD), new GifOutputOptionsModel
            {
                Width = OutputWidth,
                Height = OutputHeight,
                OutputPath = OutputPath,
                LoopCount = OutputLoopMode == GifLoopMode.Infinite ? 0 : CustomLoopCount
            });

             _dialogService.ShowMessage("產生 GIF", "GIF 已成功產生。");

        }

        #endregion

        #region /*--- Can Execute ---*/

        /// <summary>
        /// 列表中是否有圖片
        /// </summary>
        /// <returns></returns>
        private bool HasImages()
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

        /// <summary>
        /// 控制啟用上移圖片按鈕
        /// </summary>
        /// <returns></returns>
        private bool CanMoveImageUp()
        {
            if (SelectedImage is null)
                return false;

            return Images.IndexOf(SelectedImage) > 0;
        }

        /// <summary>
        /// 控制啟用下移圖片按鈕
        /// </summary>
        /// <returns></returns>
        private bool CanMoveImageDown()
        {
            if(SelectedImage is null) 
                return false;

            var index = Images.IndexOf(SelectedImage);

            return index >= 0 && index < Images.Count - 1;
        }
        
        /// <summary>
        /// 控制啟用播放按鈕
        /// </summary>
        /// <returns></returns>
        private bool CanPlayPreview()
        {
            return Images.Count >= 2 && !IsPreviewPlaying;
        }

        /// <summary>
        /// 控制啟用暫停按鈕
        /// </summary>
        /// <returns></returns>
        private bool CanPausePreview()
        {
            return IsPreviewPlaying;
        }


        private bool CanCreateGif()
        {
            return Images.Count > 0 && !string.IsNullOrWhiteSpace(OutputPath);
        }

        #endregion


        public MainViewModel(IFileDialogService fileDialogService, IImageService imageService, IDialogService dialogService)
        {
            _fileDialogService = fileDialogService;
            _imageService = imageService;
            _dialogService = dialogService;

            Images.CollectionChanged += Images_CollectionChanged;

            ImageItemDropHandler = new ImageItemDropHandler(UpdateImageOrder);
            ImageFileDropHandler = new ImageFileDropHandler(this, _imageService);
            
            _previewTimer = new DispatcherTimer();
            _previewTimer.Tick += PreviewTimer_Tick;
        }

        private void PreviewTimer_Tick(object? sender, EventArgs e)
        {
            if(Images.Count == 0)
            {
                PausePreview();
                return;
            }

            var nexIndex = CurrentPreviewIndex + 1;

            if(nexIndex >= Images.Count)
            {
                // 無限循環
                if(OutputLoopMode == GifLoopMode.Infinite)
                {
                    ShowPreviewImage(0);
                    return;
                }

                playPreviewCount++; //完成一輪

                //已達指定播放次數
                if (playPreviewCount >= CustomLoopCount)
                {
                    PausePreview();
                    return;
                }

                // 未達播放次數，從第一張重新播放
                ShowPreviewImage(0);
                return;
            }

            ShowPreviewImage(nexIndex);
        }

        /// <summary>
        /// 當 mages.Count 改變時，通知 Command 重新執行 CanExecute
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Images_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            ClearAllImagesCommand.NotifyCanExecuteChanged();
            ChangeSizeModeCommand.NotifyCanExecuteChanged();
            PlayPreviewCommand.NotifyCanExecuteChanged();
            CreateGifCommand.NotifyCanExecuteChanged();

            if(Images.Count == 0)
            {
                _previewTimer.Stop();

                CurrentPreviewIndex = 0;
                PreviewImage = null;
                IsPreviewPlaying = false;
            }
        }

        /// <summary>
        /// 移動圖片位置
        /// </summary>
        /// <param name="offset"></param>
        private void MoveImage(int offset)
        {
            if (SelectedImage is null) return;

            int oldIndex = Images.IndexOf(SelectedImage);
            int newIndex = oldIndex + offset;

            if (newIndex < 0 || newIndex >= Images.Count) return;

            Images.Move(oldIndex, newIndex);

            UpdateImageOrder();

            //因為 SelectedImage 的位置改變了，所以重新檢查「上移 / 下移」是否可以執行。
            MoveImageUpCommand.NotifyCanExecuteChanged();
            MoveImageDownCommand.NotifyCanExecuteChanged();
        }

        /// <summary>
        /// 更新集合順序
        /// </summary>
        private void UpdateImageOrder()
        {
            for(int i = 0; i < Images.Count; i++)
            {
                Images[i].Order = i + 1;
            }

            OutputWidth = Images[0].Width;
            OutputHeight = Images[0].Height;
        }


        public void AddImages(IEnumerable<string> fullPaths)
        {
            foreach(var path in fullPaths)
            {
                if (!_imageService.IsSupportedImage(path)) continue;

                var model = _imageService.GetImageInfo(path, 64);
               
                Images.Add(new ImageItemViewModel(model));
            }

            UpdateImageOrder();
        }

        /// <summary>
        /// 顯示指定的預覽圖片
        /// </summary>
        /// <param name="index">欲顯示那張圖的 index</param>
        private void ShowPreviewImage(int index)
        {
            if (index < 0 || index >= Images.Count) return;

            CurrentPreviewIndex = index;

            var image = Images[index];

            PreviewImage = _imageService.LoadImage(image.Path);

            _previewTimer.Interval = TimeSpan.FromMilliseconds(image.Delay);
        }
    }
}
