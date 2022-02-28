using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SentryUI : MonoBehaviour
{
    TargetableObject healthStorage;

    public Image healthBar;
    // Start is called before the first frame update
    void Start()
    {
        healthStorage = GetComponent<TargetableObject>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateHealthBar();
    }

    void UpdateHealthBar()
    {
        if(healthStorage.currHealth < healthStorage.prevHealth)
        {
            healthStorage.prevHealth = AnimMath.Ease(healthStorage.prevHealth, healthStorage.currHealth, .01f);
            healthBar.transform.localScale = new Vector3(healthStorage.prevHealth / healthStorage.maxHealth, 1, 1);
        }
    }
}
