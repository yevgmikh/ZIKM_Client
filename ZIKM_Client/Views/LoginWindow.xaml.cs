using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ZIKM_Client.ViewModels;

namespace ZIKM_Client.Views
{
    public class LoginWindow : Window
    {
        public LoginWindow()
        {
            this.InitializeComponent();
            //DataContext = new LoginViewModel();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
