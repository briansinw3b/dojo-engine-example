using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace NOOD
{
    public class DelayAction : MonoBehaviour
    {
        private static GameObject _initObject;
        private static List<DelayAction> _delayActions;
        public Action OnComplete;
        [SerializeField] private float delaySecond;
        [SerializeField] private Action delayAction;
        [SerializeField] private string functionName;

        #region Static
        public static DelayAction Create()
        {
            if(_initObject == null || _initObject.gameObject == null)
            {
                _initObject = new GameObject("DelayInitObject");
                _delayActions = new List<DelayAction>();
                DontDestroyOnLoad(_initObject);
            }
            GameObject delayObject = new GameObject("DelayObject");
            DelayAction delayAction = delayObject.AddComponent<DelayAction>();
            _delayActions.Add(delayAction);
            DontDestroyOnLoad(delayObject);

            return delayAction;
        }
        public static void StopDelayFunction(string functionName)
        {
            if(_delayActions != null && _delayActions.Any(x => x.functionName == functionName))
            {
                DelayAction delayAction = _delayActions.First(x => x.functionName == functionName);
                _delayActions.Remove(delayAction);
                delayAction.StopDelayFunction();
            }
        }
        #endregion

        private IEnumerator Co_Function()
        {
            yield return new WaitForSeconds(delaySecond);
            delayAction?.Invoke();
            OnComplete?.Invoke();
            _delayActions.Remove(this);
            Destroy(this.gameObject);
        }

        public void StartDelayFunction(Action action, string functionName, float second)
        {
            this.delayAction = action;
            this.delaySecond = second;
            this.functionName = functionName;
            StartCoroutine(Co_Function());
        }
        private void StopDelayFunction()
        {
            this.StopCoroutine(this.Co_Function());
            Destroy(this.gameObject, 0.2f);
        }
    }
}
