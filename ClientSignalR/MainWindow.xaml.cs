using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.AspNetCore.SignalR.Client;


namespace ClientSignalR
{
    public partial class MainWindow
    {
        public LogicSignalR Logic = new();
        
        public MainWindow()
        {
            InitializeComponent();
            InitializeEvents();
            DataContext = Logic;
            Disconnect.IsEnabled = false;
        }

        private void InitializeEvents()
        {
            Protocol.SelectionChanged += ChangedProtocol;
            Ip.TextChanged += ChangeUrl;
            Port.ValueChanged += NumberChanged;
            Address.TextChanged += ChangeUrl;
            
            AddUrl.Click += AddUrlOnClick;
            DeleteUrl.Click += DeleteUrlOnClick;
            AddMessage.Click += AddMessageOnClick;
            Connect.Click += ConnectEvent;
            Disconnect.Click += DisconnectEvent;
            
            ListUrls.SelectionChanged += ListUrlsOnSelectionChanged;
            
            if (LogicSignalR.NumberConnection == 0)
                DeleteUrl.IsEnabled = false;
        }

        private void DeleteUrlOnClick(object sender, RoutedEventArgs e)
        {
            if (LogicSignalR.NumberConnection == 0)
                DeleteUrl.IsEnabled = false;
            try
            {
                Logic.Connections.Remove((ListUrls.SelectedItem as Connection)!);
            }
            catch
            {
                LogicSignalR.NumberConnection--;
            }
        }

        private void NumberChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            Url.Text = string.Empty;
            Url.Text += $"{Protocol.Text}{Ip.Text}:{Port.Text}/{Address.Text}";
        }

        private async void DisconnectEvent(object sender, RoutedEventArgs e)
        {
            ListUrls.IsEnabled = true;
            Disconnect.IsEnabled = false;
            LogicSignalR.IsConnectOpened = false;
            Connect.IsEnabled = true;
            await Logic.Connections[LogicSignalR.CurrentConnection].HubConnection!.StopAsync();
        }

        private async void ConnectEvent(object sender, RoutedEventArgs e)
        {
            try
            {
                ListUrls.IsEnabled = false;
                Disconnect.IsEnabled = true;
                LogicSignalR.IsConnectOpened = true;
                await Logic.Connections[LogicSignalR.CurrentConnection].HubConnection!.StartAsync();
                Connect.IsEnabled = false;
            }
            catch (Exception exception)
            {
                try
                {
                    ListUrls.IsEnabled = true;
                    Disconnect.IsEnabled = false;
                    LogicSignalR.IsConnectOpened = false;
                    Connect.IsEnabled = true;
                    (ListUrls.SelectedItems[LogicSignalR.CurrentConnection] as ListBoxItem)!.Background = null;
                }
                catch
                {
                    // ignored
                }
                
                MessageBox.Show($"Ошибка подключения\n{exception}");
            }
        }

        private void ListUrlsOnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var index = Logic.Connections.IndexOf(Logic.Connections.Single(c => c.Id == (e.AddedItems[0] as Connection)!.Id));
            LogicSignalR.CurrentConnection = index;
        }

        private void AddMessageOnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var types = new Type[int.Parse(NumberParametersMessage.Text)];
                for (var i = 0; i < int.Parse(NumberParametersMessage.Text); i++)
                {
                    types[i] = typeof(object);
                }
                
                Logic.Connections[LogicSignalR.CurrentConnection].Messages
                    .Add(new Message {Text = Message.Text, NumberParameters = int.Parse(NumberParametersMessage.Text)});
                Logic.Connections[LogicSignalR.CurrentConnection].HubConnection.On(Message.Text, types,
                    answer =>
                    {
                        foreach (var item in answer)
                        {
                            ResultOutput.Text +=
                                $"{DateTime.Now.TimeOfDay}: [{Logic.Connections[LogicSignalR.CurrentConnection].Url}:" +
                                $" {Logic.Connections[LogicSignalR.CurrentConnection].Id}]" +
                                $" >> {item}\n";
                        }
                        return null;
                    });
            }
            catch (Exception exception)
            {
                MessageBox.Show($"Ошибка при добавлении обработчика сообщения\n{exception}");
            }
        }

        private void AddUrlOnClick(object sender, RoutedEventArgs e)
        {
            DeleteUrl.IsEnabled = true;
            try
            {
                Logic.Connections.Add(new Connection
                {
                    Id = LogicSignalR.NumberLastConnection,
                    Url = Url.Text,
                    HubConnection = new HubConnectionBuilder().WithUrl(Url.Text).Build(),
                    Messages = new ObservableCollection<Message>()
                });
                LogicSignalR.NumberLastConnection++;
                LogicSignalR.NumberConnection++;
            }
            catch (Exception exception)
            {
                MessageBox.Show($"Неверный формат URl\n{exception}");
            }
        }

        private void ChangeUrl(object sender, TextChangedEventArgs e)
        {
            Url.Text = string.Empty;
            Url.Text += $"{Protocol.Text}{Ip.Text}:{Port.Text}/{Address.Text}";
        }

        private void ChangedProtocol(object sender, SelectionChangedEventArgs e)
        {
            Url.Text = Url.Text.Contains("http://")
                ? Url.Text.Replace("http://", "https://")
                : Url.Text.Replace("https://", "http://");
        }
    }
}