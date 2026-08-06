namespace GifComposerWpf.Services.Interfaces
{
    public interface IFileDialogService
    {
        string[] OpenFiles(string title, string filter);
    }
}
