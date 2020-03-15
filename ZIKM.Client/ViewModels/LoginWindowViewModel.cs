using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media.Imaging;
using ReactiveUI;
using System;
using System.Linq;
using System.Reactive;
using ZIKM.Client.Services.Providers;
using ZIKM.Client.Views;
using ZIKM.Infrastructure.DataStructures;

namespace ZIKM.Client.ViewModels {
    class LoginWindowViewModel : ViewModelBase {
        private string ipAddress = "192.168.31.188";
        private string port = "8000";
        private string login = "Senpai";
        private string password = "hmmm";
        private string code;
        private IBitmap captcha;
        //private IProvider provider;

        public string IpAddress { get => ipAddress; set => this.RaiseAndSetIfChanged(ref ipAddress, value); }
        public string Port { get => port; set => this.RaiseAndSetIfChanged(ref port, value); }
        public string Login { get => login; set => this.RaiseAndSetIfChanged(ref login, value); }
        public string Password { get => password; set => this.RaiseAndSetIfChanged(ref password, value); }
        public string Code { get => code; set => this.RaiseAndSetIfChanged(ref code, value); }
        public IBitmap Captcha { get => captcha; set => this.RaiseAndSetIfChanged(ref captcha, value); }
        //public IProvider Provider { get => provider; private set => this.RaiseAndSetIfChanged(ref provider, value); }

        public ReactiveCommand<Unit, Unit> ConnectCommand { get; }
        public ReactiveCommand<Unit, Unit> LoginCommand { get; }
        public ReactiveCommand<Unit, Unit> ExitCommand { get; }

        public LoginWindowViewModel() {
            ConnectCommand = ReactiveCommand.Create(Connect);
            LoginCommand = ReactiveCommand.Create(LogIn);
            ExitCommand = ReactiveCommand.Create(Exit);
        }

        private async void Connect() {
            try {
                Provider = new TcpProvider(IpAddress, int.Parse(Port));
                Provider.PrepareProtecting();
                Captcha = Provider.GetCaptcha();
            }
            catch (Exception ex) {
                await MessageBox.Avalonia.MessageBoxManager.GetMessageBoxStandardWindow("Login", ex.Message).Show();
            }
        }

        private async void LogIn() {
            try {
                var response = Provider.SendLoginRequest(new LoginData(Login, Password, Code));
                await MessageBox.Avalonia.MessageBoxManager.GetMessageBoxStandardWindow("Login", response.Message).Show();
                if (response.Code == 0){
                    MainWindow window = new MainWindow();
                    await window.ShowDialog(((IClassicDesktopStyleApplicationLifetime)Application.Current.ApplicationLifetime)
                        .Windows.Where(i => i.GetType().Name == "LoginWindow").FirstOrDefault());
                }
                Captcha = Provider.GetCaptcha();
            }
            catch (Exception ex) {
                await MessageBox.Avalonia.MessageBoxManager.GetMessageBoxStandardWindow("Login", ex.Message).Show();
            }
        }

        private void Exit() {
            Provider?.Dispose();
            //Application.Current.Exit();
            ((IClassicDesktopStyleApplicationLifetime)Application.Current.ApplicationLifetime)
                .Windows.Where(i => i.GetType().Name == "LoginWindow").FirstOrDefault().Close();
        }
    }
}
