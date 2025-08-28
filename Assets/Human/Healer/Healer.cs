using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healer : Human
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

    public override IEnumerator AtkMotion(Monsters targetAlly)
    {
        if (atkSprites.Length < 1)
        {
            Debug.LogWarning("攻撃スプライトが足りません");
            yield break;
        }

        mode = Mode.atk;

        // 攻撃（回復）アニメ
        spriteRenderer.sprite = atkSprites[0];
        yield return Wait(0.5f, 0);



        // 回復処理
        if (targetAlly != null)
        {
            StatusManager healStatus = new StatusManager(
                "Heal",       // ID
                false,        // stackしない
                StatusManager.StatusType.heal, // タイプ
                0f,           // 効果時間は即時
                atk * atkRate * 0.5f           // 回復量
            );

            // ターゲットに回復を適用
            ApplyStatusTarget(targetAlly, healStatus);
        }



        mode = Mode.move;
    }

}
