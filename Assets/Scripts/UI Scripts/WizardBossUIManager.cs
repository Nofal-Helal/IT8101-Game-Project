using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WizardBossUIManager : MonoBehaviour
{
    private WizardBoss boss;
    public TextMeshProUGUI bossName;
    public TextMeshProUGUI bossHealth;
    // Start is called before the first frame update
    void Start()
    {
        boss = FindObjectOfType<WizardBoss>();
        bossName.text = "Wizard";
    }

    // Update is called once per frame
    void Update()
    {
        bossHealth.text = boss.health + " / " + boss.maxHealth;
        if (boss.health < 0)
        {
            gameObject.SetActive(false);
        }
    }
}
