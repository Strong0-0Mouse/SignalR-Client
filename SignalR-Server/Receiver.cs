using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Timer = System.Timers.Timer;

namespace ServerSignalR
{
    public class Receiver : IHostedService
    {
        private readonly IHubContext<ServerHub, IServerHub> _hubContext;
        private Timer? _samplingTimer;

        public Receiver(IHubContext<ServerHub, IServerHub> hubContext)
        {
            _hubContext = hubContext;
        }

        private async void SamplingTimerOnElapsed(object? sender, ElapsedEventArgs e)
        {
            await _hubContext.Clients.All.ReceiveString("Hello, World!");
            await _hubContext.Clients.All.ReceiveInt(10);
            await _hubContext.Clients.All.ReceiveDouble(10.123);
            await _hubContext.Clients.All.ReceiveObject(new SentObject {IntValue = 5, DoubleValue = 5.25});
            await _hubContext.Clients.All.ReceivePair(20, 20.6);
        }
        
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _samplingTimer = new Timer(1000);
            _samplingTimer.Elapsed += SamplingTimerOnElapsed;
            _samplingTimer.Start();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _samplingTimer?.Dispose();
            return Task.CompletedTask;
        }
    }
}