using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ZIKM_Client.Views
{
    public class FileWindow : Window
    {
        public FileWindow()
        {
            this.InitializeComponent();
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
