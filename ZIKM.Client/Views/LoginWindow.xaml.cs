using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ZIKM.Client.Views {
    public class LoginWindow : Window {
        public LoginWindow() {
            this.InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
