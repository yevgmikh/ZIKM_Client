using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using ZIKM.Client.ViewModels;
using ZIKM.Client.Views;

namespace ZIKM.Client {
    public class App : Application {
        public override void Initialize() {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted() {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop) {
                desktop.MainWindow = new LoginWindow {
                    //DataContext = new LoginWindowViewModel(),
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}
