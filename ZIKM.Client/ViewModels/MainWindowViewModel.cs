using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using ZIKM.Client.Views;
using ZIKM.Infrastructure.DataStructures;
using ZIKM.Infrastructure.Enums;

namespace ZIKM.Client.ViewModels {
    public class MainWindowViewModel : ViewModelBase {
        private Dictionary<string, string> nameType;
        private List<string> nameList;
        private string seletedName;

        public List<string> NameList { get => nameList; set => this.RaiseAndSetIfChanged(ref nameList, value); }
        public string SeletedName { get => seletedName; set => this.RaiseAndSetIfChanged(ref seletedName, value); }

        public ReactiveCommand<Unit, Unit> OpenCommand { get; }
        public ReactiveCommand<Unit, Unit> CloseCommand { get; }
        public ReactiveCommand<Unit, Unit> LogOutCommand { get; }

        public MainWindowViewModel() {
            OpenCommand = ReactiveCommand.Create(Open);
            CloseCommand = ReactiveCommand.Create(Close);
            LogOutCommand = ReactiveCommand.Create(LogOut);

            // Get data from root folder
            Provider.SendRequest(new RequestData((int)MainOperation.GetAll, ""));
            ResponseData response = Provider.GetResponse();
            SetList(response.Message);
        }

        private void SetList(string data) {
            nameType = new Dictionary<string, string>();
            foreach (var obj in data.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
                nameType.Add(obj.Split(':')[1], obj.Split(':')[0]);
            NameList = new List<string>(nameType.Keys);
        }

        private async void Open() {
            if (seletedName != null) {
                ResponseData response;
                switch (nameType[seletedName]) {
                    case "file":
                        Provider.SendRequest(new RequestData( (int)MainOperation.OpenFile, SeletedName));
                        response = Provider.GetResponse();
                        if (response.Code == 0) {
                            var window = new FileWindow();
                            await window.ShowDialog(((IClassicDesktopStyleApplicationLifetime)Application.Current.ApplicationLifetime)
                                .Windows.Where(i => i.GetType().Name == "MainWindow").FirstOrDefault());
                        }
                        else
                            await MessageBox.Avalonia.MessageBoxManager.GetMessageBoxStandardWindow("Main", response.Message).Show();
                        break;
                    case "folder":
                        Provider.SendRequest(new RequestData((int)MainOperation.OpenFolder, SeletedName));
                        response = Provider.GetResponse();
                        if (response.Code == 0) {
                            Provider?.SendRequest(new RequestData((int)MainOperation.GetAll, ""));
                            response = Provider.GetResponse();
                            SetList(response.Message);
                        }
                        else
                            await MessageBox.Avalonia.MessageBoxManager.GetMessageBoxStandardWindow("Main", response.Message).Show();
                        break;
                }
            }
        }

        private void Close() {
            Provider.SendRequest(new RequestData((int)MainOperation.CloseFolder, ""));
            ResponseData response = Provider.GetResponse();
            if (response.Code == 0) {
                Provider.SendRequest(new RequestData((int)MainOperation.GetAll, ""));
                response = Provider.GetResponse();
                SetList(response.Message);
            }
            else
                MessageBox.Avalonia.MessageBoxManager.GetMessageBoxStandardWindow("Main", response.Message).Show();
        }

        private void LogOut() {
            Provider.SendRequest(new RequestData((int)MainOperation.EndSession, ""));
            var response = Provider.GetResponse();
            MessageBox.Avalonia.MessageBoxManager.GetMessageBoxStandardWindow("Main", response.Message).Show();
            ((IClassicDesktopStyleApplicationLifetime)Application.Current.ApplicationLifetime)
                .Windows.Where(i => i.GetType().Name == "MainWindow").FirstOrDefault().Close();
        }
    }
}
