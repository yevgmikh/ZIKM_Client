using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using ReactiveUI;
using System.Linq;
using System.Reactive;
using ZIKM.Infrastructure.DataStructures;
using ZIKM.Infrastructure.Enums;

namespace ZIKM.Client.ViewModels {
    class FileWindowViewModel : ViewModelBase {
        private bool editMode;
        private string exitButton;
        private string text;

        public bool EditMode {
            get => editMode; set {
                this.RaiseAndSetIfChanged(ref editMode, value);
                if (value)
                    ExitButton = "Cancel";
                else
                    ExitButton = "Close";
            }
        }
        public string ExitButton { get => exitButton; set => this.RaiseAndSetIfChanged(ref exitButton, value); }
        public string Text { get => text; set => this.RaiseAndSetIfChanged(ref text, value); }

        public ReactiveCommand<Unit, Unit> ReadCommand { get; }
        public ReactiveCommand<Unit, Unit> WriteCommand { get; }
        public ReactiveCommand<Unit, Unit> EditCommand { get; }
        public ReactiveCommand<Unit, Unit> CloseCommand { get; }

        public FileWindowViewModel() {
            EditMode = false;
            ReadCommand = ReactiveCommand.Create(Read);
            WriteCommand = ReactiveCommand.Create(Write);
            EditCommand = ReactiveCommand.Create(Edit);
            CloseCommand = ReactiveCommand.Create(Close);
        }

        private void Read() {
            Provider.SendRequest(new RequestData((int)FileOperation.Read, ""));
            var response = Provider.GetResponse();
            if (response.Code == 0)
            {
                Text = response.Message;
            }
            else
                MessageBox.Avalonia.MessageBoxManager.GetMessageBoxStandardWindow("File", response.Message).Show();
        }

        private void Write() {
            Provider.SendRequest(new RequestData((int)FileOperation.Write, Text));
            Text = "";
            MessageBox.Avalonia.MessageBoxManager.GetMessageBoxStandardWindow("File", Provider.GetResponse().Message).Show();
        }

        private void Edit() {
            Provider.SendRequest(new RequestData((int)FileOperation.Edit, EditMode ? Text : ""));
            if (editMode) {
                Text = "";
                MessageBox.Avalonia.MessageBoxManager.GetMessageBoxStandardWindow("File", Provider.GetResponse().Message).Show();
                EditMode = !EditMode;
            }
            else {
                var response = Provider.GetResponse();
                if (response.Code == 0)
                {
                    Text = response.Message;
                    EditMode = !EditMode;
                }
                else
                    MessageBox.Avalonia.MessageBoxManager.GetMessageBoxStandardWindow("File", response.Message).Show();
            }
        }

        private void Close() {
            Provider.SendRequest(new RequestData((int)FileOperation.Exit, null));
            Provider.GetResponse();
            if (!EditMode)
                ((IClassicDesktopStyleApplicationLifetime)Application.Current.ApplicationLifetime)
                    .Windows.Where(i => i.GetType().Name == "FileWindow").FirstOrDefault().Close();
            else {
                Text = "";
                EditMode = !EditMode;
            }
        }
    }
}
