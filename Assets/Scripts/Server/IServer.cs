
namespace Project
{
    public interface IServer 
    {
        public enum eResponseType
        {
            OK, 
            Error,
        }
        event ServerEvents.ResponseEvent OnResponse;
        void SendEvent(string serverUrl, string id, string data);
    }
    public class ServerEvents
    {
        public delegate void ResponseEvent(string id, IServer.eResponseType responceType);
    }
}