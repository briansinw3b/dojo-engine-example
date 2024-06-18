using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NOOD.UI
{
    public class NoodUI : MonoBehaviour
    {
        public static T Create<T>(string path, Transform parent = null) where T : NoodUI
        {
            T prefab = Resources.Load<T>(path);
            T temp = Instantiate(prefab, parent);
            if(temp == null)
            {
                Debug.LogError("Can't find prefab with type " + typeof(T).Name);
            }
            return temp;
        }
        

        public virtual void Open()
        {
            gameObject.SetActive(true);
        }
        public virtual void Close()
        {
            gameObject.SetActive(false);
        }
    }
}
