using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetableObject : MonoBehaviour
{
    public float currHealth, prevHealth, maxHealth;
    private bool isDead = false;

    private void Start()
    {
        currHealth = maxHealth;
    }
    // Update is called once per frame
    void Update()
    {
        if (prevHealth <= 0) isDead = true;
        if (isDead) Destroy(gameObject);
    }

    public void ApplyDamage(int damage)
    {
        prevHealth = currHealth;
        currHealth -= damage;
    }

    
}
