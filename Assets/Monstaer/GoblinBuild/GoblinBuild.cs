using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoblinBuild : Monsters
{

    [SerializeField] Slider hpBar;
    public float hpMax = 150;
    [SerializeField] BoxCollider2D boxCollider2D;
    [SerializeField] Sprite[] houseSprites;
    Vector3 startPos;
    bool buildOK = false;
    void Start()
    {
        startPos = transform.position;
        StartSetup();
    }

    // Update is called once per frame
    void Update()
    {
        hp = Mathf.Clamp(hp, -30, hpMax);
        //hpBar.value = hp / hpMax;


        boxCollider2D.enabled = hp > 0;
        spriteRenderer.enabled = hp > 0;

        if (buildOK && hp <= 0)
        {
            buildOK = false;
            hp = -hpMax;
        }
        if (hp > 0 && !buildOK)
        {
            hp = hpMax * 0.8f;

            buildOK = true;
        }


        SpriteChanger();


    }

    public override void Damaged(float damage)
    {
        AudioManager.PlaySE(defaultAtkSE, 0.4f);
        StartCoroutine(DamageAnimHouse());
        hp -= damage;

        Text _damageText = Instantiate(damageText, transform.position, Quaternion.identity, canvas.transform).GetComponent<Text>();
        _damageText.text = damage.ToString("F0");
    }

    IEnumerator DamageAnimHouse()
    {
        Vector3 A = startPos;
        Vector3 B = transform.position + new Vector3(0.5f, 0);
        Vector3 C = transform.position + new Vector3(-0.2f, 0);

        float timer = 0;
        float time = 0.1f;
        while (time > timer)
        {
            transform.position = Vector3.Lerp(A, B, timer / time);
            timer += Time.deltaTime;
            yield return null;
        }

        timer = 0;
        time = 0.05f;
        while (time > timer)
        {
            transform.position = Vector3.Lerp(B, C, timer / time);
            timer += Time.deltaTime;
            yield return null;
        }

        timer = 0;
        time = 0.2f;
        while (time > timer)
        {
            transform.position = Vector3.Lerp(C, A, timer / time);
            timer += Time.deltaTime;
            yield return null;
        }

        transform.position = A;
    }

    void SpriteChanger()
    {
        float hpRate = (float)hp / hpMax;

        if (hpRate >= 1.0f)
        {
            // HP 100% 以上
        }
        else if (hpRate >= 0.75f)
        {
            // HP 75%以上
        }
        else if (hpRate >= 0.5f)
        {
            // HP 50%以上
        }
        else if (hpRate >= 0.25f)
        {
            // HP 25%以上
        }
        else if (hpRate >= 0f)
        {
            // HP 0%以上
        }
        else if (hpRate >= -5)
        {
            // HP -25%以上
        }
        else if (hpRate >= -10f)
        {
            // HP -50%以上
        }
        else if (hpRate >= -20f)
        {
            // HP -50%以上
        }
        else if (hpRate >= -30f)
        {
            // HP -50%以上
        }
    }

}
