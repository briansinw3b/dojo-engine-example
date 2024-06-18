using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class VoidEventListerner : MonoBehaviour
{
    [SerializeField] VoidEventChannelSO _channel;
    public UnityEvent OnRaiseEvent;

    private void OnEnable()
    {
        if (_channel) _channel.OnEventRaise += Response;
    }

    private void OnDisable()
    {
        if (_channel) _channel.OnEventRaise -= Response; 
    }

    private void Response()
    {
        OnRaiseEvent?.Invoke();
    }
}
