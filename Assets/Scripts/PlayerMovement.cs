using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    private GameObject cam;
    CharacterController pawn;
    private float speed = 7f;

    public Transform jointHipLeft, jointHipRight, jointKneeLeft, jointKneeRight;
    float walkAnimTimer = 0f;
    private Vector3 inputDir;
    private float velocityVertical = 0;

    private float gravMult = -9.8f;


    // Start is called before the first frame update
    void Start()
    {
        pawn = GetComponent<CharacterController>();
        cam = FindObjectOfType<CameraController>().gameObject;
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
            float playerYaw = transform.eulerAngles.y;
            float camYaw = cam.transform.eulerAngles.y;

            camYaw = AnimMath.AngleWrapDegrees(playerYaw, camYaw);

            Quaternion playerRot = Quaternion.Euler(0, playerYaw, 0);
            Quaternion targetRot = Quaternion.Euler(0, camYaw, 0);
            transform.rotation = AnimMath.Ease(playerRot, targetRot, .001f);
            
        }

        inputDir = (transform.forward * v + transform.right * h);
        if (inputDir.sqrMagnitude > 1) inputDir.Normalize();


        bool wantsToJump = Input.GetButtonDown("Jump");
        if (pawn.isGrounded)
        {
            velocityVertical = 0;
            if(wantsToJump) velocityVertical += 7f;
        }
        velocityVertical += gravMult * Time.deltaTime;

        

        Vector3 moveAmt = (inputDir * speed) + (Vector3.up * velocityVertical);


        pawn.Move(moveAmt * Time.deltaTime);
        

        WalkAnim();
    }

    void WalkAnim()
    {
        
        walkAnimTimer += Time.deltaTime * speed;
        
        Vector3 inputDirLocal = transform.InverseTransformDirection(inputDir);
        Vector3 axis = Vector3.Cross(Vector3.up, inputDirLocal);

        float alignment = Vector3.Dot(inputDirLocal, Vector3.forward);
        alignment = Mathf.Abs(alignment);

        float degrees = AnimMath.Lerp(10, 35, alignment);

        float waveHip = Mathf.Sin(walkAnimTimer) * degrees;

        jointHipLeft.localRotation = Quaternion.AngleAxis(waveHip, axis);
        jointHipRight.localRotation = Quaternion.AngleAxis(-waveHip, axis);

        float waveKneeLeft = (inputDir == Vector3.zero) ? 0 : (Mathf.Sin(walkAnimTimer) + 1) * 10;
        float waveKneeRight = (inputDir == Vector3.zero) ? 0 : (Mathf.Sin(walkAnimTimer + Mathf.PI) + 1) * 10;

        jointKneeLeft.localRotation = Quaternion.Euler(waveKneeLeft, 0, 0);
        jointKneeRight.localRotation = Quaternion.Euler(waveKneeRight, 0, 0);
    }
}
