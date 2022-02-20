using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ClientSignalR.Annotations;
using Microsoft.AspNetCore.SignalR.Client;

namespace ClientSignalR
{
    public sealed class Connection : INotifyPropertyChanged
    {
        private ObservableCollection<Message> _messages = new();
        
        public event PropertyChangedEventHandler? PropertyChanged;
        public int NumConnection { get; set; }
        public string Url { get; set; } = string.Empty;
        public HubConnection? HubConnection { get; set; }
        public ObservableCollection<Message> Messages
        {
            get => _messages;
            set
            {
                _messages = value; OnPropertyChanged();
            }
        }

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}