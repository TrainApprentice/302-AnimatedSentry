using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetachJoint : MonoBehaviour
{
    private Transform storeParent;
    private Rigidbody rb;
    private BoxCollider hitbox;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        hitbox = GetComponent<BoxCollider>();
        storeParent = transform.parent;
    }

    public void Detach()
    {
        
        rb.useGravity = true;
        rb.constraints = RigidbodyConstraints.None;
        rb.AddRelativeForce(Vector3.up * .01f);
        hitbox.enabled = true;
        transform.SetParent(null);
    }

    public void Reattach()
    {
        rb.useGravity = false;
        hitbox.enabled = false;
        rb.velocity = Vector3.zero;
        transform.SetParent(storeParent);
    }
    

}
