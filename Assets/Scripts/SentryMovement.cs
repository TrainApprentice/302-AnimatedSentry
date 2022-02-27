using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SentryMovement : MonoBehaviour
{
    CharacterController pawn;

    public Transform phase1Location, phase2Location, phase3Location;

    private Vector3 nextJumpPoint;

    private float jumpTimer = 0f;
    private bool isJumping = false;
    
    // Start is called before the first frame update
    void Start()
    { 

    }

    // Update is called once per frame
    void Update()
    {
       
    }

    void JumpToLocation(Vector3 jumpGoal, Vector3 control, float timer)
    {
        Vector3 a = AnimMath.Lerp(transform.position, control, timer);
        Vector3 b = AnimMath.Lerp(control, jumpGoal, timer);

        transform.position = AnimMath.Lerp(a, b, timer);
    }

}
