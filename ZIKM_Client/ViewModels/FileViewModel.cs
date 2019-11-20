using Avalonia;
using MessageBox.Avalonia;
using ReactiveUI;
using System;
using System.Linq;
using ZIKM_Client.Infrastructure;
using ZIKM_Client.Interfaces;
using ZIKM_Client.Services;

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

        public ReactiveCommand ReadCommand { get; }
        public ReactiveCommand WriteCommand { get; }
        public ReactiveCommand EditCommand { get; }
        public ReactiveCommand CloseCommand { get; }

        public FileViewModel()
        {
            Session = SessionData.SessionId;
            Provider = SessionData.Provider;
            EditMode = false;
            ReadCommand = ReactiveCommand.Create(Read);
            WriteCommand = ReactiveCommand.Create(Write);
            EditCommand = ReactiveCommand.Create(Edit);
            CloseCommand = ReactiveCommand.Create(Close);
        }

        private void Read()
        {
            Provider.SendRequest(new RequestData(Session, (int)FileOperation.Read, ""));
            var response = Provider.GetResponse();
            if (response.Code == 0)
            {
                Text = response.Message;
            }
            else
                new MessageBoxWindow("File", response.Message).Show();
        }

        private void Write()
        {
            Provider.SendRequest(new RequestData(Session, (int)FileOperation.Write, Text));
            Text = "";
            new MessageBoxWindow("File", Provider.GetResponse().Message).Show();
        }

        private void Edit()
        {
            Provider.SendRequest(new RequestData(Session, (int)FileOperation.Edit, EditMode ? Text : ""));
            if (editMode)
            {
                Text = "";
                new MessageBoxWindow("File", Provider.GetResponse().Message).Show();
                EditMode = !EditMode;
            }
            else
            {
                var response = Provider.GetResponse();
                if (response.Code == 0)
                {
                    Text = response.Message;
                    EditMode = !EditMode;
                }
                else
                    new MessageBoxWindow("File", response.Message).Show();
            }
        }

        private void Close()
        {
            Provider.SendRequest(new RequestData(Session, (int)FileOperation.Exit, null));
            Provider.GetResponse();
            if (!EditMode)
                Application.Current.Windows.Where(i => i.GetType().Name == "FileWindow").FirstOrDefault().Close();
            else
            {
                Text = "";
                EditMode = !EditMode;
            }
        }
    }
}
