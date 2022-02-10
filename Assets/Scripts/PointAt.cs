using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Axis
{
    Forward,
    Backward,
    Right,
    Left,
    Up,
    Down
}

public class PointAt : MonoBehaviour
{
    public Axis orientation;
    public PlayerTargeting playerTarget;
    private Quaternion startRot;

    // Start is called before the first frame update
    void Start()
    {
        playerTarget = GetComponentInParent<PlayerTargeting>();
        startRot = transform.localRotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerTarget.currTarget && playerTarget.playerWantsToAim)
        {
            Vector3 toTarget = playerTarget.currTarget.transform.position - transform.position;

            Vector3 fromVector = Vector3.zero;
            switch (orientation)
            {
                case Axis.Forward:
                    fromVector = Vector3.forward;
                    break;
                case Axis.Backward:
                    fromVector = Vector3.back;
                    break;
                case Axis.Right:
                    fromVector = Vector3.right;
                    break;
                case Axis.Left:
                    fromVector = Vector3.left;
                    break;
                case Axis.Up:
                    fromVector = Vector3.up;
                    break;
                case Axis.Down:
                    fromVector = Vector3.down;
                    break;
            }


            transform.rotation = Quaternion.FromToRotation(fromVector, toTarget);
        }
        else transform.rotation = startRot;
    }
}
