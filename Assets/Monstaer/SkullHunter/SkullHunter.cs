using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkullHunter : Monsters
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

        for (int i = 0; i < 15; i++)
        {
            int q = (i % 3);
            spriteRenderer.sprite = atkSprites[q];
            yield return Wait(0.2f, atkSpdRate);

        }
        spriteRenderer.sprite = atkSprites[3];
        yield return Wait(0.1f, atkSpdRate);

        spriteRenderer.sprite = atkSprites[4];
        yield return Wait(0.1f, atkSpdRate);


        spriteRenderer.sprite = atkSprites[5];
        yield return Wait(0.1f, atkSpdRate);

        spriteRenderer.sprite = atkSprites[7];
        yield return Wait(0.15f, atkSpdRate);

        Attack(target);
        ApplyStatusTarget(target, new StatusManager("BlooderSpdRateDown", true, StatusManager.StatusType.spdRate, 0.2f, -0.8f));

        spriteRenderer.sprite = atkSprites[6];
        yield return Wait(0.05f, atkSpdRate);

        spriteRenderer.sprite = atkSprites[7];
        yield return Wait(0.7f, atkSpdRate);
        mode = Mode.move;
    }

}
