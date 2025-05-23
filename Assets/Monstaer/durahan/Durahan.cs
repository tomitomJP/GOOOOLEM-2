using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Durahan : Monsters
{
    // Start is called before the first frame update
    void Start()
    {
        StartSetup();
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
        yield return Wait(0.25f, atkSpdRate);

        spriteRenderer.sprite = atkSprites[1];
        yield return Wait(0.1f, 1);

        Attack(target);
        spriteRenderer.sprite = atkSprites[2];
        yield return Wait(0.05f, 1);

        spriteRenderer.sprite = atkSprites[3];
        yield return Wait(0.05f, 1);

        spriteRenderer.sprite = atkSprites[4];
        yield return Wait(0.3f, 1);

        mode = Mode.move;

    }
}
