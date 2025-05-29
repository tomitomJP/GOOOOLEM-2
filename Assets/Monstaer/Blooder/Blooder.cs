using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blooder : Monsters
{
    // Start is called before the first frame update
    [SerializeField] GameObject thunder;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip masicSE;
    [SerializeField] AudioClip kaminariSE;

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
                AtkTriggerRate = 10;
                StartCoroutine(AtkMotion());
            }
            else
            {
                AtkTriggerRate *= 1.25f;
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

        spriteRenderer.sprite = atkSprites[0];
        audioSource.PlayOneShot(masicSE);
        yield return Wait(0.1f);

        float t = 1;
        float t2 = 0;
        int i = 0;
        while (t >= t2)
        {
            i++;
            int q = (i % 2) + 1;
            spriteRenderer.sprite = atkSprites[q];
            t2 += Time.deltaTime;
            yield return null;
        }
        RaycastHit2D[] hit;
        if (Physics2D.Raycast(rayOrigin, transform.right, Mathf.Infinity, enemyLayer))
        {
            spriteRenderer.sprite = atkSprites[3];
            yield return Wait(0.2f);

            spriteRenderer.sprite = atkSprites[4];
            yield return Wait(0.1f);

            if (Physics2D.Raycast(rayOrigin, transform.right, Mathf.Infinity, enemyLayer))
            {
                hit = Physics2D.RaycastAll(rayOrigin, transform.right, Mathf.Infinity, enemyLayer);
                for (int j = 0; j < hit.Length; j++)
                {
                    GameObject monster = hit[j].collider.gameObject;
                    Instantiate(thunder, new Vector3(monster.transform.position.x, 9.5f, 0), Quaternion.identity);
                    audioSource.PlayOneShot(kaminariSE, 0.3f);
                    Attack(monster.GetComponent<Monsters>());
                    // スピード1.5倍、5秒間
                    ApplyStatusTarget(monster.GetComponent<Monsters>(), new StatusManager("BlooderSpdRateDown", true, StatusManager.StatusType.spdRate, 0.1f, -0.8f));
                    ApplyStatusTarget(monster.GetComponent<Monsters>(), new StatusManager("BlooderAtkSpdRateDown", true, StatusManager.StatusType.atkSpdRate, 0.1f, -0.8f));

                    ApplyStatusTarget(monster.GetComponent<Monsters>(), new StatusManager("BlooderSpdRateDown1", true, StatusManager.StatusType.spdRate, 3f, -0.3f));
                    ApplyStatusTarget(monster.GetComponent<Monsters>(), new StatusManager("BlooderAtkSpdRateDown1", true, StatusManager.StatusType.atkSpdRate, 3f, -0.4f));

                }
            }
            spriteRenderer.sprite = atkSprites[4];
            yield return Wait(0.5f);
        }



        mode = Mode.move;
    }
}
