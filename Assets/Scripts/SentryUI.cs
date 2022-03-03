using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SentryUI : MonoBehaviour
{
    TargetableObject healthStorage;
    SentryAttacks aiBase;

    private DetachJoint[] sentryJointDetachScripts;
    private string nameAndTitle;

    private string[] names = new string[10] { "Glorpo", "Jeff", "Anais", "Jack", "Arnold", "Zote", "Valak", "Maltera", "Ibrido", "Katrik" };
    private string[] titles = new string[10] { "Magnificent", "Radiant", "Ascended", "Enigmatic", "Terrifying", "Powerful", "Diligent", "Almighty", "Unbroken", "Grand" };

    public Image healthBar;
    public TMP_Text sentryName;
    // Start is called before the first frame update
    void Start()
    {
        healthStorage = GetComponent<TargetableObject>();
        aiBase = GetComponent<SentryAttacks>();

        sentryJointDetachScripts = GetComponentsInChildren<DetachJoint>();

        if (!healthBar) healthBar = FindObjectOfType<Image>();
        if (!sentryName) sentryName = FindObjectOfType<TMP_Text>();

        RandomizeName();
    }

    
    // Update is called once per frame
    void Update()
    {
        if (!healthStorage.isDead)
        {
            UpdateHealthBar();
            UpdateAIPhases();
        }
        else DeathAnim();
        


    }

    private void DeathAnim()
    {
        foreach(DetachJoint j in sentryJointDetachScripts)
        {
            j.Detach();
        }
    }

    void UpdateHealthBar()
    {
        if(healthStorage.currHealth < healthStorage.prevHealth)
        {
            healthStorage.prevHealth = AnimMath.Ease(healthStorage.prevHealth, healthStorage.currHealth, .01f);
            healthBar.transform.localScale = new Vector3(healthStorage.prevHealth / healthStorage.maxHealth, 1, 1);
        }
        if(healthStorage.currHealth == healthStorage.maxHealth)
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

    private void RandomizeName()
    {
        nameAndTitle = "";
        int randName = Random.Range(0, 10);
        nameAndTitle += names[randName] + ", the ";
        if (randName != 5)
        {
            int randTitle = Random.Range(0, 10);
            nameAndTitle += titles[randTitle];
        }
        else
        {
            for(int i = 0; i < titles.Length; i++)
            {
                if (i != 9) nameAndTitle += titles[i] + ", ";
                else nameAndTitle += titles[i];
            }
        }

        sentryName.text = nameAndTitle;
    }


}
