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
        
    }

    void UpdateHealthBar()
    {
        //if(healthStorage.currHealth < healthStorage.prevHealth)
    }
}
