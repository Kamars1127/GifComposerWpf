using GifComposerWpf.ViewModels;
using System.Windows;

namespace GifComposerWpf.Views
{
    /// <summary>
    /// MessageDialogView.xaml 的互動邏輯
    /// </summary>
    public partial class MessageDialogView : Window
    {
        public MessageDialogView(MessageDialogViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;

            vm.CloseRequested += OnCloseRequested;
        }

        private void OnCloseRequested(bool result)
        {
            DialogResult = result;
        }
    }
}
