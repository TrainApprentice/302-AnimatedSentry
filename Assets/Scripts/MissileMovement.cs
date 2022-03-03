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
    private float flySpeed = 40f;
    private GameObject currRet;
    private float lifespan = 5f;

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
            transform.position = target.position + new Vector3(0, 100f, 0) + offset;
            flySpeed = 50f;

            Ray ray = new Ray();
            ray.origin = transform.position;
            ray.direction = direction;
            Vector3 retPos = Vector3.zero;

            RaycastHit hit;


            if (Physics.Raycast(ray, out hit))
            {
                retPos = hit.point; 
            }


            currRet = Instantiate(reticleBase, retPos, Quaternion.identity);
        }

    }

    // Update is called once per frame
    void Update()
    {
        transform.position += direction * flySpeed * Time.deltaTime;
        if (lifespan > 0) lifespan -= Time.deltaTime;
        else Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            other.GetComponent<PlayerMovement>().TakeHit();
            GameObject newExplosion = Instantiate(explosionBase, transform.position, Quaternion.identity);
            Destroy(gameObject);
            if (isRainAttack) Destroy(currRet);
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Wall"))
        {
            GameObject newExplosion = Instantiate(explosionBase, contactPoint.position, Quaternion.identity);
            Destroy(gameObject);
            if (isRainAttack) Destroy(currRet);
        }
    }


}
