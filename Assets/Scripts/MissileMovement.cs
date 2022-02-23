using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileMovement : MonoBehaviour
{
    public Transform target;
    public Transform contactPoint;
    public Vector3 offset;
    public bool isRainAttack = false;
    public GameObject explosionBase, reticleBase;

    private Vector3 direction;
    private float flySpeed = 25f;
    private GameObject currRet;

    // Start is called before the first frame update
    void Start()
    {
        if (!isRainAttack)
        {
            direction = target.position - transform.position;
            direction.Normalize();
            transform.LookAt(target);
        }
        else
        {
            direction = Vector3.down;
            transform.eulerAngles = new Vector3(90, 0, 0);
            transform.position = target.position + new Vector3(0, 70f, 0) + offset;
            flySpeed = 50f;
            currRet = Instantiate(reticleBase, target.position - new Vector3(0, 1f, 0) + offset, Quaternion.identity);
        }

    }

    // Update is called once per frame
    void Update()
    {
        transform.position += direction * flySpeed * Time.deltaTime;
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            GameObject newExplosion = Instantiate(explosionBase, transform.position, Quaternion.identity);
            Destroy(gameObject);
            if (isRainAttack) Destroy(currRet);
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Player"))
        {
            GameObject newExplosion = Instantiate(explosionBase, contactPoint.position, Quaternion.identity);
            Destroy(gameObject);
            if (isRainAttack) Destroy(currRet);
        }
        
        
    }


}
