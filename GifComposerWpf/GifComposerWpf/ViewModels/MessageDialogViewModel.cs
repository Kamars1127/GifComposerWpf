using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace GifComposerWpf.ViewModels
{
    public partial class MessageDialogViewModel : ObservableObject
    {
        public event Action<bool>? CloseRequested;

        /// <summary>
        /// 視窗標題
        /// </summary>
        [ObservableProperty]
        private string title = string.Empty;

        /// <summary>
        /// 訊息
        /// </summary>
        [ObservableProperty]
        private string message = string.Empty;

        [RelayCommand]
        private void Ok()
        {
            CloseRequested?.Invoke(true);
        }
    }
}
