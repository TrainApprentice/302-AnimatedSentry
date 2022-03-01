using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentManager : MonoBehaviour
{

    public GameObject wall1, wall2;
    public SentryAttacks sentryBase;

    private void Update()
    {
        if (sentryBase.currPhase > 1 && wall1) SetWallToFall(wall1);
        if (sentryBase.currPhase > 2 && wall2) SetWallToFall(wall2);
        
    }

    void SetWallToFall(GameObject wall)
    {
        if(wall.GetComponent<BoxCollider>().enabled) wall.GetComponent<BoxCollider>().enabled = false;
        if(!wall.GetComponent<Rigidbody>().useGravity) wall.GetComponent<Rigidbody>().useGravity = true;

        if (wall.transform.position.y < -20) Destroy(wall);
    }
}
