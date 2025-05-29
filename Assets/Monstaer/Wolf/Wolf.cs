using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wolf : Monsters
{

    [SerializeField] GameObject bites;
    void Start()
    {
        StartSetup();
        rayOrigin.y = 0.0f;
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
        yield return Wait(0.15f, 0);

        Vector3 offset = new Vector2(3f, 0);

        if (player == 1) offset.x *= -1;

        Vector3 pos = transform.position + offset;
        if (player == 0)
        {
            Instantiate(bites, pos, Quaternion.Euler(new Vector3(0, 0, -20)));

        }
        else
        {
            Instantiate(bites, pos, Quaternion.Euler(new Vector3(0, 0, 20)));

        }

        spriteRenderer.sprite = atkSprites[1];
        yield return Wait(0.15f, 0);

        Attack(target);
        spriteRenderer.sprite = atkSprites[2];
        yield return Wait(0.15f, 0);



        mode = Mode.move;

    }
}
