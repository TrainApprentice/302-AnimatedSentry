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
        if (isDead) Destroy(gameObject);
    }

    public void ApplyDamage(int damage)
    {
        currHealth -= damage;
        if (currHealth <= 0) isDead = true;
    }

    
}
