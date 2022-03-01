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

    private Vector3 lastJumpPoint;
    private Vector3 currControlPoint;

    private float jumpTimer = 0f;
    private float jumpLength = 2f;

    // Update is called once per frame
    void Update()
    {
        if (isJumping) {
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
        }

    }

    public void SetupJump()
    {

        Vector3 dirToJumpPoint = nextJumpPoint - lastJumpPoint;
        lastJumpPoint = transform.position;



        Vector3 controlPoint = (lastJumpPoint == nextJumpPoint) ? new Vector3(transform.position.x, 30f, transform.position.z) : lastJumpPoint + new Vector3(dirToJumpPoint.x / 2, 30f, dirToJumpPoint.z / 2);


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
