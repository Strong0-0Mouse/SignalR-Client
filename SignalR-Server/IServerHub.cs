using System.Threading.Tasks;

namespace ServerSignalR
{
    public interface IServerHub
    {
        Task ReceiveString(string stringValue);
        Task ReceiveDouble(double doubleValue);
        Task ReceiveInt(int intValue);
        Task ReceiveObject(object objectValue);
        Task ReceivePair(int intValue, double doubleValue);
    }
}