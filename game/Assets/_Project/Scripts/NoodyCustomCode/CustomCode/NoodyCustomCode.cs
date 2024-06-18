using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using ImpossibleOdds;
using System.Linq;
using System.Resources;
using System.Runtime.CompilerServices;

namespace NOOD
{
    public static class NoodyCustomCode
    {
        public static Thread newThread;

        #region Look, mouse and Vector zone
        // public static bool IsPointerOverUIElement()
        // {

        // }
        public static Vector3 ScreenPointToWorldPoint(Vector2 screenPoint)
        {
            Camera cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
            return cam.ScreenToWorldPoint(screenPoint);
        }
        public static Vector3 MouseToWorldPoint()
        {
            Camera cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
            Vector3 mousePos = Input.mousePosition;
            return cam.ScreenToWorldPoint(mousePos);
        }
        public static Vector3 MouseToWorldPoint2D()
        {
            Camera cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
            Vector3 mousePos = MouseToWorldPoint();
            Vector3 temp = new Vector3(mousePos.x, mousePos.y, 0f);
            return temp;
        }
        public static Vector2 WorldPointToScreenPoint(Vector3 worldPoint)
        {
            Camera cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
            return cam.WorldToScreenPoint(worldPoint);
        }
        public static void LookToMouse2D(Transform objectTransform)
        {
            Vector3 mousePosition = MouseToWorldPoint();
            LookToPoint2D(objectTransform, mousePosition);
        }
        public static void LookToPoint2D(Transform objectTransform, Vector3 targetPosition)
        {
            Vector3 lookDirection = LookDirection(objectTransform.position, targetPosition);
            float angle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg;
            objectTransform.localEulerAngles = new Vector3(0f, 0f, angle);
        }
        public static Vector3 LookDirection(Vector3 FromPosition, Vector3 targetPosition)
        {
            return (targetPosition - FromPosition).normalized;
        }
        public static Vector3 GetPointAroundAPosition2D(Vector3 centerPosition, float degrees, float radius)
        {
            float radians = degrees * Mathf.Deg2Rad;
            float x = Mathf.Cos(radians);
            float y = Mathf.Sin(radians);
            Vector3 pos = new Vector3(x, y, centerPosition.z);
            return pos += centerPosition;
        }
        public static Vector3 GetPointAroundAPosition2D(Vector3 centerPosition, float radius)
        {
            int degrees = UnityEngine.Random.Range(0, 360);
            float radians = degrees * Mathf.Deg2Rad;
            float x = Mathf.Cos(radians);
            float y = Mathf.Sin(radians);
            Vector3 pos = new Vector3(x, y, centerPosition.z);
            pos *= radius;
            return pos += centerPosition;
        }
        public static Vector3 GetPointAroundAPosition3D(Vector3 centerPosition, float degrees, float radius)
        {
            float radians = degrees * Mathf.Deg2Rad;
            float x = Mathf.Cos(radians);
            float z = Mathf.Sin(radians);
            Vector3 pos = new Vector3(x, centerPosition.y, z);
            return pos += centerPosition;
        }
        public static Vector3 GetPointAroundAPosition3D(Vector3 centerPosition, float radius)
        {
            int degrees = UnityEngine.Random.Range(0, 360);
            float radians = degrees * Mathf.Deg2Rad;
            float x = Mathf.Cos(radians);
            float z = Mathf.Sin(radians);
            Vector3 pos = new Vector3(x, centerPosition.y, z);
            pos *= radius;
            return pos += centerPosition;
        }
        //Returns 'true' if we touched or hovering on Unity UI element.
        public static bool IsPointerOverUIElement()
        {
            GameObject gameObject = GetCurrentPointObject();
            if (gameObject == null) return false;

            return gameObject.layer == LayerMask.NameToLayer("UI"); 
        }
        public static GameObject GetCurrentPointObject()
        {
            if (GetEventSystemRaycastResults() != null && GetEventSystemRaycastResults().Count > 0)
                return GetEventSystemRaycastResults()[0].gameObject;
            else return null;
        }
        public static GameObject GetRaycastGameObject2D()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
            if(hit.collider != null)
            {
                return hit.collider.gameObject;
            }
            else return null;
        }
        //Gets all event system raycast results of current mouse or touch position.
        public static List<RaycastResult> GetEventSystemRaycastResults()
        {
            PointerEventData eventData = new PointerEventData(EventSystem.current);
            eventData.position = Input.mousePosition;
            List<RaycastResult> raycastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, raycastResults);
            return raycastResults;
        }
        #endregion

        #region Background Function
        public static void RunInBackground(Action function, Queue<Action> mainThreadActions = null)
        {
            //! WebGL doesn't do multithreading

            /* Create a mainThreadQueue in main script to hold the Action and run the action in Update like below
                 if your Action do something to Unity object like set transform, set physic or contain Unity Time class 
                !Ex for mainThreadQueue:
                    Queue<Action> mainThreadQueue = new Queue<Action>()
                    
                    void Update()
                    {
                        if(mainThreadQueue.Count > 0)
                        {
                            Action action = mainThreadQueue.Dequeue();
                            action();
                        }
                    }
            */

            //! if your function has parameters, use param like this
            //! NoodyCustomCode.RunInBackGround(() => yourFunction(parameters)); 

            Thread t = new Thread(() =>
            {
                if (mainThreadActions != null)
                {
                    AddToMainThread(function, mainThreadActions);
                }
                else
                {
                    function();
                }
            });
            t.Start();
        }

        private static void AddToMainThread(Action function, Queue<Action> mainThreadActions)
        {
            mainThreadActions.Enqueue(function);
        }

        //TODO: learn Unity.Jobs and create a Function to run many complex job in multithread

        #endregion

        #region Delay Function
        /// <summary>
        /// Start one time CR function and return DelayAction object
        /// </summary>
        /// <param name="action"></param>
        /// <param name="delaySecond"></param>
        /// <returns></returns>
        public static DelayAction StartDelayFunction(Action action, float delaySecond)
        {
            return StartDelayFunction(action, "", delaySecond);
        }
        public static DelayAction StartDelayFunction(Action action, string functionName, float delaySecond)
        {
            DelayAction delay = DelayAction.Create();

            delay.StartDelayFunction(() =>
            {
                action?.Invoke();
            }, functionName, delaySecond);
            return delay;
        }
        public static void StopDelayFunction(string functionName)
        {
            DelayAction.StopDelayFunction(functionName);
        }
        #endregion

        #region Camera
        /// <summary>
        /// Make camera size always show all object with collider (2D and 3D)
        /// (center, size) = CalculateOrthoCamsize();
        /// </summary>
        /// <param name="_camera">Main camera</param>
        /// <param name="_buffer">Amount of padding size</param>
        /// <returns></returns>
        public static (Vector3 center, float size) CalculateOrthoCamSize(Camera _camera, float _buffer)
        {
            Bounds bound = new Bounds(); //Create bound with center Vector3.zero;

            foreach (Collider2D col in UnityEngine.Object.FindObjectsOfType<Collider2D>())
            {
                bound.Encapsulate(col.bounds);
            }

            foreach (Collider col in UnityEngine.Object.FindObjectsOfType<Collider>())
            {
                bound.Encapsulate(col.bounds);
            }

            bound.Expand(_buffer);

            float vertical = bound.size.y;
            float horizontal = bound.size.x * _camera.pixelHeight / _camera.pixelWidth;

            //Debug.Log("V: " + vertical + ", H: " + horizontal);

            float size = Mathf.Max(horizontal, vertical) * 0.5f;
            Vector3 center = bound.center + new Vector3(0f, 0f, -10f);

            return (center, size);
        }

        /// <summary>
        /// Move camera base on your input (Put this function in Update to track the input), direction = -1 for opposite direction, 1 for follow direction
        /// </summary>
        /// <param name="camera">Camera you want to move</param>
        /// <param name="direction">-1 for oposite direction, 1 for follow direction</param>
        private static Vector3 DCMousePostion = Vector3.zero;
        private static Vector3 DCDir;
        private static Vector3 tempPos;
        private static Vector3 campPos;
        public static void DragCamera(GameObject camera, int direction = 1)
        {
            if (Input.GetMouseButtonDown(0))
            {
                DCMousePostion = MouseToWorldPoint2D();
            }

            if (Input.GetMouseButton(0))
            {
                if (MouseToWorldPoint2D() != DCMousePostion)
                {
                    DCDir = MouseToWorldPoint2D() - DCMousePostion;
                    camera.transform.position += direction * DCDir;
                }
            }
        }

        /// <summary>
        /// Move camera base on your input (Put this function in Update to track the input), direction = -1 for opposite direction, 1 for follow direction
        /// </summary>
        /// <param name="camera">Camera you want to move</param>
        /// <param name="direction">-1 for oposite direction, 1 for follow direction</param>
        public static void DragCamera(GameObject camera, float minX, float maxX, float minY, float maxY, int direction = 1)
        {
            if (Input.GetMouseButtonDown(0))
            {
                DCMousePostion = MouseToWorldPoint2D();
            }

            if (Input.GetMouseButton(0))
            {
                if (MouseToWorldPoint2D() != DCMousePostion)
                {
                    DCDir = MouseToWorldPoint2D() - DCMousePostion;

                    tempPos = direction * DCDir;
                    campPos = camera.transform.position;

                    if (campPos.x + tempPos.x > minX && campPos.x + tempPos.x < maxX)
                    {
                        campPos.x += tempPos.x;
                    }

                    if (campPos.y + tempPos.y > minY && campPos.y + tempPos.y < maxY)
                    {
                        campPos.y += tempPos.y;
                    }

                    camera.transform.position = campPos;
                }
            }
        }

        /// <summary>
        /// Move camera base on your input (Put this function in Update to track the input), direction = -1 for opposite direction, 1 for follow direction
        /// </summary>
        /// <param name="camera">Camera you want to move</param>
        /// <param name="direction">-1 for oposite direction, 1 for follow direction</param>
        public static void DragCamera2Finger(GameObject camera, float minX, float maxX, float minY, float maxY, int direction = 1)
        {
            if (Input.touchCount >= 2)
            {
                Touch touchZero = Input.GetTouch(0);
                Touch touchOne = Input.GetTouch(1);
                Vector2 avergare = (touchOne.position + touchZero.position) / 2;
                if (touchOne.phase == TouchPhase.Began)
                {

                    DCMousePostion = ScreenPointToWorldPoint(avergare);

                }
                if (touchOne.phase == TouchPhase.Moved)
                {
                    if (ScreenPointToWorldPoint(avergare) != DCMousePostion)
                    {
                        DCDir = ScreenPointToWorldPoint(avergare) - DCMousePostion;

                        tempPos = direction * DCDir;
                        campPos = camera.transform.position;

                        if (campPos.x + tempPos.x > minX && campPos.x + tempPos.x < maxX)
                        {
                            campPos.x += tempPos.x;
                        }

                        if (campPos.y + tempPos.y > minY && campPos.y + tempPos.y < maxY)
                        {
                            campPos.y += tempPos.y;
                        }

                        camera.transform.position = campPos;
                    }
                }

            }

        }

        public static void SmoothCameraFollow(GameObject camera, float smoothTime, Transform targetTransform, Vector3 offset,
        float maxX, float maxY, float minX, float minY)
        {

            Vector3 temp = camera.transform.position;
            Vector3 targetPosition = targetTransform.position + offset;
            Vector3 currentSpeed = Vector3.zero;
            Vector3 smoothPosition = Vector3.SmoothDamp(camera.transform.position, targetPosition, ref currentSpeed, smoothTime);
            if (smoothPosition.x < maxX && smoothPosition.x > minX)
            {
                temp.x = smoothPosition.x;
            }

            if (smoothPosition.y < maxY && smoothPosition.y > minY)
            {
                temp.y = smoothPosition.y;
            }

            temp.z = smoothPosition.z;
            camera.transform.position = temp;
        }

        public static void SmoothCameraFollow(GameObject camera, float smoothTime, Transform targetTransform, Vector3 offset)
        {

            Vector3 temp = camera.transform.position;
            Vector3 targetPosition = targetTransform.position + offset;
            Vector3 currentSpeed = Vector3.zero;
            Vector3 smoothPosition = Vector3.SmoothDamp(camera.transform.position, targetPosition, ref currentSpeed, smoothTime);
            //Vector3 smoothPosition = Vector3.Lerp(temp, targetPosition, smoothTime);

            temp.x = smoothPosition.x;
            temp.y = smoothPosition.y;
            temp.z = smoothPosition.z;

            camera.transform.position = temp;
        }

        public static void LerpSmoothCameraFollow(GameObject camera, float smoothTime, Transform targetTransform, Vector3 offset)
        {

            Vector3 temp = camera.transform.position;
            Vector3 targetPosition = targetTransform.position + offset;
            Vector3 currentSpeed = Vector3.zero;
            //Vector3 smoothPosition = Vector3.SmoothDamp(camera.transform.position, targetPosition, ref currentSpeed, smoothTime);
            Vector3 smoothPosition = Vector3.Lerp(temp, targetPosition, smoothTime * Time.fixedDeltaTime);

            temp.x = smoothPosition.x;
            temp.y = smoothPosition.y;
            temp.z = smoothPosition.z;

            camera.transform.position = temp;
        }

        public static IEnumerator ObjectShake(GameObject @object, float duration, float magnitude)
        {
            Vector3 OriginalPos = @object.transform.localPosition;
            float elapsed = 0.0f;
            float range = 1f;
            while (elapsed < duration)
            {
                float x, y;
                if (elapsed / duration * 100 < 80)
                {
                    //Starting shake
                    x = UnityEngine.Random.Range(-range, range) * magnitude;
                    y = UnityEngine.Random.Range(-range, range) * magnitude;
                }
                else
                {
                    //Ending
                    range -= Time.deltaTime * elapsed;
                    x = UnityEngine.Random.Range(-range, range) * magnitude;
                    y = UnityEngine.Random.Range(-range, range) * magnitude;
                }

                @object.transform.localPosition = new Vector3(x, y, OriginalPos.z);

                elapsed += Time.deltaTime;
                yield return null;
            }
            @object.transform.localPosition = OriginalPos;
        }
        #endregion

        #region Color
        /// <summary>
        /// <para>Return RGBA color </para>
        /// </summary>
        /// <param name="hexCode">hex code form RRGGBB or RRGGBBAA for alpha output</param>
        /// <returns>Color with RGBA form</returns>
        public static Color HexToColor(string hexCode)
        {
            _ = ColorUtility.TryParseHtmlString(hexCode, out Color color);

            return color;
        }
        //----------------------------//
        /// <summary>
        /// Return hex code with alpha of the color
        /// </summary>
        /// <param name="color">Color's form RRGGBBAA</param>
        /// <returns></returns>
        public static string ColorAToHex(Color color)
        {
            return ColorUtility.ToHtmlStringRGBA(color);
        }
        //----------------------------//
        /// <summary>
        /// Return hex code without alpha of the color
        /// </summary>
        /// <param name="color">Color's form RRGGBB</param>
        /// <returns></returns>
        public static string ColorToHex(Color color)
        {
            return ColorUtility.ToHtmlStringRGB(color);
        }
        //----------------------------//
        public static void FadeCanvasGroup(this object source, CanvasGroup canvasGroup, float endValue, float speed)
        {
            StartUpdater(source, () =>
            {
                if (Mathf.Abs(canvasGroup.alpha - endValue) > 0.01f)
                {
                    if(canvasGroup.alpha < endValue)
                        canvasGroup.alpha += Time.deltaTime * speed;
                    if (canvasGroup.alpha > endValue)
                        canvasGroup.alpha -= Time.deltaTime * speed;
                    return true;
                }
                return false;
            });
        }
        
        //----------------------------//
        /// <summary>
        /// reduce alpha to 0 over Time.deltaTime
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static void FadeOutImage(Image image)
        {
            FadeOutImage(image, 0, Time.deltaTime);
        }
        /// <summary>
        /// reduce alpha to 0
        /// </summary>
        /// <param name="image"></param>
        /// <param name="pauseTimePerLoop"></param>
        public static void FadeOutImage(Image image, float pauseTimePerLoop)
        {
            FadeOutImage(image, 0, pauseTimePerLoop);
        }
        /// <summary>
        /// reduce alpha to the end value
        /// </summary>
        /// <param name="image"></param>
        /// <param name="endValue">the stop value</param>
        /// <param name="pauseTimePerLoop">time between loop</param>
        public static void FadeOutImage(Image image, float endValue, float pauseTimePerLoop)
        {
            GameObject fadeOutObj = new GameObject("FadeOutObj");
            CoroutineScript coroutineScript = fadeOutObj.AddComponent<CoroutineScript>();

            coroutineScript.StartCoroutineLoop(() =>
            {
                Color color = image.color;
                color.a -= Time.deltaTime;
                image.color = color;
                if (color.a <= endValue)
                {
                    image.gameObject.SetActive(false);
                    return true;
                }
                return false;
            }, "",pauseTimePerLoop, -1);
        }
        //----------------------------//
        /// <summary>
        /// Fade in the image by crease color over Time.deltaTime
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static void FadeInImage(Image image)
        {
            FadeInImage(image, 1, Time.deltaTime);
        }
        /// <summary>
        /// increase alpha to 1
        /// </summary>
        /// <param name="image"></param>
        /// <param name="pauseTimePerLoop"></param>
        public static void FadeInImage(Image image, float pauseTimePerLoop)
        {
            FadeInImage(image, 1, pauseTimePerLoop);
        }
        /// <summary>
        /// increase alpha to the maxValue
        /// </summary>
        /// <param name="image"></param>
        /// <param name="maxValue"></param>
        /// <param name="pauseTimePerLoop">time between loop</param>
        public static void FadeInImage(Image image, float maxValue, float pauseTimePerLoop)
        {
            GameObject fadeInObj = new GameObject("FadeInObj");
            image.gameObject.SetActive(true);
            CoroutineScript coroutineScript = fadeInObj.AddComponent<CoroutineScript>();

            coroutineScript.StartCoroutineLoop(() =>
            {
                Color color = image.color;
                color.a += Time.deltaTime;
                image.color = color;
                if(image.color.a >= maxValue)
                {
                    return true;
                }
                return false;
            },"", pauseTimePerLoop, -1);
        }
        //----------------------------//
        /// <summary>
        /// decrease alpha to 0 over Time.deltaTime
        /// </summary>
        /// <param name="textMeshProUGUI"></param>
        public static void FadeOutTextMeshUGUI(TextMeshProUGUI textMeshProUGUI)
        {
            FadeOutTextMeshUGUI(textMeshProUGUI, 0, Time.deltaTime);
        }
        /// <summary>
        /// increase alpha to 0
        /// </summary>
        /// <param name="textMeshProUGUI"></param>
        /// <param name="pauseTimePerLoop">time between loop</param>
        public static void FadeOutTextMeshUGUI(TextMeshProUGUI textMeshProUGUI, float pauseTimePerLoop)
        {
            FadeOutTextMeshUGUI(textMeshProUGUI, 0, pauseTimePerLoop);
        }
        /// <summary>
        /// decrease alpha to endValue
        /// </summary>
        /// <param name="textMeshProUGUI"></param>
        /// <param name="endValue"></param>
        /// <param name="pauseTimePerLoop">Time between loop</param>
        public static void FadeOutTextMeshUGUI(TextMeshProUGUI textMeshProUGUI, float endValue,float pauseTimePerLoop)
        {
            GameObject fadeOutObj = new GameObject("FadeOutObj");
            CoroutineScript coroutineScript = fadeOutObj.AddComponent<CoroutineScript>();

            coroutineScript.StartCoroutineLoop(() =>
            {
                Color color = textMeshProUGUI.color;
                color.a -= Time.deltaTime;
                textMeshProUGUI.color = color;
                if(textMeshProUGUI.color.a <= endValue)
                {
                    textMeshProUGUI.gameObject.SetActive(false);
                    return true;
                }
                return false;
            }, "", pauseTimePerLoop, -1);
        }
        //----------------------------//
        /// <summary>
        /// increase alpha to 1 over Time.deltaTime
        /// </summary>
        /// <param name="textMeshProUGUI"></param>
        public static void FadeInTextMeshUGUI(TextMeshProUGUI textMeshProUGUI)
        {
            FadeInTextMeshUGUI(textMeshProUGUI, 1, Time.deltaTime);
        }
        /// <summary>
        /// increase alpha to 1
        /// </summary>
        /// <param name="textMeshProUGUI"></param>
        /// <param name="pauseTimePerLoop"></param>
        public static void FadeInTextMeshUGUI(TextMeshProUGUI textMeshProUGUI, float pauseTimePerLoop)
        {
            FadeInTextMeshUGUI(textMeshProUGUI, 1, pauseTimePerLoop);
        }
        /// <summary>
        /// increase alpha to maxValue
        /// </summary>
        /// <param name="textMeshProUGUI"></param>
        /// <param name="maxValue"></param>
        /// <param name="pauseTimePerLoop"></param>
        public static void FadeInTextMeshUGUI(TextMeshProUGUI textMeshProUGUI, float maxValue, float pauseTimePerLoop)
        {
            textMeshProUGUI.gameObject.SetActive(true);
            GameObject fadeInObj = new GameObject("FadeInObj");
            CoroutineScript coroutineScript = fadeInObj.AddComponent<CoroutineScript>();

            coroutineScript.StartCoroutineLoop(() =>
            {
                Color color = textMeshProUGUI.color;
                color.a += Time.deltaTime;
                textMeshProUGUI.color = color;
                if(textMeshProUGUI.color.a >= maxValue)
                {
                    return true;
                }
                return false;
            },"", pauseTimePerLoop, -1);
        }
        public static void FadeOutTextMeshColor(TextMeshPro textMeshPro, float endValue, float duration)
        {
            GameObject fadeOutObj = new GameObject("FadeOutObj");
            CoroutineScript coroutineScript = fadeOutObj.AddComponent<CoroutineScript>();

            coroutineScript.StartCoroutineLoop(() =>
            {
                Color color = textMeshPro.color;
                color.a -= Time.deltaTime / duration;
                textMeshPro.color = color;
                if(textMeshPro.color.a <= endValue)
                {
                    textMeshPro.gameObject.SetActive(false);
                    return true;
                }
                return false;
            }, "", 0, -1);
        }
        #endregion
    
        #region CoroutineFunction
        /// <summary>
        /// Create a coroutineScript for coroutine loop function
        /// </summary>
        /// <returns></returns>
        public static CoroutineScript CreateNewCoroutineObj()
        {
            return CoroutineScript.Create();
        }
        /// <summary>
        /// Create Coroutine loop with loopTime
        /// </summary>
        /// <param name="action"></param>
        /// <param name="loopTime"> How many times function will loop </param>
        public static void StartNewCoroutineLoop(Action action, int loopTime)
        {
            StartNewCoroutineLoop(action, "", Time.deltaTime, loopTime);
        }
        /// <summary>
        /// Create Coroutine loop with loopTime
        /// </summary>
        /// <param name="action"></param>
        /// <param name="pausePerLoop"> Time pause per loop </param>
        public static void StartNewCoroutineLoop(Action action, float pausePerLoop)
        {
            StartNewCoroutineLoop(action, "", pausePerLoop, -1);
        }
        /// <summary>
        /// Create Coroutine loop with 1 frame pause and infinity loop
        /// </summary>
        /// <param name="action"></param>
        public static void StartNewCoroutineLoop(Action action)
        {
            StartNewCoroutineLoop(action, Time.deltaTime);
        }
        /// <summary>
        /// Create Coroutine loop with pausePerLoop and loopTime 
        /// </summary>
        /// <param name="action"></param>
        /// <param name="pausePerLoop"> time pause per loop </param>
        /// <param name="loopTime"> how many loop time</param>
        public static void StartNewCoroutineLoop(Action action, string functionName, float pausePerLoop, int loopTime)
        {
            CoroutineScript coroutineScript = CreateNewCoroutineObj();
            coroutineScript.StartCoroutineLoop(() =>
            {
                action?.Invoke();
                return false;
            }, functionName, pausePerLoop, loopTime);
        }
        /// <summary>
        /// Create coroutine loop with Func(bool) will stop if Func(bool) return true
        /// </summary>
        /// <param name="func"> function to perform </param>
        public static void StartNewCoroutineLoop(Func<bool> func, string functionName)
        {
            CoroutineScript coroutineScript = CreateNewCoroutineObj();
            coroutineScript.StartCoroutineLoop(func, functionName, Time.deltaTime, -1);
        }
        public static void StopCoroutineLoop(string functionName)
        {
            CoroutineScript.StopCoroutineLoop(functionName);
        }
        #endregion

        #region Events
        /// <summary>
        /// UnSubscribe all function belong to currentObject from staticType
        /// </summary>
        /// <param name="staticType"></param>
        /// <param name="currentObject"></param>
        public static void UnSubscribeFromStatic(Type staticType, object currentObject)
        {
            if(currentObject != null)
                staticType.PurgeDelegatesOf(currentObject);
        }
        /// <summary>
        /// UnSubscribe all function belong to functionObject from delegateObject
        /// </summary>
        /// <param name="delegateObject"> object that hold events </param>
        /// <param name="functionObject"> object that hold functions </param>
        public static void UnSubscribeAllEvent(object delegateObject, object functionObject)
        {
            if(delegateObject != null && functionObject != null)
                delegateObject.PurgeDelegatesOf(functionObject);
        }
        /// <summary>
        /// UnSubscribe all function belong to currentObject from instance of object
        /// </summary>
        /// <typeparam name="T"> The type of instance object </typeparam>
        /// <param name="currentObject"></param>
        public static void UnSubscribeAllEvent<T>(object currentObject) where T : MonoBehaviour
        {
            // Get Instance in scene
            UnityEngine.Object instance = GameObject.FindObjectOfType<T>();
            UnSubscribeAllEvent(instance, currentObject);
        }
        #endregion

        #region Update Functions
        public static void StartUpdater(object target, Action action)
        {
            UpdateObject.Create(target, () => {action?.Invoke(); return false;}, "", false);
        }
        public static void StartUpdater(object target, Action action, string functionName)
        {
            UpdateObject.Create(target, () => {action?.Invoke(); return false;}, functionName, false);
        }
        public static void StartUpdater(object target, Action action, string functionName, bool stopAllWithTheSameName)
        {
            UpdateObject.Create(target, () => {action?.Invoke(); return false;}, functionName, stopAllWithTheSameName);
        }
        public static UpdateObject StartUpdater(object target, Func<bool> func)
        {
            return UpdateObject.Create(target, func, "", false);
        }
        public static UpdateObject StartUpdater(object target, Func<bool> func, string functionName)
        {
            return UpdateObject.Create(target, func, functionName, false);
        }
        public static void StartUpdater(object target, Func<bool> func, string functionName, bool stopAllWithTheSameName)
        {
            UpdateObject.Create(target, func, functionName, stopAllWithTheSameName);
        }
        public static void StopUpdaterWithName(string functionName)
        {
            UpdateObject.StopWithName(functionName);
        }
        public static void StopAllUpdaterWithName(string functionName)
        {
            UpdateObject.StopAllWithName(functionName);
        }
        #endregion 

        #region Transform and Collider 
        public static Vector3 GetRandomPointInsideCollider(Collider collider)
        {
            Vector3 minPosition = collider.bounds.min;
            Vector3 maxPosition = collider.bounds.max;

            float randX = UnityEngine.Random.Range(minPosition.x, maxPosition.x);
            float randY = UnityEngine.Random.Range(minPosition.y, maxPosition.y);
            float randZ = UnityEngine.Random.Range(minPosition.z, maxPosition.z);

            return new Vector3(randX, randY, randZ);
        }
        public static Vector2 GetRandomPointInsideCollider2D(Collider2D collider)
        {
            Vector2 minPosition = collider.bounds.min;
            Vector2 maxPosition = collider.bounds.max;

            float randX = UnityEngine.Random.Range(minPosition.x, maxPosition.x);
            float randY = UnityEngine.Random.Range(minPosition.y, maxPosition.y);

            return new Vector2(randX, randY);
        }
        #endregion

        #region Hex and String
        public static bool CompareHexStrings(string hex1, string hex2)
        {
            // Remove the "0x" prefix if it exists
            hex1 = hex1.StartsWith("0x") ? hex1.Substring(2) : hex1;
            hex2 = hex2.StartsWith("0x") ? hex2.Substring(2) : hex2;

            // Pad the shorter string with leading zeros
            if (hex1.Length < hex2.Length)
            {
                hex1 = hex1.PadLeft(hex2.Length, '0');
            }
            else if (hex2.Length < hex1.Length)
            {
                hex2 = hex2.PadLeft(hex1.Length, '0');
            }

            // Compare the strings
            return hex1.Equals(hex2, System.StringComparison.OrdinalIgnoreCase);
        }
        #endregion
    }

}
