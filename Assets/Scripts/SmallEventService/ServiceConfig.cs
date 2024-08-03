using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project
{
    [System.Serializable]
    public class ServiceConfig 
    {
        public string serverUrl;
        public float cooldownBeforeSend;
        public bool log;
    }
}