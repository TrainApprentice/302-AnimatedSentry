using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SentryMovement : MonoBehaviour
{
    

    public Transform phase1Location, phase2Location, phase3Location;
    public GameObject explosionBase;
    public Vector3 nextJumpPoint;
    public bool isJumping = false;
    public bool sameJumpLocation = false;

    public Transform jointHips, jointLeftKnee, jointRightKnee, jointLeftHip, jointRightHip, jointBack, jointNeck, jointEyes;

    private Vector3 lastJumpPoint;
    private Vector3 currControlPoint;

    private float jumpTimer = 0f;
    private float jumpLength = 3f;

    private float idleAnimTimer = 0;

    private void Start()
    {
        phase1Location = GameObject.Find("Phase1").transform;
        phase2Location = GameObject.Find("Phase2").transform;
        phase3Location = GameObject.Find("Phase3").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (isJumping) {

            JumpAnim();
            jumpTimer += Time.deltaTime;
            float p = jumpTimer / jumpLength;
            p = Mathf.Clamp(p, 0, 1);
            transform.position = FindPointOnCurve(p);
            if (jumpTimer >= jumpLength)
            {
                GameObject newExplosion = Instantiate(explosionBase, nextJumpPoint, Quaternion.identity);
                newExplosion.GetComponent<ExplosionBehavior>().maxLife = 1.5f;
                newExplosion.GetComponent<ExplosionBehavior>().maxRadius = 45f;
                isJumping = false;
            }
        }
        else
        {
            jumpTimer = jumpLength;
            lastJumpPoint = Vector3.zero;
            nextJumpPoint = Vector3.zero;
            currControlPoint = Vector3.zero;
            IdleAnim();
        }

    }

    public void SetupJump()
    {

        Vector3 dirToJumpPoint = nextJumpPoint - lastJumpPoint;
        lastJumpPoint = transform.position;


        sameJumpLocation = lastJumpPoint == nextJumpPoint;
        Vector3 controlPoint = (sameJumpLocation) ? new Vector3(transform.position.x, 50f, transform.position.z) : lastJumpPoint + new Vector3(dirToJumpPoint.x / 2, 50f, dirToJumpPoint.z / 2);


        currControlPoint = controlPoint;
        isJumping = true;
        jumpTimer = 0f;

    }

    Vector3 FindPointOnCurve(float p)
    {
        Vector3 a = AnimMath.Lerp(lastJumpPoint, currControlPoint, p);
        Vector3 b = AnimMath.Lerp(currControlPoint, nextJumpPoint, p);

        return AnimMath.Lerp(a, b, p);
    }

    private void JumpAnim()
    {
        Quaternion spineGoal;
        Quaternion hipGoal;
        Quaternion leftHipGoal;
        Quaternion kneesGoal;
        Quaternion mainRotGoal = Quaternion.Euler(0, 180, 0);
        if (jumpTimer <= jumpLength/2)
        {
            spineGoal = Quaternion.Euler(-20, 0, 0);
            hipGoal = Quaternion.Euler(15, 0, 0);
            kneesGoal = Quaternion.Euler(25, 0, 0);
            leftHipGoal = Quaternion.identity;
        }
        else
        {
            spineGoal = Quaternion.Euler(15, 0, 0);
            hipGoal = Quaternion.identity;
            kneesGoal = Quaternion.Euler(45, 0, 0);
            leftHipGoal = Quaternion.Euler(-45, 0, 0);
        }

        if (!sameJumpLocation)
        {
            transform.rotation = AnimMath.Ease(transform.rotation, mainRotGoal, .0001f);
            jointNeck.localRotation = AnimMath.Ease(jointNeck.localRotation, Quaternion.identity, .0001f);
        }

        jointBack.localRotation = AnimMath.Ease(jointBack.localRotation, spineGoal, .001f);
        jointHips.localRotation = AnimMath.Ease(jointHips.localRotation, hipGoal, .001f);
        jointLeftHip.localRotation = AnimMath.Ease(jointLeftHip.localRotation, leftHipGoal, .001f);

        jointLeftKnee.localRotation = AnimMath.Ease(jointLeftKnee.localRotation, kneesGoal, .001f);
        jointRightKnee.localRotation = AnimMath.Ease(jointRightKnee.localRotation, kneesGoal, .001f);

        idleAnimTimer = 0;
    }

    private void IdleAnim()
    {
        transform.rotation = AnimMath.Ease(transform.rotation, Quaternion.identity, .001f);
        idleAnimTimer += Time.deltaTime;

        float wave = Mathf.Sin(idleAnimTimer);

        jointBack.localRotation = Quaternion.Euler(wave * 2.5f + 2.5f, 0, 0);
        jointEyes.localPosition = new Vector3(jointEyes.localPosition.x, 2.38f + (wave * .1f), jointEyes.localPosition.z);


        float legsWave = wave * 10 + 15;
        jointLeftHip.localRotation = Quaternion.Euler(-legsWave * 2, 0, 0);
        jointRightHip.localRotation = Quaternion.Euler(-legsWave * 2, 0, 0);

        jointLeftKnee.localRotation = Quaternion.Euler(legsWave, 0, 0);
        jointRightKnee.localRotation = Quaternion.Euler(legsWave, 0, 0);

        jointHips.localRotation = Quaternion.Euler(legsWave, 0, 0);
        jointHips.localPosition = new Vector3(0, -wave * .1f - .1f, 0);
    }

    

}
