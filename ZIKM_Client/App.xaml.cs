using Avalonia;
using Avalonia.Markup.Xaml;

namespace ZIKM_Client
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
