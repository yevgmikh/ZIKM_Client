using Avalonia;
using Avalonia.Media.Imaging;
using MessageBox.Avalonia;
using ReactiveUI;
using System;
using System.Linq;
using System.Reactive;
using ZIKM_Client.Interfaces;
using ZIKM_Client.Services;
using ZIKM_Client.Sevices;
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
        private IProvider provider;

        public string IpAddress { get => ipAddress; set => this.RaiseAndSetIfChanged(ref ipAddress, value); }
        public string Port { get => port; set => this.RaiseAndSetIfChanged(ref port, value); }
        public string Login { get => login; set => this.RaiseAndSetIfChanged(ref login, value); }
        public string Password { get => password; set => this.RaiseAndSetIfChanged(ref password, value); }
        public string Code { get => code; set => this.RaiseAndSetIfChanged(ref code, value); }
        public IBitmap Captcha { get => captcha; set => this.RaiseAndSetIfChanged(ref captcha, value); }
        public IProvider Provider { get => provider; private set => this.RaiseAndSetIfChanged(ref provider, value); }

        public ReactiveCommand<Unit, Unit> ConnectCommand { get; }
        public ReactiveCommand<Unit, Unit> LoginCommand { get; }
        public ReactiveCommand<Unit, Unit> ExitCommand { get; }

        public LoginViewModel()
        {
            ConnectCommand = ReactiveCommand.Create(Connect);
            LoginCommand = ReactiveCommand.Create(LogIn);
            ExitCommand = ReactiveCommand.Create(Exit);
        }

        private async void Connect()
        {
            try
            {
                Provider = new TCPProvider(IpAddress, int.Parse(Port));
                Captcha = Provider.GetCaptcha();
            }
            catch (Exception ex)
            {
                await new MessageBoxWindow("Login", ex.Message).Show();
            }
        }

        private async void LogIn()
        {
            try
            {
                var response = Provider.SendLoginRequest(new Infrastructure.LoginData(Login, Password, Code));
                await new MessageBoxWindow("Login", response.Message).Show();
                if (response.Code == 0)
                {
                    SessionData.SessionId = response.SessionId;
                    SessionData.Provider = Provider;
                    MainWindow window = new MainWindow();
                    await window.ShowDialog(Application.Current.Windows.Where(i => i.GetType().Name == "LoginWindow").FirstOrDefault());
                }
                Captcha = Provider.GetCaptcha();
            }
            catch (Exception ex)
            {
                await new MessageBoxWindow("Login", ex.Message).Show();
            }
        }

        private void Exit()
        {
            Provider?.Dispose();
            Application.Current.Exit();
        }
    }
}
