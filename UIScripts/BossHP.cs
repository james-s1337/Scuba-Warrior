using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BossHP : MonoBehaviour
{
    [SerializeField] GameObject bossHealthUI;
    [SerializeField] TextMeshProUGUI txt;
    [SerializeField] TextMeshProUGUI mainTxt;
    [SerializeField] Image bar;
    [SerializeField] int defaultWidth;
    Enemy boss;
    bool bossBattle;
    int maxHP;
    void Start()
    {
        bossHealthUI.SetActive(false);
        bossBattle = false;
    }

    void Update()
    {
        if (bossBattle)
        {
            if (maxHP == 0)
            {
                maxHP = boss.getMaxHP();
            }
            updateBar(boss.getCurrentHP());
        }
    }

    public void setMaxHP(int max, string name, GameObject enemy)
    {
        maxHP = max;
        txt.text = name;
        mainTxt.text = name;
        boss = enemy.GetComponent<Enemy>();
        maxHP = boss.getMaxHP();
        updateBar(maxHP);
        bossHealthUI.SetActive(true);
        bossBattle = true;
    }

    public void updateBar(int newHP)
    {
        float percentage = Mathf.Clamp((float) newHP/maxHP, 0, 1);
        bar.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, defaultWidth * percentage);

        if (percentage == 0)
        {
            bossBattle = false;
            bossHealthUI.SetActive(false);
        }
    }
}
