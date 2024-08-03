using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project
{

    [CreateAssetMenu(fileName = "GameConfig", menuName = "MKGames/GameConfig"), System.Serializable]
    public class GameConfig : ScriptableObject
    {
        public ServiceConfig serviceConfig;
    }
}