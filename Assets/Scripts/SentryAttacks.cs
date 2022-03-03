using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SentryAttacks : MonoBehaviour
{
    public GameObject missile, reticle;
    public Transform target;
    public float currPhase = 0;

    public Transform jointNeck;
    public Transform missileLaunchPoint;

    public AudioClip missileFire;

    private AudioSource sfx;

    private float missileRainOffset = .2f;
    private int randMissileNum = 0;
    private SentryMovement mover;
    private TargetableObject healthManager;
    private PlayerMovement playerRef;
    private float cooldownToNextAttack = 3f;
    private float rainChance, missileChance, shockwaveChance;

    private void Start()
    {
        target = FindObjectOfType<PlayerMovement>().transform;
        mover = GetComponent<SentryMovement>();
        playerRef = FindObjectOfType<PlayerMovement>();
        healthManager = GetComponent<TargetableObject>();
        sfx = GetComponent<AudioSource>();
    }
    private void Update()
    {
        if(randMissileNum > 0)
        {
            RainAnim();
            missileRainOffset -= Time.deltaTime;
            if(missileRainOffset <= 0)
            {
                MissileRainAttack();
                missileRainOffset = .5f;
                randMissileNum--;
            }
        }

        if(!healthManager.isDead && !playerRef.isDead) RunAI();
    }

    private void RunAI()
    {
        if(!mover.isJumping) cooldownToNextAttack -= Time.deltaTime;
        else cooldownToNextAttack = 6 - currPhase;

        AssignAttackChances();

        if(cooldownToNextAttack <= 0)
        {
            float rand = Random.Range(0f,1f);

            if (rand < shockwaveChance) JumpShockwaveAttack();
            else if (rand < shockwaveChance + rainChance)
            {
                RainAnim();
                MissileRainAttack(false);
                randMissileNum = Random.Range(1, 4);
            }
            else if (rand < shockwaveChance + rainChance + missileChance) TargetedMissileAttack();
            else print("That didn't work");

            cooldownToNextAttack = 7 - currPhase;
        }
        jointNeck.localPosition = AnimMath.Ease(jointNeck.localPosition, new Vector3(0, 3.34f, 0), .001f);
        
    }

    private void AssignAttackChances()
    {
        if(Vector3.Distance(transform.position, playerRef.transform.position) < 15f)
        {
            shockwaveChance = .7f;
            rainChance = .3f;
            missileChance = 0;
        }
        else
        {
            shockwaveChance = (currPhase < 3) ? .1f : .05f;
            rainChance = (currPhase < 3) ? .4f : .65f;
            missileChance = (currPhase < 3) ? .5f : .3f;
        }
    }

    private void MissileRainAttack(bool doRand = true)
    {
        GameObject newMissile = Instantiate(missile, missileLaunchPoint.position, Quaternion.identity); ;
        newMissile.GetComponent<MissileMovement>().isRainAttack = true;
        newMissile.GetComponent<MissileMovement>().target = target;
        
        if (doRand)
        {
            float randAngle = Random.Range(0, 360);
            float randDistance = Random.Range(3, 10);

            Vector3 offset = new Vector3(Mathf.Cos(randAngle), 0, Mathf.Sin(randAngle)) * randDistance;

            newMissile.GetComponent<MissileMovement>().offset = offset;
        }

        jointNeck.localPosition -= jointNeck.transform.forward;
        sfx.clip = missileFire;
        sfx.Play();
    }
    private void TargetedMissileAttack()
    {
        GameObject newMissile = Instantiate(missile, missileLaunchPoint.position, Quaternion.identity);
        newMissile.GetComponent<MissileMovement>().target = target;

        jointNeck.localPosition -= jointNeck.transform.forward;
        sfx.clip = missileFire;
        sfx.Play();
    }

    private void JumpShockwaveAttack()
    {
        mover.nextJumpPoint = transform.position;
        mover.SetupJump();
    }

    public void ShiftPhase(int newPhase)
    {
        currPhase = newPhase;

        switch(currPhase)
        {
            case 1:
                mover.nextJumpPoint = mover.phase1Location.position;
                break;
            case 2:
                mover.nextJumpPoint = mover.phase2Location.position;
                break;
            case 3:
                mover.nextJumpPoint = mover.phase3Location.position;
                break;
        }
        mover.SetupJump();
    }

    private void RainAnim()
    {
        Quaternion neckGoal = Quaternion.Euler(-90, jointNeck.localRotation.eulerAngles.y, 0);
        jointNeck.localRotation = AnimMath.Ease(jointNeck.localRotation, neckGoal, .001f);

    }

    
    
}
