using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Human : Monsters
{
    [Header("人間の設定")]
    public int level = 1;
    [SerializeField] private float growthRate = 0.25f; // 二次関数の係数



    public void HumanSetUp()
    {
        hp = GetHP(level);
        atk = GetATK(level);

        monstarDeadPar = Resources.Load<GameObject>("Paticle/HumanDeadParticle");
    }

    public float GetHP(int level)
    {
        if (level < 1) level = 1;

        float _hp = hp + (growthRate * Mathf.Pow(level - 1, 2));
        return Mathf.Floor(_hp);
    }

    public float GetATK(int level)
    {
        if (level < 1)
        {
            level = 1;
            return atk;
        }

        float atkValue = atk * (1.3f + ((level - 1f) / 20f));
        return Mathf.Floor(atkValue);
    }

}
