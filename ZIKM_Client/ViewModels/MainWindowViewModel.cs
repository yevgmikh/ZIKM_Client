using Avalonia;
using MessageBox.Avalonia;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using ZIKM_Client.Infrastructure;
using ZIKM_Client.Interfaces;
using ZIKM_Client.Services;
using ZIKM_Client.Views;

namespace ZIKM_Client.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private Dictionary<string, string> nameType;
        private List<string> nameList;
        private string seletedName;

        public List<string> NameList { get => nameList; set => this.RaiseAndSetIfChanged(ref nameList, value); }
        public string SeletedName { get => seletedName; set => this.RaiseAndSetIfChanged(ref seletedName, value); }

        public Guid Session { get; private set; }
        public IProvider Provider { get; private set; }

        public ReactiveCommand OpenCommand { get; }
        public ReactiveCommand CloseCommand { get; }
        public ReactiveCommand LogOutCommand { get; }

        public MainWindowViewModel()
        {
            Session = SessionData.SessionId;
            Provider = SessionData.Provider;
            OpenCommand = ReactiveCommand.Create(Open);
            CloseCommand = ReactiveCommand.Create(Close);
            LogOutCommand = ReactiveCommand.Create(LogOut);

            // Get data from root folder
            Provider.SendRequest(new RequestData(Session, (int)MainOperation.GetAll, ""));
            ResponseData response = Provider.GetResponse();
            SetList(response.Message);
        }

        private void SetList(string data)
        {
            nameType = new Dictionary<string, string>();
            foreach (var obj in data.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
                nameType.Add(obj.Split(':')[1], obj.Split(':')[0]);
            NameList = new List<string>(nameType.Keys);
        }

        private async void Open()
        {
            if (seletedName != null)
            {
                ResponseData response;
                switch (nameType[seletedName])
                {
                    case "file":
                        Provider.SendRequest(new RequestData(Session, (int)MainOperation.OpenFile, SeletedName));
                        response = Provider.GetResponse();
                        if (response.Code == 0)
                        {
                            var window = new FileWindow();
                            await window.ShowDialog(Application.Current.Windows.Where(i => i.GetType().Name == "MainWindow").FirstOrDefault());
                        }
                        else
                            await new MessageBoxWindow("Main", response.Message).Show();
                        break;
                    case "folder":
                        Provider.SendRequest(new RequestData(Session, (int)MainOperation.OpenFolder, SeletedName));
                        response = Provider.GetResponse();
                        if (response.Code == 0)
                        {
                            Provider?.SendRequest(new RequestData(Session, (int)MainOperation.GetAll, ""));
                            response = Provider.GetResponse();
                            SetList(response.Message);
                        }
                        else
                            await new MessageBoxWindow("Main", response.Message).Show();
                        break;
                }
            }
        }

        private void Close()
        {
            Provider.SendRequest(new RequestData(Session, (int)MainOperation.CloseFolder, ""));
            ResponseData response = Provider.GetResponse();
            if (response.Code == 0)
            {
                Provider.SendRequest(new RequestData(Session, (int)MainOperation.GetAll, ""));
                response = Provider.GetResponse();
                SetList(response.Message);
            }
            else
                new MessageBoxWindow("Main", response.Message).Show();
        }

        private void LogOut()
        {
            Provider.SendRequest(new RequestData(Session, (int)MainOperation.EndSession, ""));
            var response = Provider.GetResponse();
            new MessageBoxWindow("Main", response.Message).Show();
            Application.Current.Windows.Where(i => i.GetType().Name == "MainWindow").FirstOrDefault().Close();
        }
    }
}
