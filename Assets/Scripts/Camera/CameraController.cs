using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController: MonoBehaviour
{
    public Transform thingToLookAt;
    public float mouseSensitivityX = 2f;
    public float mouseSensitivityY = 2f;
    public float scrollSensitivity = 1f;
    public Vector3 cameraOffset;

    private Camera cam;

    private float pitch = 0, yaw = 0;
    private float zoom = 10;
    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponentInChildren<Camera>(); 
    }

    // Update is called once per frame
    void Update()
    {
        // Rotation
        yaw += Input.GetAxis("Mouse X") * mouseSensitivityX; // Yaw (y)
        pitch -= Input.GetAxis("Mouse Y") * mouseSensitivityY; // Pitch (x)

        pitch = Mathf.Clamp(pitch, -89, 89);

        transform.rotation = Quaternion.Euler(pitch, yaw, 0);
        

        

        // Dolly
        Vector2 scrollAmt = Input.mouseScrollDelta;
        zoom -= scrollAmt.y * scrollSensitivity;

        zoom = Mathf.Clamp(zoom, 2, 20);

        float z = AnimMath.Ease(cam.transform.localPosition.z, -zoom, .01f);
        
        cam.transform.localPosition = new Vector3(0, 0, z);

    }
    void LateUpdate()
    {
        // Position
        if (thingToLookAt == null) return;
        if (Vector3.Distance(transform.position, thingToLookAt.position) > .01f) transform.position = AnimMath.Ease(transform.position, thingToLookAt.position + cameraOffset, .001f);
        else transform.position = thingToLookAt.position + cameraOffset;
    }
}
