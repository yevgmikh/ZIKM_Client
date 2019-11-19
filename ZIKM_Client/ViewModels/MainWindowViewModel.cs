using Avalonia;
using Avalonia.Controls;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using ZIKM_Client.Infrastructure;
using ZIKM_Client.Interfaces;
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

        //public MainWindowViewModel() { }

        public MainWindowViewModel(Guid session, IProvider provider)
        {
            Session = session;
            Provider = provider;
            OnClickCommand = ReactiveCommand.Create(() => { /* do something */ });
            //new LoginWindow().ShowDialog(Application.Current.MainWindow);
            Provider?.SendRequest(new RequestData(Session, (int)MainOperation.GetAll, ""));
            ResponseData? response = Provider?.GetResponse();
            SetList(response?.Message);
        }

        public ReactiveCommand OnClickCommand { get; }

        private void SetList(string data)
        {
            nameType = new Dictionary<string, string>();
            foreach (var obj in data.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
                nameType.Add(obj.Split(':')[1], obj.Split(':')[0]);
            NameList = new List<string>(nameType.Keys);
        }

        public async void Open()
        {
            if (seletedName != null)
            {
                ResponseData? response;
                switch (nameType[seletedName])
                {
                    case "file":
                        Provider?.SendRequest(new RequestData(Session, (int)MainOperation.OpenFile, SeletedName));
                        response = Provider?.GetResponse();
                        if (response?.Code == 0)
                        {
                            var window = new FileWindow()
                            {
                                DataContext = new FileViewModel(Session, Provider)
                            };
                            await window.ShowDialog(Application.Current.Windows.Where(i => i.GetType().Name == "MainWindow").FirstOrDefault());
                        }
                        break;
                    case "folder":
                        Provider?.SendRequest(new RequestData(Session, (int)MainOperation.OpenFolder, SeletedName));
                        response = Provider?.GetResponse();
                        if (response?.Code == 0)
                        {
                            Provider?.SendRequest(new RequestData(Session, (int)MainOperation.GetAll, ""));
                            response = Provider?.GetResponse();
                            SetList(response?.Message);
                        }
                        break;
                }
            }
            
            
        }

        public void Close()
        {
            Provider?.SendRequest(new RequestData(Session, (int)MainOperation.CloseFolder, ""));
            ResponseData? response = Provider?.GetResponse();
            if (response?.Code == 0)
            {
                Provider?.SendRequest(new RequestData(Session, (int)MainOperation.GetAll, ""));
                response = Provider?.GetResponse();
                SetList(response?.Message);
            }
        }

        public void LogOut()
        {
            Provider.SendRequest(new RequestData(Session, (int)MainOperation.EndSession, ""));
            var response = Provider.GetResponse();
            //Provider?.Dispose();
            Application.Current.Windows.Where(i => i.GetType().Name == "MainWindow").FirstOrDefault().Close();
        }
    }
}
