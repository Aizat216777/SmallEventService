
namespace Project
{
    public interface IEventService
    {
        void TrackEvent(string type, string data);
    }
}