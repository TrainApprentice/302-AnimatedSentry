using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionBehavior : MonoBehaviour
{
    private float maxLife = .75f;
    private float lifespan;
    private float radius;
    private float maxRadius = 15f;
    private Material mat;

    private void Start()
    {
        mat = GetComponent<MeshRenderer>().material;
        lifespan = maxLife;
    }

    private void Update()
    {
        lifespan -= Time.deltaTime;

        radius = (maxLife - lifespan) * (1/maxLife) * maxRadius;
        transform.localScale = new Vector3(radius, radius, radius);
        mat.color = new Color(mat.color.r, mat.color.g, mat.color.b, lifespan/maxLife);


        if (lifespan <= 0) Destroy(gameObject);
    }
}
