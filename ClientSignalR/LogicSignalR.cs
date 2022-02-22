using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ClientSignalR.Annotations;

namespace ClientSignalR
{
    public sealed class LogicSignalR : INotifyPropertyChanged
    {
        private ObservableCollection<Connection> _connections = new();
        
        public static int NumberConnection { get; set; }
        public static int NumberLastConnection { get; set; }
        public static int CurrentConnection { get; set; }
        public static bool IsConnectOpened { get; set; }
        
        public ObservableCollection<Connection> Connections
        {
            get => _connections;
            set
            {
                _connections = value; OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}