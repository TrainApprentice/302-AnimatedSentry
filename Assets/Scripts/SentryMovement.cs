using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SentryMovement : MonoBehaviour
{
    CharacterController pawn;

    public Transform phase1Location, phase2Location, phase3Location;

    private Vector3 lastJumpPoint;
    private Vector3 nextJumpPoint;
    private Vector3 currControlPoint;

    private float jumpTimer = 0f;
    private float jumpLength = 2f;
    private bool isJumping = false;
    
    // Start is called before the first frame update
    void Start()
    { 

    }

    // Update is called once per frame
    void Update()
    {
        if (isJumping) {
            jumpTimer += Time.deltaTime;
            float p = jumpTimer / jumpLength;
            p = Mathf.Clamp(p, 0, 1);
            transform.position = FindPointOnCurve(p);
            if (jumpTimer >= jumpLength) isJumping = false;
        }
        else
        {
            jumpTimer = jumpLength;
            lastJumpPoint = Vector3.zero;
            nextJumpPoint = Vector3.zero;
            currControlPoint = Vector3.zero;
        }
        if(Input.GetKeyDown("j") && !isJumping)
        {
            //Vector3 randJumpPoint = new Vector3(Random.Range(-5, 6) + transform.position.x, 0, Random.Range(-5, 6) + transform.position.z);
            Vector3 dirToJumpPoint = nextJumpPoint - lastJumpPoint;
            nextJumpPoint = phase1Location.position;
            lastJumpPoint = transform.position;

            

            Vector3 controlPoint = lastJumpPoint + new Vector3(dirToJumpPoint.x/2, 30f, dirToJumpPoint.z/2);


            currControlPoint = controlPoint;
            isJumping = true;
            jumpTimer = 0f;

        }

    }

    void JumpToLocation(float timer)
    {
        
        Vector3 a = AnimMath.Lerp(lastJumpPoint, currControlPoint, timer);
        Vector3 b = AnimMath.Lerp(currControlPoint, nextJumpPoint, timer);

        transform.position = AnimMath.Lerp(a, b, timer);
        
    }

    Vector3 FindPointOnCurve(float p)
    {
        Vector3 a = AnimMath.Lerp(lastJumpPoint, currControlPoint, p);
        Vector3 b = AnimMath.Lerp(currControlPoint, nextJumpPoint, p);

        return AnimMath.Lerp(a, b, p);
    }

    private void OnDrawGizmos()
    {
        for (int i = 0; i < 20; i++)
        {
            Vector3 a = FindPointOnCurve(i / 20f);
            Vector3 b = FindPointOnCurve((i + 1) / 20f);

            Gizmos.DrawLine(a, b);
        }
    }

}
