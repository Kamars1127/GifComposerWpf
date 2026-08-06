using GifComposerWpf.Services.Interfaces;
using Microsoft.Win32;

namespace GifComposerWpf.Services
{
    public class FileDialogService : IFileDialogService
    {
        public string[] OpenFiles(string title, string filter)
        {
            var dialog = new OpenFileDialog
            {
                Title = title,
                Filter = filter,
                Multiselect = true
            };

            return dialog.ShowDialog() == true ? dialog.FileNames : Array.Empty<string>();
        }
    }
}
