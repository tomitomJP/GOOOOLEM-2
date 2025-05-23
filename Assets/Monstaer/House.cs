using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class House : Monsters
{
    [SerializeField] Slider hpBar;

    [SerializeField] Text hpBarText;
    void Start()
    {
        StartSetup();
    }

    // Update is called once per frame
    void Update()
    {
        hpBar.value = hp / 1000f;
        hpBarText.text = Mathf.Floor(hp).ToString() + "/" + "1000";
    }
}
