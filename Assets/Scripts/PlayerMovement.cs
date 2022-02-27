using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    private GameObject cam;
    CharacterController pawn;
    private float speed = 7f;
    private float currDodgeCooldown = 0f;
    private float baseDodgeCooldown = 0f;
    

    public Transform jointHips, jointHipLeft, jointHipRight, jointKneeLeft, jointKneeRight;
    public Transform jointSpine, jointNeck, jointHairLeft, jointHairRight;
    public Transform jointShoulderLeft, jointShoulderRight, jointElbowLeft, jointElbowRight;
    public Transform skeletonBase;
    public bool isDead = false;
    PlayerTargeting playerTargeting;


    float walkAnimTimer = 0f;
    float idleAnimTimer = 0f;
    float airAnimTimer = 0f;
    private Vector3 inputDir;
    private Vector3 storedDirection;
    private float velocityVertical = 0;
    private float gravMult = -9.8f;
    private bool isInvincible = false;
    private bool isGrounded = true;
    private DetachJoint[] allJointDetachScripts;

    // Start is called before the first frame update
    void Start()
    {
        pawn = GetComponent<CharacterController>();
        cam = FindObjectOfType<CameraController>().gameObject;
        playerTargeting = GetComponent<PlayerTargeting>();

        allJointDetachScripts = FindObjectsOfType<DetachJoint>();
        print(allJointDetachScripts.Length);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isDead)
        {
            // Movement
            float v = Input.GetAxisRaw("Vertical");
            float h = Input.GetAxisRaw("Horizontal");

            bool playerIsAiming = (playerTargeting && playerTargeting.playerWantsToAim && playerTargeting.target);
            if (playerIsAiming)
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
            else if (cam && (h != 0 || v != 0) && currDodgeCooldown <= 0)
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

            isGrounded = (pawn.isGrounded || transform.position.y <= -1.36f);
            bool wantsToJump = Input.GetButtonDown("Jump");
            if (isGrounded)
            {
                velocityVertical = 0;
                airAnimTimer = 0;
                if (wantsToJump) velocityVertical += 7f;
            }
            velocityVertical += gravMult * Time.deltaTime;

            

            if (currDodgeCooldown > 0)
            {
                inputDir = storedDirection;
                speed = AnimMath.Ease(speed, 0f, .2f);
                isInvincible = true;
                Quaternion faceDirection = Quaternion.LookRotation(storedDirection);
                transform.rotation = AnimMath.Ease(transform.rotation, faceDirection, .0001f);
                currDodgeCooldown -= Time.deltaTime;
            }
            else
            {
                isInvincible = false;
                speed = 7f;
            }
            if (Input.GetKeyDown("left shift") && pawn.isGrounded)
            {
                if (currDodgeCooldown <= 0)
                {
                    speed = 12f;
                    storedDirection = inputDir;
                    currDodgeCooldown = .75f;
                    baseDodgeCooldown = currDodgeCooldown;
                }
            }

            Vector3 moveAmt = (inputDir * speed) + (Vector3.up * velocityVertical);

            pawn.Move(moveAmt * Time.deltaTime);

            if (isGrounded && inputDir != Vector3.zero && currDodgeCooldown <= 0) WalkAnim();
            else if (currDodgeCooldown > 0) DodgeAnim();
            else if (!isGrounded) AirAnim();
            else IdleAnim();
        }
        else DeathAnim();


        // DEBUG ONLY

        if (Input.GetKeyDown("k"))
        {
            isDead = true;
            pawn.enabled = false;
        }
    }
    
    public void TakeHit()
    {
        if(!isInvincible) isDead = true;
    }
    void Restart()
    {
        
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
        jointSpine.localRotation = AnimMath.Ease(jointSpine.localRotation, Quaternion.identity, .001f); 

        jointHips.localRotation = AnimMath.Ease(jointHips.localRotation, Quaternion.identity, .001f);

        if (!playerTargeting.target)
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
            jointElbowLeft.localRotation = Quaternion.identity;
            jointElbowRight.localRotation = Quaternion.identity;
            jointShoulderLeft.localPosition = new Vector3(jointShoulderLeft.localPosition.x, .276f, jointShoulderLeft.localPosition.z);
            jointShoulderRight.localPosition = new Vector3(jointShoulderRight.localPosition.x, .276f, jointShoulderRight.localPosition.z);
        }
        idleAnimTimer = 0f;
    }

    void IdleAnim()
    {
        idleAnimTimer += Time.deltaTime * speed/2;
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
        jointElbowLeft.localRotation =  AnimMath.Ease(jointElbowLeft.localRotation, Quaternion.identity, .0001f);
        jointElbowRight.localRotation = AnimMath.Ease(jointElbowRight.localRotation, Quaternion.identity, .0001f);
        jointKneeLeft.localRotation =   AnimMath.Ease(jointKneeLeft.localRotation, Quaternion.identity, .0001f);
        jointKneeRight.localRotation =  AnimMath.Ease(jointKneeRight.localRotation, Quaternion.identity, .0001f);
        jointHipLeft.localRotation =    AnimMath.Ease(jointHipLeft.localRotation, Quaternion.identity, .0001f);
        jointHipRight.localRotation =   AnimMath.Ease(jointHipRight.localRotation, Quaternion.identity, .0001f);


        float waveSpine = Mathf.Sin(idleAnimTimer) * .03f;
        jointSpine.localPosition = new Vector3(0, waveSpine, 0);
        jointSpine.localRotation = AnimMath.Ease(jointSpine.localRotation, Quaternion.identity, .001f);

        jointHips.localRotation = AnimMath.Ease(jointHips.localRotation, Quaternion.identity, .001f);

        walkAnimTimer = 0f;
    }
    void AirAnim()
    {
        if (airAnimTimer < 1.4f) airAnimTimer += Time.deltaTime;
        else airAnimTimer = 1.4f;

        Quaternion spineGoal;
        Quaternion neckGoal;
        Quaternion shouldersGoal;
        Quaternion elbowsGoal;
        Quaternion hipGoal;
        Quaternion kneesGoal;

        if(airAnimTimer < .7f)
        {
            spineGoal = Quaternion.Euler(-15, 0, 0);
            neckGoal = Quaternion.Euler(-20, 0, 0);
            shouldersGoal = Quaternion.Euler(-25, 0, 0);
            elbowsGoal = Quaternion.Euler(-45, 0, 0);
            hipGoal = Quaternion.Euler(15, 0, 0);
            kneesGoal = Quaternion.Euler(60, 0, 0);
        }
        else
        {
            spineGoal = Quaternion.Euler(15, 0, 0);
            neckGoal = Quaternion.Euler(20, 0, 0);
            shouldersGoal = Quaternion.Euler(90, 0, 0);
            elbowsGoal = Quaternion.Euler(-30, 0, 0);
            hipGoal = Quaternion.Euler(-15, 0, 0);
            kneesGoal = Quaternion.Euler(45, 0, 0);
        }

        jointSpine.localRotation = AnimMath.Ease(jointSpine.localRotation, spineGoal, .001f);

        jointNeck.localRotation = AnimMath.Ease(jointNeck.localRotation, neckGoal, .001f);

        jointShoulderLeft.localRotation = AnimMath.Ease(jointShoulderLeft.localRotation, shouldersGoal, .001f);
        jointShoulderRight.localRotation = AnimMath.Ease(jointShoulderRight.localRotation, shouldersGoal, .001f);

        jointElbowLeft.localRotation = AnimMath.Ease(jointElbowLeft.localRotation, elbowsGoal, .001f);
        jointElbowRight.localRotation = AnimMath.Ease(jointElbowRight.localRotation, elbowsGoal, .001f);

        jointHips.localRotation = AnimMath.Ease(jointHips.localRotation, hipGoal, .001f);

        jointHipLeft.localRotation = AnimMath.Ease(jointHipLeft.localRotation, Quaternion.identity, .001f);
        jointHipRight.localRotation = AnimMath.Ease(jointHipRight.localRotation, Quaternion.identity, .001f);

        jointKneeLeft.localRotation = AnimMath.Ease(jointKneeLeft.localRotation, kneesGoal, .001f);
        jointKneeRight.localRotation = AnimMath.Ease(jointKneeRight.localRotation, kneesGoal, .001f);

        float hairWave = Mathf.Sin(airAnimTimer * 2) * .1f;
        jointHairLeft.localPosition = new Vector3(jointHairLeft.localPosition.x, .5f + hairWave, jointHairLeft.localPosition.z);
        jointHairRight.localPosition = new Vector3(jointHairRight.localPosition.x, .5f + hairWave, jointHairRight.localPosition.z);

        
    }
    void DeathAnim()
    {
        foreach (DetachJoint j in allJointDetachScripts)
        {
            j.Detach();
        }
    }
    void DodgeAnim()
    {

        if(currDodgeCooldown > baseDodgeCooldown * .9f) 
        {
            
            jointSpine.localRotation = AnimMath.Ease(jointSpine.localRotation, Quaternion.identity, .0001f);

            jointNeck.localRotation = AnimMath.Ease(jointNeck.localRotation, Quaternion.identity, .0001f);

            jointShoulderLeft.localRotation = AnimMath.Ease(jointShoulderLeft.localRotation, Quaternion.Euler(90, 0, 0), .0001f);
            jointShoulderRight.localRotation = AnimMath.Ease(jointShoulderRight.localRotation, Quaternion.Euler(90, 0, 0), .0001f);

            jointElbowLeft.localRotation = AnimMath.Ease(jointElbowLeft.localRotation, Quaternion.identity, .0001f);
            jointElbowRight.localRotation = AnimMath.Ease(jointElbowRight.localRotation, Quaternion.identity, .0001f);

            jointHipLeft.localRotation = AnimMath.Ease(jointHipLeft.localRotation, Quaternion.identity, .0001f);
            jointHipRight.localRotation = AnimMath.Ease(jointHipRight.localRotation, Quaternion.identity, .0001f);

            jointKneeLeft.localRotation = AnimMath.Ease(jointKneeLeft.localRotation, Quaternion.identity, .0001f);
            jointKneeRight.localRotation = AnimMath.Ease(jointKneeRight.localRotation, Quaternion.identity, .0001f);

            skeletonBase.localRotation = AnimMath.Ease(skeletonBase.localRotation, Quaternion.identity, .001f);
        }
        else if(currDodgeCooldown > baseDodgeCooldown * .75f)
        { 
            Quaternion spineGoal = Quaternion.Euler(90, 0, 0);
            Quaternion neckGoal = Quaternion.Euler(30, 0, 0);
            Quaternion shouldersGoal = Quaternion.Euler(-50, 0, 0);
            Quaternion elbowsGoal = Quaternion.Euler(-70, 0, 0);
            Quaternion hipsGoal = Quaternion.Euler(-30, 0, 0);
            Quaternion kneesGoal = Quaternion.Euler(40, 0, 0);

            jointSpine.localRotation = AnimMath.Ease(jointSpine.localRotation, spineGoal, .0001f);

            jointNeck.localRotation = AnimMath.Ease(jointNeck.localRotation, neckGoal, .0001f);

            jointShoulderLeft.localRotation = AnimMath.Ease(jointShoulderLeft.localRotation, shouldersGoal, .0001f);
            jointShoulderRight.localRotation = AnimMath.Ease(jointShoulderRight.localRotation, shouldersGoal, .0001f);

            jointElbowLeft.localRotation = AnimMath.Ease(jointElbowLeft.localRotation, elbowsGoal, .0001f);
            jointElbowRight.localRotation = AnimMath.Ease(jointElbowRight.localRotation, elbowsGoal, .0001f);

            jointHipLeft.localRotation = AnimMath.Ease(jointHipLeft.localRotation, hipsGoal, .0001f);
            jointHipRight.localRotation = AnimMath.Ease(jointHipRight.localRotation, hipsGoal, .0001f);

            jointKneeLeft.localRotation = AnimMath.Ease(jointKneeLeft.localRotation, kneesGoal, .0001f);
            jointKneeRight.localRotation = AnimMath.Ease(jointKneeRight.localRotation, kneesGoal, .0001f);

        }
        if(currDodgeCooldown > baseDodgeCooldown * .2f)
        {
            Quaternion spinRotationGoal = Quaternion.Euler(320, 0, 0);
            skeletonBase.localRotation = AnimMath.Ease(skeletonBase.localRotation, spinRotationGoal, .005f, Time.deltaTime, false, false);
            if (currDodgeCooldown < baseDodgeCooldown * .6f) 
            {
                Vector3 skeletonGoalPosition = new Vector3(0, -1f, 0);
                skeletonBase.localPosition = AnimMath.Ease(skeletonBase.localPosition, skeletonGoalPosition, .001f);
            }
        }
        else
        {
            jointSpine.localRotation = AnimMath.Ease(jointSpine.localRotation, Quaternion.identity, .0001f);

            jointNeck.localRotation = AnimMath.Ease(jointNeck.localRotation, Quaternion.identity, .0001f);

            jointShoulderLeft.localRotation = AnimMath.Ease(jointShoulderLeft.localRotation, Quaternion.Euler(90, 0, 0), .0001f);
            jointShoulderRight.localRotation = AnimMath.Ease(jointShoulderRight.localRotation, Quaternion.Euler(90, 0, 0), .0001f);

            jointElbowLeft.localRotation = AnimMath.Ease(jointElbowLeft.localRotation, Quaternion.identity, .001f);
            jointElbowRight.localRotation = AnimMath.Ease(jointElbowRight.localRotation, Quaternion.identity, .001f);

            jointHipLeft.localRotation = AnimMath.Ease(jointHipLeft.localRotation, Quaternion.identity, .0001f);
            jointHipRight.localRotation = AnimMath.Ease(jointHipRight.localRotation, Quaternion.identity, .0001f);

            jointKneeLeft.localRotation = AnimMath.Ease(jointKneeLeft.localRotation, Quaternion.identity, .0001f);
            jointKneeRight.localRotation = AnimMath.Ease(jointKneeRight.localRotation, Quaternion.identity, .0001f);

            skeletonBase.localRotation = AnimMath.Ease(skeletonBase.localRotation, Quaternion.identity, .0001f);
            skeletonBase.localPosition = AnimMath.Ease(skeletonBase.localPosition, Vector3.zero, .0001f);

        }
        if(currDodgeCooldown <= .01f)
        {
            jointSpine.localRotation = Quaternion.identity;

            jointNeck.localRotation = Quaternion.identity;

            jointShoulderLeft.localRotation = Quaternion.Euler(90, 0, 0);
            jointShoulderRight.localRotation = Quaternion.Euler(90, 0, 0);

            jointElbowLeft.localRotation = Quaternion.identity;
            jointElbowRight.localRotation = Quaternion.identity;

            jointHipLeft.localRotation = Quaternion.identity;
            jointHipRight.localRotation = Quaternion.identity;

            jointKneeLeft.localRotation = Quaternion.identity;
            jointKneeRight.localRotation = Quaternion.identity;

            skeletonBase.localRotation = Quaternion.identity;
            skeletonBase.localPosition = Vector3.zero;
        }
        walkAnimTimer = 0f;
        idleAnimTimer = 0f;
    }
}
