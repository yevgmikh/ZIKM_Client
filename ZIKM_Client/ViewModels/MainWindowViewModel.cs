using Avalonia;
using Avalonia.Controls;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using ZIKM_Client.Infrastructure;
using ZIKM_Client.Interfaces;
using ZIKM_Client.Views;

namespace ZIKM_Client.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private List<string> nameList;
        private string seletedName;

        public List<string> NameList { get => nameList; set => this.RaiseAndSetIfChanged(ref nameList, value); }
        public string SeletedName { get => seletedName; set => this.RaiseAndSetIfChanged(ref seletedName, value); }

        public Guid Session { get; private set; }
        public IProvider Provider { get; private set; }

        //public MainWindowViewModel() { }

        public MainWindowViewModel(Guid session, IProvider provider)
        {
            Session = session;
            Provider = provider;
            OnClickCommand = ReactiveCommand.Create(() => { /* do something */ });
            //new LoginWindow().ShowDialog(Application.Current.MainWindow);
            Provider?.SendRequest(new RequestData(Session, (int)MainOperation.GetFolders, ""));
            ResponseData? response = Provider?.GetResponse();
            NameList = new List<string>(response?.Message.Split(';'));
        }

        public ReactiveCommand OnClickCommand { get; }

        public void Open()
        {
            Provider?.SendRequest(new RequestData(Session, (int)MainOperation.OpenFolder, SeletedName));
            ResponseData? response = Provider?.GetResponse();
            if (response?.Code == 0)
            {
                Provider?.SendRequest(new RequestData(Session, (int)MainOperation.GetFiles, ""));
                response = Provider?.GetResponse();
                NameList = new List<string>(response?.Message.Split(';'));
            }
        }

        public void Close()
        {
            Provider?.SendRequest(new RequestData(Session, (int)MainOperation.CloseFolder, ""));
            ResponseData? response = Provider?.GetResponse();
            if (response?.Code == 0)
            {
                Provider?.SendRequest(new RequestData(Session, (int)MainOperation.GetFolders, ""));
                response = Provider?.GetResponse();
                NameList = new List<string>(response?.Message.Split(';'));
            }
        }

        public void LogOut()
        {
            Provider.SendRequest(new RequestData(Session, 6, ""));
            var response = Provider.GetResponse();
            Provider?.Dispose();
            Application.Current.MainWindow.Close();
        }
    }
}
