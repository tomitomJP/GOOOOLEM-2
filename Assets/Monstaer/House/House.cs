using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class House : Monsters
{
    [SerializeField] Slider hpBar;
    [SerializeField] Slider hpBar_Red;

    [SerializeField] Text hpBarText;
    [SerializeField] float hpMax;
    [SerializeField] Transform monstersPearent;
    [SerializeField] BoxCollider2D boxCollider2D;

    [SerializeField] Sprite[] houseSprites;

    [SerializeField] SpriteRenderer DoorSpr;
    [SerializeField] Sprite[] DoorSprite;

    Vector3 startPos;
    void Start()
    {
        startPos = transform.position;
        StartCoroutine(DamageSliderMove());
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

        if (hp <= 0)
        {
            boxCollider2D.enabled = false;
        }

        if (hp / 150 > 0.75f)
        {
            spriteRenderer.sprite = houseSprites[0];
        }
        else if (hp / 150 > 0.5f)
        {
            spriteRenderer.sprite = houseSprites[1];

        }
        else if (hp / 150 > 0.25f)
        {
            spriteRenderer.sprite = houseSprites[2];

        }
        else if (hp / 150 > 0)
        {
            spriteRenderer.sprite = houseSprites[3];

        }
        else
        {
            spriteRenderer.sprite = houseSprites[4];
        }
    }

    public void DoorAnimTrigger()
    {
        StartCoroutine(DoorAnim());
    }

    IEnumerator DoorAnim()
    {

        for (int i = 0; i < DoorSprite.Length; i++)
        {
            DoorSpr.sprite = DoorSprite[i];
            yield return Wait(0.1f, 2);
        }
        yield return Wait(0.3f, 2);

        for (int i = 0; i < DoorSprite.Length; i++)
        {
            DoorSpr.sprite = DoorSprite[(DoorSprite.Length - 1) - i];
            yield return Wait(0.1f, 2);
        }

    }

    public override void Damaged(float damage)
    {
        StartCoroutine(DamageAnimHouse());
        damages.Add(new Damage(hp, damage));
        hp -= damage;

        Text _damageText = Instantiate(damageText, transform.position, Quaternion.identity, canvas.transform).GetComponent<Text>();
        _damageText.text = damage.ToString("F0");
    }


    class Damage
    {
        public float beforeHP;
        public float afterHP;

        public Damage(float nowHP, float damage)
        {
            beforeHP = nowHP;
            afterHP = nowHP - damage;
        }

    }
    List<Damage> damages = new List<Damage>();



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

    IEnumerator DamageSliderMove()
    {
        while (true)
        {
            if (damages.Count == 0)
            {
                yield return null;
                continue;
            }

            float A = damages[0].beforeHP / hpMax;
            float B = damages[0].afterHP / hpMax;
            float timer = 0;

            yield return Wait(0.5f, 2);
            while (timer < 0.5f)
            {
                hpBar_Red.value = math.lerp(A, B, timer / 0.5f);
                timer += Time.deltaTime;
                yield return null;
            }

            hpBar_Red.value = B; // 最終的にぴったり合わせる
            damages.RemoveAt(0); // 次のダメージへ
        }
    }

}
