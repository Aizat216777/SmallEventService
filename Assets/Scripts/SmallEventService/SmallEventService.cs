using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Project 
{
    public class SmallEventService : IEventService
    {
        private IServer server;
        private ServiceConfig serviceConfig;
        private SentEventsContainer sentEventsContainer;
        private EventDataContainer currentEvents;
        private Tween delayToSendEventTween;
        public SmallEventService(IServer server, ServiceConfig serviceConfig) 
        {
            this.serviceConfig = serviceConfig;
            this.server = server;
            this.server.OnResponse += ServerResponceCallback;

            if (PlayerPrefs.HasKey(nameof(sentEventsContainer)))
            {
                sentEventsContainer = JsonUtility.FromJson<SentEventsContainer>(PlayerPrefs.GetString(nameof(sentEventsContainer)));
            }
            if (PlayerPrefs.HasKey(nameof(currentEvents)))
            {
                currentEvents = JsonUtility.FromJson<EventDataContainer>(PlayerPrefs.GetString(nameof(currentEvents)));
            }
            if(sentEventsContainer == null)
            {
                sentEventsContainer = new SentEventsContainer();
            }
            if (currentEvents == null)
            {
                currentEvents = new EventDataContainer();
            }

            if (currentEvents.events.Count > 0 ||
                sentEventsContainer.sentEvents.Count > 0)
            {
                while (sentEventsContainer.sentEvents.Count > 0)
                {
                    currentEvents.events.AddRange(sentEventsContainer.sentEvents[0].eventDataContainer.events);
                    sentEventsContainer.sentEvents.RemoveAt(0);
                }
                if (serviceConfig.log)
                {
                    Debug.Log("SendOldEvents");
                }
                SendEventsWithCooldown();
                Save();
            }
        }
        ~SmallEventService()
        {
            server.OnResponse -= ServerResponceCallback;
        }
        private void ServerResponceCallback(string id, IServer.eResponseType responceType)
        {
            if (serviceConfig.log)
            {
                Debug.LogFormat("Responce {0}, {1}", id, responceType);
            }
            for (int i = 0; i < sentEventsContainer.sentEvents.Count; i++)
            {
                if (sentEventsContainer.sentEvents[i].ID == id)
                {
                    if(responceType == IServer.eResponseType.Error)
                    {
                        currentEvents.events.AddRange(sentEventsContainer.sentEvents[i].eventDataContainer.events);
                    }
                    sentEventsContainer.sentEvents.RemoveAt(i);
                    Save(); 
                    if (currentEvents.events.Count > 0)
                    {
                        SendEventsWithCooldown();
                    }
                    break;
                }
            }
        }
        public void TrackEvent(string type, string data)
        {
            currentEvents.events.Add(new EventData(type, data));
            if (serviceConfig.log)
            {
                Debug.LogFormat("added new track event {0}, {1}", type, data);
            }
            Save();
            SendEventsWithCooldown();
        }
        private void SendEventsWithCooldown()
        {
            if(delayToSendEventTween == null)
            {
                delayToSendEventTween = DOVirtual.DelayedCall(
                    serviceConfig.cooldownBeforeSend,
                    () =>
                    {
                        SentEvents sentEvents = new SentEvents(currentEvents);
                        sentEventsContainer.sentEvents.Add(sentEvents);
                        currentEvents = new EventDataContainer();
                        Save();
                        if (serviceConfig.log)
                        {
                            Debug.LogFormat("SendEvent {0}, {1}, {2}", serviceConfig.serverUrl, sentEvents.ID, sentEvents.Data);
                        }
                        server.SendEvent(serviceConfig.serverUrl, sentEvents.ID, sentEvents.Data);
                        delayToSendEventTween = null;
                    });
            }
        }
        private void Save()
        {
            PlayerPrefs.SetString(nameof(sentEventsContainer), JsonUtility.ToJson(sentEventsContainer));
            PlayerPrefs.SetString(nameof(currentEvents), JsonUtility.ToJson(currentEvents));
        }

        [System.Serializable]
        private class SentEventsContainer
        {
            public List<SentEvents> sentEvents = new List<SentEvents>();
        }
        [System.Serializable]
        private class SentEvents
        {
            public EventDataContainer eventDataContainer;
            [SerializeField]
            private string id;
            public string ID => id;
            public string Data => JsonUtility.ToJson(eventDataContainer);
            public SentEvents(EventDataContainer eventDataContainer)
            {
                this.eventDataContainer = eventDataContainer;
                id = System.Guid.NewGuid().ToString();
            }            
        }
        [System.Serializable]
        private class EventDataContainer
        {
            public List<EventData> events = new List<EventData>();
        }
        [System.Serializable]
        private class EventData
        {
            public string type;
            public string data;
            public EventData(string type, string data)
            {
                this.type = type;
                this.data = data;
            }
        }
    }
}