using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Doragon : Monsters
{
    [SerializeField] Transform firePoint;
    [SerializeField] GameObject fireParticle;



    void Start()
    {
        //StartCoroutine(AtkMotion());
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

        spriteRenderer.sprite = atkSprites[num];
        yield return Wait(1.3f, atkSpdRate);

        num++;
        spriteRenderer.sprite = atkSprites[num];



        Transform fire = Instantiate(fireParticle, firePoint.transform.position, Quaternion.identity, transform).transform;
        fire.transform.localEulerAngles = new Vector3(45, 90, 0);

        GetTargets();
        yield return Wait(0.2f, 1);
        GetTargets();
        yield return Wait(0.2f, 1);
        GetTargets();
        yield return Wait(0.2f, 1);
        GetTargets();
        yield return Wait(0.2f, 1);
        GetTargets();
        yield return Wait(0.2f, 1);
        mode = Mode.move;
    }

    void GetTargets()
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position + rayOrigin, transform.right, enemyDistance * 2f, enemyLayer);
        Debug.DrawRay(transform.position + rayOrigin, transform.right * (enemyDistance * 2f), Color.yellow, 0.3f);

        foreach (RaycastHit2D hit in hits)
        {

            // ここで hit.collider などを使って処理
            Attack(hit.collider.gameObject.GetComponent<Monsters>(), atk / 5);
        }
    }
}
