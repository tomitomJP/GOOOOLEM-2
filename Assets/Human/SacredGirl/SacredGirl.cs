using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SacredGirl : Human
{
    [SerializeField] GameObject impact;
    [SerializeField] AudioClip trumpetSE;
    [SerializeField] Sprite[] holySprite;
    [SerializeField] GameObject holyBeam;
    [SerializeField] GameObject chargePar;

    [SerializeField] float purificationRate = -25;
    void Start()
    {
        StartSetup();
        HumanSetUp();


    }

    public override void FirstSkill()
    {
        if (Random.Range(0, 10) == 0 || level >= 18) StartCoroutine(Purification());

    }
    // Update is called once per frame
    void Update()
    {
        Updating();
    }

    public override IEnumerator AtkMotion(Monsters target)//攻撃アニメーションなど
    {
        Debug.Log("ATK" + gameObject.name);

        int num = 0;
        if (atkSprites.Length < 2)
        {
            Debug.LogWarning("攻撃スプライトが足りません");
            yield break;
        }
        mode = Mode.atk;

        if (Random.Range(0, 100) <= purificationRate)
        {
            StartCoroutine(Purification());
            purificationRate = -25;
            yield break;

        }

        spriteRenderer.sprite = atkSprites[0];
        yield return Wait(0.5f);

        int shootCount = 0;

        do
        {
            shootCount++;
            purificationRate += shootCount * 3;

            spriteRenderer.sprite = atkSprites[0];
            yield return Wait(0.2f);

            InstantImpact();
            AudioManager.PlaySEWithPitch(trumpetSE, 1f, 0.3f + (0.15f * (shootCount - 1)));
            spriteRenderer.sprite = atkSprites[1];
            yield return Wait(0.1f, 2);

            spriteRenderer.sprite = atkSprites[2];
            yield return Wait(0.1f, 2);

            spriteRenderer.sprite = atkSprites[0];
            yield return Wait(0.2f);
            if (Random.Range(0, 5) != 0)
            {
                break;
            }
        } while (true);



        mode = Mode.move;
    }

    void InstantImpact()
    {

        GameObject I = Instantiate(impact, transform.position, Quaternion.Euler(transform.eulerAngles));
        //I.layer = Mathf.RoundToInt(Mathf.Log(this.myLayer.value, 2));

        AngelImpact angelImpact = I.GetComponent<AngelImpact>();
        angelImpact.angel = this;
    }

    public IEnumerator Purification()
    {
        mode = Mode.atk;

        float animDuration = 0.3f;
        float duration = 3;
        int n = 0;

        Instantiate(chargePar, transform.position, Quaternion.identity, transform);
        for (int i = 0; i < 40; i++)
        {
            if (hp <= 0)
            {
                yield break;
            }
            int q = i % 3;
            spriteRenderer.sprite = holySprite[q];
            yield return Wait(0.2f);

        }

        spriteRenderer.sprite = holySprite[4];
        yield return Wait(0.1f, 2);

        spriteRenderer.sprite = holySprite[5];
        yield return Wait(0.1f, 2);

        if (hp <= 0)
        {
            yield break;
        }
        List<Monsters> targets = GameManager.GetMonsters(GameManager.type.mon);

        foreach (var target in targets)
        {

            if (target == null || !target.gameObject.activeSelf || target.hp <= 0) continue;

            if (!target.gameObject.TryGetComponent<House>(out House house))
            {
                Instantiate(holyBeam, target.transform.position, Quaternion.identity);
                target.Damaged(0, this, false, new StatusManager("holy", false, StatusManager.StatusType.spdRate, 5, -1));
                target.Damaged(0, this, false, new StatusManager("holy1", false, StatusManager.StatusType.atkSpdRate, 5, -1));
            }

        }

        for (int i = 0; i < 17; i++)
        {

            int q = i % 3;
            spriteRenderer.sprite = holySprite[q];
            yield return Wait(0.2f);

        }

        foreach (var target in targets)
        {
            if (target == null || !target.gameObject.activeSelf || target.hp <= 0) continue;

            if (!target.gameObject.TryGetComponent<House>(out House house))
            {
                target.Damaged(10000, this, false);

            }
        }

        mode = Mode.move;

        yield break;
    }

    void ShieldingMove()
    {
        Vector2 direction;
        if (player == 0)
        {
            direction = Vector2.right;
        }
        else
        {
            direction = -Vector2.right;

        }

        if (!Physics2D.Raycast(transform.position + rayOrigin, direction, 0.8f, enemyLayer))
        {
            transform.Translate(direction * Time.deltaTime * 0.7f, Space.World);
        }

    }



}
