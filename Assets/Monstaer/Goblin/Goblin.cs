using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goblin : Monsters
{
    // Start is called before the first frame update
    void Start()
    {
        StartSetup();
        rayOrigin.y = 0.5f;
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
        yield return Wait(0.25f);

        spriteRenderer.sprite = atkSprites[1];
        yield return Wait(0.1f, 2);

        Attack(target);
        spriteRenderer.sprite = atkSprites[2];
        yield return Wait(0.05f, 2);

        mode = Mode.move;

    }
}
