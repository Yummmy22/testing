using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;
public class FBXCameraController : MonoBehaviour
{
    public enum EMouse { FreeRotation, LeftButton, MiddleButton, RightButton }

    [Header("Main Setting")]
    public Transform TargetCamera;
    public Transform TargetPlayer;
    public bool RotationEnabled;

    [Header("Distance Setting")]
    public float distance = 5f;
    public float minDistance = 1f; //Min distance of the camera from the target
    public float maxDistance = 10f;
    public int yMinLimit = 10; //Lowest vertical angle in respect with the target.
    public int yMaxLimit = 80;

    [Header("Mouse Setting")]
    public EMouse MouseInput;
    public bool CursorVisible;
    public bool CursorLocked;
    public int mouseZoomSpeed = 2;
    public float mouseXSpeed = 1000.0f;
    public float mouseYSpeed = 1000.0f;

    [Header("Touch Setting")]
    public float touchZoomSpeed = 1;
    public float touchXSpeed = 200.0f;
    public float touchYSpeed = 100.0f;

    private float lastDist = 0;
    private float curDist = 0;


    private float x = 0.0f;
    private float y = 0.0f;
    private Touch touch;

    public bool MouseLookActive = true;

    public static bool IsPointerOverUIElement()
    {
        return IsPointerOverUIElement(GetEventSystemRaycastResults());
    }
    ///Returns 'true' if we touched or hovering on Unity UI element.
    public static bool IsPointerOverUIElement(List<RaycastResult> eventSystemRaysastResults)
    {
        for (int index = 0; index < eventSystemRaysastResults.Count; index++)
        {
            RaycastResult curRaysastResult = eventSystemRaysastResults[index];
            if (curRaysastResult.gameObject.layer == LayerMask.NameToLayer("UI"))
                return true;
        }
        return false;
    }
    ///Gets all event systen raycast results of current mouse or touch position.
    static List<RaycastResult> GetEventSystemRaycastResults()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> raysastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raysastResults);
        return raysastResults;
    }

    void Start()
    {
        Cursor.visible = CursorVisible;
        Cursor.lockState = CursorLockMode.None;
        if (CursorLocked)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        TargetCamera.gameObject.SetActive(true);
        TargetCamera.transform.parent = null;
        Vector3 angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;
    }



    void Update()
    {

        if (Input.GetKey(KeyCode.Escape) || Input.GetKey(KeyCode.LeftAlt))
        {
            CursorVisible = true;
            Cursor.visible = true;
            MouseLookActive = false;
            Cursor.lockState = CursorLockMode.None;
        }

        if (Input.GetKey(KeyCode.Mouse0) && !IsPointerOverUIElement())
        {
            MouseLookActive = true;
            if (CursorLocked)
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
        } 

        if (IsPointerOverUIElement())
        {
            MouseLookActive = false;
        }

        if (TargetPlayer && TargetCamera && MouseLookActive)
        {
            if (RotationEnabled)
            {

                //Zooming with touch
                if (Input.touchCount > 1 && (Input.GetTouch(0).phase == TouchPhase.Moved || Input.GetTouch(1).phase == TouchPhase.Moved))
                {
                    //Two finger touch does pinch to zoom
                    var touch1 = Input.GetTouch(0);
                    var touch2 = Input.GetTouch(1);
                    curDist = Vector2.Distance(touch1.position, touch2.position);
                    if (curDist > lastDist)
                    {
                        distance -= Vector2.Distance(touch1.deltaPosition, touch2.deltaPosition) * touchZoomSpeed / 10;
                    }
                    else
                    {
                        distance += Vector2.Distance(touch1.deltaPosition, touch2.deltaPosition) * touchZoomSpeed / 10;
                    }
                    lastDist = curDist;
                }
                else if (Input.GetAxis("Mouse ScrollWheel") != 0)  //ELSE Zooming with mouse
                {
                    distance += Input.GetAxis("Mouse ScrollWheel") * mouseZoomSpeed;
                    distance = Mathf.Clamp(distance, minDistance, maxDistance);
                }

                //Rotate with touch
                if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Moved)
                {
                    //One finger touch does orbit
                    touch = Input.GetTouch(0);
                    x += touch.deltaPosition.x * touchXSpeed * Time.deltaTime;
                    y -= touch.deltaPosition.y * touchYSpeed * Time.deltaTime;
                }
                else
                {
                    KeyCode MouseButton = KeyCode.Mouse0;

                    if (MouseInput == EMouse.LeftButton)
                    {
                        MouseButton = KeyCode.Mouse0;
                    }
                    if (MouseInput == EMouse.RightButton)
                    {
                        MouseButton = KeyCode.Mouse1;
                    }
                    if (MouseInput == EMouse.MiddleButton)
                    {
                        MouseButton = KeyCode.Mouse2;
                    }

                    if (Input.GetKey(MouseButton) || MouseInput == EMouse.FreeRotation)
                    {
                        x += Input.GetAxis("Mouse X") * mouseXSpeed * Time.deltaTime;
                        y -= Input.GetAxis("Mouse Y") * mouseYSpeed * Time.deltaTime;
                    }
                }

                y = ClampAngle(y, yMinLimit, yMaxLimit);
                Quaternion rotation = Quaternion.Euler(y, x, 0);
                Vector3 vTemp = new Vector3(0.0f, 0.0f, -distance);
                Vector3 position = rotation * vTemp + TargetPlayer.position;
                TargetCamera.position = position;
                TargetCamera.rotation = rotation;
            }
        }

    }



    static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360)
            angle += 360;
        if (angle > 360)
            angle -= 360;
        return Mathf.Clamp(angle, min, max);
    }
}