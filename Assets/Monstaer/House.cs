using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class House : Monsters
{
    [SerializeField] Slider hpBar;

    [SerializeField] Text hpBarText;
    [SerializeField] float hpMax;
    [SerializeField] Transform monstersPearent;
    [SerializeField] BoxCollider2D boxCollider2D;
    void Start()
    {

        StartSetup();
        hpMax = hp;
    }

    // Update is called once per frame
    void Update()
    {
        hp = Mathf.Clamp(hp, 0, hpMax);
        hpBar.value = hp / hpMax;
        hpBarText.text = Mathf.Floor(hp).ToString() + "/" + hpMax;

        if (monstersPearent.childCount > 0)
        {
            boxCollider2D.enabled = false;
        }
        else
        {
            boxCollider2D.enabled = true;

        }
    }
}
