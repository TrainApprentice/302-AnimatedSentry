using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    private GameObject cam;
    CharacterController pawn;
    private float speed = 7f;

    public Transform jointHips, jointHipLeft, jointHipRight, jointKneeLeft, jointKneeRight;
    public Transform jointSpine, jointNeck, jointHairLeft, jointHairRight;
    public Transform jointShoulderLeft, jointShoulderRight, jointElbowLeft, jointElbowRight;
    PlayerTargeting playerTargeting;


    float walkAnimTimer = 0f;
    float idleAnimTimer = 0f;
    float airAnimTimer = 0f;
    private Vector3 inputDir;
    private float velocityVertical = 0;
    private float gravMult = -9.8f;


    // Start is called before the first frame update
    void Start()
    {
        pawn = GetComponent<CharacterController>();
        cam = FindObjectOfType<CameraController>().gameObject;
        playerTargeting = GetComponent<PlayerTargeting>();
    }

    // Update is called once per frame
    void Update()
    {
        // Movement
        float v = Input.GetAxisRaw("Vertical");
        float h = Input.GetAxisRaw("Horizontal");

        bool playerIsAiming = (playerTargeting && playerTargeting.playerWantsToAim && playerTargeting.target);
        if(playerIsAiming)
        {
            Vector3 toTarget = playerTargeting.target.transform.position - transform.position;
            toTarget.Normalize();
            Quaternion worldRot = Quaternion.LookRotation(toTarget);
            Vector3 euler = worldRot.eulerAngles;
            euler.x = 0;
            euler.z = 0;
            worldRot.eulerAngles = euler;
            transform.rotation = AnimMath.Ease(transform.rotation, worldRot, .001f);
        }
        else if (cam && (h != 0 || v != 0))
        {
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

        if (Input.GetKey("left shift")) speed = 12f;
        else speed = 7f;

        Vector3 moveAmt = (inputDir * speed) + (Vector3.up * velocityVertical);


        pawn.Move(moveAmt * Time.deltaTime);


        if (pawn.isGrounded && inputDir != Vector3.zero) WalkAnim();
        else if (!pawn.isGrounded) AirAnim();
        else IdleAnim();
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

        float waveKneeLeft = (Mathf.Sin(walkAnimTimer) + 1) * 10;
        float waveKneeRight = (Mathf.Sin(walkAnimTimer + Mathf.PI) + 1) * 10;

        jointKneeLeft.localRotation = Quaternion.Euler(waveKneeLeft, 0, 0);
        jointKneeRight.localRotation = Quaternion.Euler(waveKneeRight, 0, 0);

        float waveSpine = Mathf.Cos(walkAnimTimer) * .03f;
        jointSpine.localPosition = new Vector3(0, waveSpine, 0);

        if(!playerTargeting.target)
        {
            float waveShoulder = Mathf.Sin(walkAnimTimer) * 20f;

            jointShoulderLeft.localRotation = Quaternion.Euler(waveShoulder + 90, 0, 0);
            jointShoulderRight.localRotation = Quaternion.Euler(-waveShoulder + 90, 0, 0);

            float waveElbowLeft = -(Mathf.Sin(walkAnimTimer) + 1) * 10;
            float waveElbowRight = -(Mathf.Sin(walkAnimTimer + Mathf.PI) + 1) * 10;

            jointElbowLeft.localRotation = Quaternion.Euler(waveElbowLeft, 0, 0);
            jointElbowRight.localRotation = Quaternion.Euler(waveElbowRight, 0, 0);
        }
        else
        {
            jointElbowLeft.localRotation = Quaternion.Euler(0, 0, 0);
            jointElbowRight.localRotation = Quaternion.Euler(0, 0, 0);
        }
        
    }

    void IdleAnim()
    {
        idleAnimTimer += Time.deltaTime * 20f;
        if(!playerTargeting.target)
        {
            jointShoulderLeft.localRotation = Quaternion.Euler(90, 0, 0);
            jointShoulderRight.localRotation = Quaternion.Euler(90, 0, 0);

            float waveShoulders = Mathf.Sin(idleAnimTimer) * .05f;
            jointShoulderLeft.localPosition = new Vector3(jointShoulderLeft.localPosition.x, waveShoulders + .276f, jointShoulderLeft.localPosition.z);
            jointShoulderRight.localPosition = new Vector3(jointShoulderRight.localPosition.x, waveShoulders + .276f, jointShoulderRight.localPosition.z);
        }
        else
        {
            jointShoulderLeft.localPosition = new Vector3(jointShoulderLeft.localPosition.x, .276f, jointShoulderLeft.localPosition.z);
            jointShoulderRight.localPosition = new Vector3(jointShoulderRight.localPosition.x, .276f, jointShoulderRight.localPosition.z); 
        }
        jointElbowLeft.localRotation = Quaternion.Euler(0, 0, 0);
        jointElbowRight.localRotation = Quaternion.Euler(0, 0, 0);
        jointKneeLeft.localRotation = Quaternion.Euler(0, 0, 0);
        jointKneeRight.localRotation = Quaternion.Euler(0, 0, 0);
        jointHipLeft.localRotation = Quaternion.Euler(0, 0, 0);
        jointHipRight.localRotation = Quaternion.Euler(0, 0, 0);


        float waveSpine = Mathf.Sin(idleAnimTimer) * .03f;
        jointSpine.localPosition = new Vector3(0, waveSpine, 0);

        
    }
    void AirAnim()
    {

    }
}
