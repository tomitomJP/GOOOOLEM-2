using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Peace : MonoBehaviour
{

    public bool check = false;
    public bool selecting = false;
    public int peaceNumber = 0;
    [SerializeField] SpriteRenderer checkingSprite;
    [SerializeField] SpriteRenderer selectingSprite;
    SpriteRenderer peaceSprite;
    void Start()
    {
        peaceSprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (check)
        {
            checkingSprite.enabled = true;
            peaceSprite.sortingOrder = 4;
        }
        else if (selecting)
        {
            selectingSprite.enabled = true;
            peaceSprite.sortingOrder = 4;
        }
        else
        {
            if (check == false)
            {
                checkingSprite.enabled = false;

            }
            if (selecting == false)
            {
                selectingSprite.enabled = false;
            }
            peaceSprite.sortingOrder = 0;
        }
    }
}
