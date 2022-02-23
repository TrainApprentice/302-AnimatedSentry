using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SentryAttacks : MonoBehaviour
{
    public GameObject missile, reticle;
    public Transform target;

    private float missileRainOffset = .2f;
    private int randMissileNum = 0;

    private void Start()
    {
        target = FindObjectOfType<PlayerMovement>().transform;
    }
    private void Update()
    {
        if (Input.GetKeyDown("m"))
        {
            TargetedMissileAttack();
        }
        if(Input.GetKeyDown("r"))
        {
            MissileRainAttack(false);
            randMissileNum = (int)Random.Range(1, 4);
        }

        if(randMissileNum > 0)
        {
            missileRainOffset -= Time.deltaTime;
            if(missileRainOffset <= 0)
            {
                MissileRainAttack();
                missileRainOffset = Random.Range(0, .5f);
                randMissileNum--;
            }
        }
    }
    public void MissileRainAttack(bool doRand = true)
    {
        
        GameObject newMissile = Instantiate(missile);
        newMissile.GetComponent<MissileMovement>().isRainAttack = true;
        newMissile.GetComponent<MissileMovement>().target = target;
        
        if(doRand)
        {
            float randAngle = Random.Range(0, 360);
            float randDistance = Random.Range(3, 10);

            Vector3 offset = new Vector3(Mathf.Cos(randAngle), 0, Mathf.Sin(randAngle)) * randDistance;

            newMissile.GetComponent<MissileMovement>().offset = offset;
        }
        
        
           
        
        
    }
    public void TargetedMissileAttack()
    {
        GameObject newMissile = Instantiate(missile);
        newMissile.GetComponent<MissileMovement>().target = target;
    }
}
