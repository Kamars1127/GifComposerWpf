using GifComposerWpf.Services.Interfaces;
using GifComposerWpf.ViewModels;
using GifComposerWpf.Views;
using System.Diagnostics;

namespace GifComposerWpf.Services
{
    public class DialogService : IDialogService
    {
        public bool ShowMessage(string title, string message)
        {
            var dialog = new MessageDialogView(new MessageDialogViewModel
            {
                Title = title,
                Message = message
            });

            dialog.Owner = App.Current.MainWindow;


            return dialog.ShowDialog() == true ? true : false;
        }
    }
}
