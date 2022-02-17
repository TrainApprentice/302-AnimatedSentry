using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController: MonoBehaviour
{
    public PlayerTargeting player;


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
        if (!player) player = FindObjectOfType<PlayerTargeting>();
    }

    // Update is called once per frame
    void Update()
    {
        bool isAiming = (player && player.target && player.playerWantsToAim);

        // Position
        if (player == null) return;
        if (Vector3.Distance(transform.position, player.transform.position) > .01f) transform.position = AnimMath.Ease(transform.position, player.transform.position + cameraOffset, .001f);
        else transform.position = player.transform.position + cameraOffset;




        float playerYaw = player.transform.eulerAngles.y;
        playerYaw = AnimMath.AngleWrapDegrees(yaw, playerYaw);
        // Rig Rotation

        if (isAiming)
        {
            Quaternion tempTarget = Quaternion.Euler(20, playerYaw, 0);
            
            transform.rotation = AnimMath.Ease(transform.rotation, tempTarget, .001f);
        }
        else
        {
            yaw += Input.GetAxis("Mouse X") * mouseSensitivityX; // Yaw (y)
            pitch -= Input.GetAxis("Mouse Y") * mouseSensitivityY; // Pitch (x)

            pitch = Mathf.Clamp(pitch, -89, 89);
            

            transform.rotation = AnimMath.Ease(transform.rotation, Quaternion.Euler(pitch, yaw, 0), .001f);
        }
        




        // Dolly

        Vector2 scrollAmt = Input.mouseScrollDelta;
        zoom -= scrollAmt.y * scrollSensitivity;

        zoom = Mathf.Clamp(zoom, 2, 20);

        

        float z = (isAiming) ? -3 : -zoom;

        cam.transform.localPosition = AnimMath.Ease(cam.transform.localPosition, new Vector3(0, 0, z), .01f);

        // Rotate ONLY the camera

        if(isAiming)
        {
            Vector3 toAimTarget = player.target.transform.position - cam.transform.position;

            Vector3 lookEuler = Quaternion.LookRotation(toAimTarget).eulerAngles;

            lookEuler.y = AnimMath.AngleWrapDegrees(playerYaw, lookEuler.y);

            Quaternion lookTemp = Quaternion.Euler(lookEuler.x, lookEuler.y, 0);

            cam.transform.rotation = AnimMath.Ease(cam.transform.rotation, lookTemp, .001f);
        }
        else
        {
            cam.transform.localRotation = AnimMath.Ease(cam.transform.localRotation, Quaternion.identity, .001f);
        }



    }
   
}
