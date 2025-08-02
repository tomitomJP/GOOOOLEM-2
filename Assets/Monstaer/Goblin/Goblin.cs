using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goblin : Monsters
{
    // Start is called before the first frame update
    public bool building = false;
    public float buildPosX;
    public GoblinBuild goblinBuild;
    public bool nowBuiding;
    void Start()
    {
        StartSetup();
        rayOrigin.y = 0.5f;

        GameObject[] goblinBuilds = GameObject.FindGameObjectsWithTag("GoblinBuild");

        for (int i = 0; i < goblinBuilds.Length; i++)
        {
            GoblinBuild _goblinBuild = goblinBuilds[i].GetComponent<GoblinBuild>();
            if (_goblinBuild.player == player)
            {
                goblinBuild = _goblinBuild;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (BuildingDist() < 0.3f && goblinBuild.hp < goblinBuild.hpMax && hp > 0)
        {
            if (!nowBuiding)
            {
                nowBuiding = true;
                StartCoroutine(BuildMotion(goblinBuild));
            }
        }
        else
        {
            Updating();
        }
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
        yield return Wait(0.1f);

        Attack(target);
        spriteRenderer.sprite = atkSprites[2];
        yield return Wait(0.05f);

        mode = Mode.move;

    }

    float BuildingDist()
    {
        float dist = Vector2.Distance(transform.position, goblinBuild.transform.position);
        return dist;
    }

    public IEnumerator BuildMotion(Monsters target)//攻撃アニメーションなど
    {
        int num = 0;
        if (atkSprites.Length < 2)
        {
            Debug.LogWarning("攻撃スプライトが足りません");
            yield break;
        }
        mode = Mode.atk;

        spriteRenderer.sprite = atkSprites[0];
        yield return Wait(0.4f);

        spriteRenderer.sprite = atkSprites[1];
        yield return Wait(0.2f);

        target.hp += 5f;
        spriteRenderer.sprite = atkSprites[2];
        yield return Wait(0.05f);

        yield return Wait(1.5f);

        mode = Mode.move;
        nowBuiding = false;
    }
}
