using DG.Tweening;
using UnityEngine;

namespace Project
{
    public class DummyServer : IServer
    {
        public event ServerEvents.ResponseEvent OnResponse = delegate { };
        public void SendEvent(string serverUrl, string id, string data)
        {
            DOVirtual.DelayedCall(
                Random.Range(2.0f, 5.0f),
                () =>
                {
                    OnResponse(id, Random.value < 0.6f ? IServer.eResponseType.OK : IServer.eResponseType.Error);
                });
        }
    }
}