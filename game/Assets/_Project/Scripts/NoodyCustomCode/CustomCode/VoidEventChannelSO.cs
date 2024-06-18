using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Events/Void Event")]
public class VoidEventChannelSO : ScriptableObject
{
    public UnityAction OnEventRaise;
    public void RaiseEvent()
    {
        OnEventRaise?.Invoke();
    }
}
