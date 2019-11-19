using Avalonia;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZIKM_Client.Infrastructure;
using ZIKM_Client.Interfaces;

namespace ZIKM_Client.ViewModels
{
    class FileViewModel : ViewModelBase
    {
        private bool editMode;
        private string exitButton;
        private string text;

        public bool EditMode
        {
            get => editMode; set
            {
                this.RaiseAndSetIfChanged(ref editMode, value);
                if (value)
                    ExitButton = "Cancel";
                else
                    ExitButton = "Close";
            }
        }
        public string ExitButton { get => exitButton; set => this.RaiseAndSetIfChanged(ref exitButton, value); }
        public string Text { get => text; set => this.RaiseAndSetIfChanged(ref text, value); }

        public Guid Session { get; private set; }
        public IProvider Provider { get; private set; }

        public FileViewModel(Guid session, IProvider provider)
        {
            Session = session;
            Provider = provider;
            EditMode = false;
        }

        public void Read()
        {
            Provider.SendRequest(new RequestData(Session, (int)FileOperation.Read, ""));
            Text = Provider.GetResponse().Message;
        }

        public void Write()
        {
            Provider.SendRequest(new RequestData(Session, (int)FileOperation.Write, Text));
            Text = Provider.GetResponse().Message;
        }

        public void Edit()
        {
            Provider.SendRequest(new RequestData(Session, (int)FileOperation.Edit, EditMode ? Text : ""));
            Text = Provider.GetResponse().Message;
            EditMode = !EditMode;

        }

        public void Close()
        {
            Provider.SendRequest(new RequestData(Session, (int)FileOperation.Exit, null));
            Text = Provider.GetResponse().Message;
            if (!EditMode)
                Application.Current.Windows.Where(i => i.GetType().Name == "FileWindow").FirstOrDefault().Close();
            else
                EditMode = !EditMode;
        }
    }
}
