using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "GameManagerData", menuName = "ScriptableObject/GameManagerData")]
    public class GameManagerData : ScriptableObject
    {
        public string privateKey;
        public string accountAddress;
    }
}
