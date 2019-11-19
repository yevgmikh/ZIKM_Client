using Avalonia;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ZIKM_Client.Interfaces;
using ZIKM_Client.Views;

namespace ZIKM_Client.ViewModels
{
    class LoginViewModel : ViewModelBase
    {
        private string ipAddress = "192.168.31.190";
        private string port = "8000";
        private string login = "Senpai";
        private string password = "hmmm";
        private string code;
        private IBitmap captcha;

        public string IpAddress { get => ipAddress; set => this.RaiseAndSetIfChanged(ref ipAddress, value); }
        public string Port { get => port; set => this.RaiseAndSetIfChanged(ref port, value); }
        public string Login { get => login; set => this.RaiseAndSetIfChanged(ref login, value); }
        public string Password { get => password; set => this.RaiseAndSetIfChanged(ref password, value); }
        public string Code { get => code; set => this.RaiseAndSetIfChanged(ref code, value); }
        public IBitmap Captcha { get => captcha; set => this.RaiseAndSetIfChanged(ref captcha, value); }

        public IProvider Provider { get; private set; }

        public LoginViewModel()
        {
            Provider = new TCPProvider(IpAddress, int.Parse(Port));
            Captcha = Provider.GetCaptcha();
        }

        public void LogIn()
        {
            try
            {
                var response = Provider.SendLoginRequest(new Infrastructure.LoginData(Login, Password, Code));
                if (response.Code == 0)
                {
                    //Provider.SendRequest(new Infrastructure.RequestData(response.SessionId, 6, ""));
                    //response = Provider.GetResponse();
                    MainWindow window = new MainWindow()
                    {
                        DataContext = new MainWindowViewModel(response.SessionId, Provider)
                    };
                    //((MainWindowViewModel)Application.Current.MainWindow.DataContext).Session = response.SessionId;
                    //((MainWindowViewModel)Application.Current.MainWindow.DataContext).Provider = Provider;
                    //Application.Current.Windows.Where(i => i.GetType().Name == "LoginWindow").FirstOrDefault().Close();
                    window.ShowDialog(Application.Current.Windows.Where(i => i.GetType().Name == "LoginWindow").FirstOrDefault());
                }
                else
                {
                    Login = response.Message;
                    Captcha = Provider.GetCaptcha();

                }
            }
            catch (Exception ex)
            {
                Login = ex.Message;
            }
        }
    }
}
