using GifComposerWpf.Services;
using GifComposerWpf.Services.Interfaces;
using GifComposerWpf.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System.Configuration;
using System.Data;
using System.Windows;

namespace GifComposerWpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public IServiceProvider? Services { get; private set; } = null;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            
            var services = new ServiceCollection(); //建立 DI 註冊清單
            ConfigureServices(services); //註冊所有需要的類別

            var options = new ServiceProviderOptions();

#if DEBUG
            //開發期間啟用：提早檢查 DI 註冊與生命週期問題
            options.ValidateOnBuild = true;
            options.ValidateScopes = true;
#endif
            //建置 ServiceProvider（DI 容器）
            Services = services.BuildServiceProvider(options);

            //由 DI 建出主視窗並顯示
            var mainWindow = Services.GetRequiredService<MainWindow>();
            MainWindow = mainWindow;
            mainWindow.Show();

        }

        protected static void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<MainWindow>();  // Views
            services.AddTransient<MainViewModel>(); //ViewModels
            services.AddSingleton<IFileDialogService, FileDialogService>(); //Services

        }

        protected override void OnExit(ExitEventArgs e)
        {
            //釋放由 DI 容器管理的 IDisposable 資源
            if(Services is IDisposable disposable)
            {
                disposable.Dispose();
            }

            base.OnExit(e);
        }
    }

}
