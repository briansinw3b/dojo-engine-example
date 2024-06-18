using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UpdateObject : MonoBehaviour
{
    public static List<UpdateObject> updateObjects;
    public Func<bool> _func;
    public string _functionName;
    private bool _isPause;

    #region Init
    private static UpdateObject Create(string objectName = "")
    {
        return new GameObject("UpdateObject " + objectName).AddComponent<UpdateObject>();
    }
    public static UpdateObject Create(object target, Func<bool> func, string functionName, bool stopAllWithTheSameName)
    {
        InitIfNeed();

        if(stopAllWithTheSameName)
        {
            StopAllWithName(functionName);
        }

        UpdateObject updateObject = UpdateObject.Create("UpdateObject " + functionName);
        updateObject._func = () => 
        {
            if (target == null) return true;
            else return func.Invoke();
        };
        updateObject._functionName = functionName;

        updateObjects.Add(updateObject);
        return updateObject;
    }
    private static void InitIfNeed()
    {
        if(GameObject.FindAnyObjectByType<UpdateObject>() == false || updateObjects == null)
        {
            updateObjects = new List<UpdateObject>();
        }
    }
    #endregion

    #region Update
    // Update is called once per frame
    void Update()
    {
        if(_isPause == true) return;
        if(_func != null)
        {
            if(_func?.Invoke() == true)
            {
                DestroySelf();
            }
        }
    }
    public void Pause()
    {
        _isPause = true;
    }
    public void Resume()
    {
        _isPause = false;
    }
    public void Stop()
    {
        DestroySelf();
    }
    #endregion

    #region Static Functions
    public static void StopWithName(string functionName)
    {
        InitIfNeed();
        if(updateObjects.First(x => x._functionName == functionName))
        {
            updateObjects.First(x => x._functionName == functionName).DestroySelf();
        }
    }
    public static void StopAllWithName(string functionName)
    {
        InitIfNeed();
        for(int i = 0; i < updateObjects.Count; i++)
        {
            if(updateObjects[i]._functionName == functionName)
            {
                updateObjects[i].DestroySelf();
                i--;
            }
        }
    }
    private static void RemoveFromUpdateObjectList(UpdateObject updateObject)
    {
        if(updateObjects.Contains(updateObject))
            updateObjects.Remove(updateObject);
    }
    #endregion

    #region Destroy
    public void DestroySelf()
    {
        if(this.gameObject != null)
        {
            RemoveFromUpdateObjectList(this);
            Destroy(this.gameObject);
        }
    }
    #endregion
}
