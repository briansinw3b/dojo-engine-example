using System.Collections;
using System.Collections.Generic;
using Dojo;
using Dojo.Starknet;
using NOOD;
using UnityEngine;

namespace Game
{
    public class EntityManager : MonoBehaviorInstance<EntityManager>
    {
        [SerializeField] private WorldManager _worldManager;

        public T GetModel<T>(string hexCode, uint gameId) where T : ModelInstance
        {
            GameObject[] gameObjects = _worldManager.Entities();            
            foreach(GameObject g in gameObjects)
            {
                if(g.TryGetComponent<T>(out T model))
                {
                    FieldElement entityId =(FieldElement) model.Model.Members["entityId"].value;
                    uint _gameId = (uint)model.Model.Members["gameId"].value;
                    if(NoodyCustomCode.CompareHexStrings(hexCode, entityId.Hex()) && _gameId == gameId)
                    {
                        return (T)model;
                    }
                }
            }
            Debug.LogWarning("can't find model " + typeof(T).Name);
            return null;
        }
    }
}
