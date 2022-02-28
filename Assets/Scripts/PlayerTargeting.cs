using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTargeting : MonoBehaviour
{
    public float visionRadius = 20f;
    [Range(1, 20)]
    public int roundsPerSecond = 3;
    public TargetableObject target { get; private set; }
    public bool playerWantsToAim { get; private set; } = false;
    public bool playerWantsToAttack { get; private set; } = false;

    public PointAt jointShoulderRight, jointShoulderLeft, jointNeck;
    public PlayerMovement controller;
   

    private List<TargetableObject> validTargets = new List<TargetableObject>();
    private float cooldownScan = 0;
    private float cooldownPick = 0;
    private float cooldownAttack = 0;

    private CameraController cam;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
        cam = FindObjectOfType<CameraController>();
    }

    // Update is called once per frame
    void Update()
    {
        playerWantsToAim = Input.GetMouseButton(1);
        playerWantsToAttack = Input.GetMouseButton(0);

        if (cooldownScan > 0) cooldownScan -= Time.deltaTime;
        if (cooldownPick > 0) cooldownPick -= Time.deltaTime;
        if (cooldownAttack > 0) cooldownAttack -= Time.deltaTime;
        if (!controller.isInvincible && !controller.isDead)
        {
            if (playerWantsToAim)
            {

                if (target != null)
                {
                    Vector3 toTarget = target.transform.position - transform.position;
                    toTarget.y = 0;
                    if (!CanSeeThing(target) && toTarget.magnitude > 5) target = null;
                }

                if (cooldownScan <= 0) ScanForTargets();
                if (cooldownPick <= 0) PickTarget();
            }
            else
            {
                target = null;
            }

            if (jointShoulderLeft) jointShoulderLeft.target = (target) ? target.transform : null;
            if (jointShoulderRight) jointShoulderRight.target = (target) ? target.transform : null;
            if (jointNeck) jointNeck.target = (target) ? target.transform : null;

            if (playerWantsToAttack) DoAttack();
        }

    }

    void ScanForTargets()
    {
        cooldownScan = .5f;
        validTargets.Clear();

        TargetableObject[] allTargets = GameObject.FindObjectsOfType<TargetableObject>();

        foreach(TargetableObject t in allTargets)
        {
            if (CanSeeThing(t)) validTargets.Add(t);
        }


    }

    private bool CanSeeThing(TargetableObject t)
    {
        Vector3 toTarget = t.transform.position - transform.position;
        if (toTarget.sqrMagnitude > visionRadius * visionRadius) return false;
        
        float alignment = Vector3.Dot(transform.forward, toTarget.normalized);
        if (alignment < .3333f) return false;

        Ray ray = new Ray();
        ray.origin = transform.position;
        ray.direction = toTarget;

        RaycastHit hit;

        if(Physics.Raycast(ray, out hit, visionRadius))
        { 
            if (!hit.collider.CompareTag("Enemy")) return false;
        }
        

        return true;
        
    }

    void PickTarget()
    {
        if (target) return;
        float closestDist = visionRadius;
        TargetableObject closestTarget = new TargetableObject();
        foreach(TargetableObject t in validTargets)
        {
            float testDist = Vector3.Distance(transform.position, t.transform.position);
            if(testDist < closestDist)
            {
                closestDist = testDist;
                closestTarget = t;
            }
        }
        target = closestTarget;
    }

    void DoAttack()
    {
        if (!playerWantsToAim) return;
        if (target == null) return;
        if (cooldownAttack > 0) return;
        if (!CanSeeThing(target)) return;
        if (controller.isInvincible) return;

        cooldownAttack = 1f/roundsPerSecond;

        jointShoulderLeft.transform.localEulerAngles += new Vector3(-20, 0, 0);
        jointShoulderRight.transform.localEulerAngles += new Vector3(-20, 0, 0);

        target.ApplyDamage(2);
        cam.DoShake(.3f, .999f);
    }
}
