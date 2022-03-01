using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SentryUI : MonoBehaviour
{
    TargetableObject healthStorage;
    SentryAttacks aiBase;

    public Image healthBar;
    // Start is called before the first frame update
    void Start()
    {
        healthStorage = GetComponent<TargetableObject>();
        aiBase = GetComponent<SentryAttacks>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateHealthBar();
        UpdateAIPhases();
    }

    void UpdateHealthBar()
    {
        if(healthStorage.currHealth < healthStorage.prevHealth)
        {
            healthStorage.prevHealth = AnimMath.Ease(healthStorage.prevHealth, healthStorage.currHealth, .01f);
            healthBar.transform.localScale = new Vector3(healthStorage.prevHealth / healthStorage.maxHealth, 1, 1);
        }
    }
    void UpdateAIPhases()
    {
        if (healthStorage.currHealth < 90 && aiBase.currPhase < 1) aiBase.ShiftPhase(1);
        if (healthStorage.currHealth < 60 && aiBase.currPhase < 2) aiBase.ShiftPhase(2);
        if (healthStorage.currHealth < 30 && aiBase.currPhase < 3) aiBase.ShiftPhase(3);
    }
}
