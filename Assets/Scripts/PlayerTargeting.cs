using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTargeting : MonoBehaviour
{
    public float visionRadius = 15f;

    public TargetableObject currTarget { get; private set; }
    public bool playerWantsToAim { get; private set; } = false;

private List<TargetableObject> validTargets = new List<TargetableObject>();
    private float cooldownScan = .5f;
    private float cooldownPick = .5f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        playerWantsToAim = Input.GetMouseButton(1);

        if (cooldownScan > 0) cooldownScan -= Time.deltaTime;
        if (cooldownPick > 0) cooldownPick -= Time.deltaTime;

        if (playerWantsToAim)
        {
            if(cooldownScan <= 0) ScanForTargets();
            if (cooldownPick <= 0) PickTarget();
        }
        else
        {
            currTarget = null;
        }
        
    }

    void ScanForTargets()
    {
        cooldownScan = .5f;
        validTargets.Clear();

        TargetableObject[] allTargets = GameObject.FindObjectsOfType<TargetableObject>();

        foreach(TargetableObject t in allTargets)
        {
            Vector3 toTarget = t.transform.position - transform.position;
            if(toTarget.sqrMagnitude <= visionRadius * visionRadius)
            {
                float alignment = Vector3.Dot(transform.forward, toTarget.normalized);
                if(alignment > .3333f) validTargets.Add(t);
            }
        }


    }

    void PickTarget()
    {
        if (currTarget) return;
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
        currTarget = closestTarget;
    }
}
