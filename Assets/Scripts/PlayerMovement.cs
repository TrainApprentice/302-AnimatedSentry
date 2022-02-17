using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public GameObject cam;
    CharacterController pawn;
    private float speed = 7f;

    private bool setYaw = false;

    // Start is called before the first frame update
    void Start()
    {
        pawn = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        
        

        // Movement
        float v = Input.GetAxisRaw("Vertical");
        float h = Input.GetAxisRaw("Horizontal");

        if (cam && (h != 0 || v != 0))
        {
            //WrapAngle();
            Quaternion targetRot = Quaternion.Euler(0, cam.transform.eulerAngles.y, 0);
            transform.rotation = AnimMath.Ease(transform.rotation, targetRot, .001f);

        }

        Vector3 direction = (transform.forward * v + transform.right * h);
        if (direction.sqrMagnitude > 1) direction.Normalize();
        
        pawn.SimpleMove(direction * speed);

        


    }
    void WrapAngle()
    {
        float playerYaw = transform.eulerAngles.y;
        float camYaw = cam.transform.eulerAngles.y;
        
        float targetYaw = camYaw;
        float tempYaw = playerYaw;

        float currYaw = 0;
        Quaternion targetRot;

        while (camYaw > playerYaw + 180)
        {
            targetYaw = camYaw + 360;
            currYaw = AnimMath.Ease(tempYaw, targetYaw, .001f);
            targetRot = Quaternion.Euler(0, currYaw, 0);
            transform.rotation = targetRot;

        }
        while(camYaw < playerYaw - 180)
        {
            targetYaw = camYaw - 360;
            currYaw = AnimMath.Ease(tempYaw, targetYaw, .001f);
            targetRot = Quaternion.Euler(0, currYaw, 0);
            transform.rotation = targetRot;
        }
        currYaw = AnimMath.Ease(playerYaw, camYaw, .001f);

        targetRot = Quaternion.Euler(0, currYaw, 0);
        transform.rotation = targetRot;


    }
}
