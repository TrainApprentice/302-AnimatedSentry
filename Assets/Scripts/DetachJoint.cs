using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetachJoint : MonoBehaviour
{
    private Transform storeParent;
    private Rigidbody rb;
    private BoxCollider collider;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        collider = GetComponent<BoxCollider>();
        storeParent = transform.parent;
    }

    public void Detach()
    {
        
        rb.useGravity = true;
        rb.AddRelativeForce(Vector3.up * .01f);
        collider.enabled = true;
        transform.SetParent(null);
    }

    public void Reattach()
    {
        rb.useGravity = false;
        collider.enabled = false;
        transform.SetParent(storeParent);
    }
    

}
