using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Angel : Monsters
{
    [SerializeField] GameObject impact;
    void Start()
    {
        StartSetup();
    }

    // Update is called once per frame
    void Update()
    {
        if (mode != Mode.atk)
        {
            Move();
        }
        else
        {
            MoveAniTimer = MoveAniTime;
        }

        if (0 >= hp)
        {
            Dead();
        }
        RaycastHit2D[] hit;
        if (Physics2D.Raycast(rayOrigin, transform.right, Mathf.Infinity, enemyLayer))
        {
            RandomAtk();
        }
        UpdateStatuses();
    }

    float time = 2;
    float timer = 0;
    float AtkTriggerRate = 60;

    void RandomAtk()
    {
        if (time <= timer)
        {

            if (Random.Range(0, 100) <= AtkTriggerRate)
            {
                AtkTriggerRate = 20;
                StartCoroutine(AtkMotion());
            }
            else
            {
                AtkTriggerRate *= 1.1f;
            }
            timer = 0;
        }
        else
        {
            timer += Time.deltaTime;
        }
    }

    public IEnumerator AtkMotion()//攻撃アニメーションなど
    {
        int num = 0;
        if (atkSprites.Length < 2)
        {
            Debug.LogWarning("攻撃スプライトが足りません");
            yield break;
        }
        mode = Mode.atk;

        for (int i = 0; i < 10; i++)
        {
            int q = (i % 2);
            spriteRenderer.sprite = atkSprites[q];
            yield return Wait(0.2f, atkSpdRate);

        }

        spriteRenderer.sprite = atkSprites[2];
        yield return Wait(0.1f, atkSpdRate);


        InstantImpact();
        spriteRenderer.sprite = atkSprites[3];
        yield return Wait(0.2f, atkSpdRate);

        spriteRenderer.sprite = atkSprites[2];
        yield return Wait(0.1f, atkSpdRate);

        InstantImpact();
        spriteRenderer.sprite = atkSprites[3];
        yield return Wait(0.2f, atkSpdRate);

        spriteRenderer.sprite = atkSprites[2];
        yield return Wait(0.1f, atkSpdRate);

        InstantImpact();
        spriteRenderer.sprite = atkSprites[3];
        yield return Wait(0.5f, atkSpdRate);

        spriteRenderer.sprite = atkSprites[0];
        yield return Wait(0.3f, atkSpdRate);


        mode = Mode.move;
    }

    void InstantImpact()
    {

        GameObject I = Instantiate(impact, transform.position, Quaternion.Euler(transform.eulerAngles));
        I.layer = Mathf.RoundToInt(Mathf.Log(this.myLayer.value, 2));

        AngelImpact angelImpact = I.GetComponent<AngelImpact>();
        angelImpact.angel = this;
    }
}
