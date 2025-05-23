using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] float[] HouseHealth = { 300, 300 };
    float[] HouseHealth_Max = { 300, 300 };

    [SerializeField] Slider[] HouseHealth_slider;
    [SerializeField] Text[] HouseHealth_text;

    void Start()
    {
        for (int i = 0; i < 2; i++)
        {
            HouseHealth_Max[i] = HouseHealth[i];
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < 2; i++)
        {
            HouseHealth_slider[i].value = HouseHealth[i] / HouseHealth_Max[i];
            HouseHealth_text[i].text = HouseHealth[i] + "/" + HouseHealth_Max[i];
        }
    }


    public void HouseAtk(int playerNum, float dmg)
    {
        playerNum %= 2;

        Debug.Log(playerNum);
        HouseHealth[playerNum] -= dmg;
        HouseHealth[playerNum] = Mathf.Clamp(HouseHealth[playerNum], 0, HouseHealth_Max[playerNum]);
    }
}
