using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MetalSlime : Monsters
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
        yield return Wait(0.5f, 0);

        spriteRenderer.sprite = atkSprites[1];
        yield return Wait(0.1f, 2);

        Attack(target);




        mode = Mode.move;
    }

    public override void Damaged(float damage, Monsters attacker, bool Melee = true, StatusManager newStatus = null)
    {

        if (damage > 0)
        {
            AudioManager.PlaySE(defaultAtkSE, 0.3f);
        }
        else
        {
            return;
        }

        if (hp > 0)
        {

            hp -= 1;

            // ヒット演出
            InsHitPar(attacker);

            // ダメージテキスト表示
            Text _damageText = Instantiate(damageText, transform.position, Quaternion.identity, canvas.transform).GetComponent<Text>();
            _damageText.text = "1";
        }

        if (hp <= 0)
        {
            Dead();
        }
    }
}
