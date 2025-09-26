using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonDestoroyer : Human
{
   void Start()
    {
        StartSetup();
        HumanSetUp();
    }

    // Update is called once per frame
    void Update()
    {
        Updating();
    }

    public override IEnumerator AtkMotion(Monsters target)//攻撃アニメーションなど
    {
        int num = 0;
        if (atkSprites.Length < 2)
        {
            Debug.LogWarning("攻撃スプライトが足りません");
            yield break;
        }
        mode = Mode.atk;

        spriteRenderer.sprite = atkSprites[0];
        yield return Wait(0.4f, 2);

        AreaAtk();
        spriteRenderer.sprite = atkSprites[1];
        yield return Wait(0.1f, 2);

        spriteRenderer.sprite = atkSprites[2];
        yield return Wait(0.1f, 2);

        spriteRenderer.sprite = atkSprites[3];
        yield return Wait(0.1f, 2);




        mode = Mode.move;

    }

     void AreaAtk()
{
    if (Physics2D.Raycast(rayOrigin + transform.position, transform.right, Mathf.Infinity, enemyLayer))
    {
        RaycastHit2D[] hit;
        hit = Physics2D.RaycastAll(rayOrigin + transform.position, transform.right, Mathf.Infinity, enemyLayer);
        for (int j = 0; j < hit.Length; j++)
        {
            GameObject monsterObj = hit[j].collider.gameObject;
            Monsters monster = monsterObj.GetComponent<Monsters>();

            if (monster != null)
            {
                float damage = atk / 2f;

               if (monsterObj.TryGetComponent<Doragon>(out _) ||
                    monsterObj.TryGetComponent<Doragon_1>(out _))
                {
                    damage *= 1.5f;
                }

                Attack(monster, damage);
            }
        }
    }
}

}

