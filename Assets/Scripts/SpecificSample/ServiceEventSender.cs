using UnityEngine;
using VContainer;
using EasyButtons;

namespace Project
{
    public class ServiceEventSender : MonoBehaviour
    {
        private IEventService eventService;
        [Inject]
        public void Construct(IEventService service)
        {
            eventService = service;
        }
        [Button]
        public void RandomTrackEvent() => TrackEvent("t_" + Random.Range(0, 9999), "d_" + Random.Range(0, 9999));
        [Button]
        public void TrackEvent(string type, string data)
        {
            if (eventService != null) 
            {
                eventService.TrackEvent(type, data);
            }
        }
    }
}