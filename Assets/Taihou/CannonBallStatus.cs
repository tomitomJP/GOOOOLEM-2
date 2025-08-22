using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBallStatus : Monsters
{
    public float lifeTime = 10f;          // 弾の寿命
    public GameObject explosionPrefab;   // 爆発エフェクトのPrefab
    [SerializeField] float attackRadius = 2f; // 攻撃半径
    [SerializeField] LayerMask[] targetLayer;

    void Start()
    {
        StartSetup();
        // 5秒後に自動で消える（撃ちっぱなし対策）
        Destroy(gameObject, lifeTime);
    }

    private bool hasExploded = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasExploded) return; // すでに処理したなら無視
        hasExploded = true;

        // 爆発エフェクト
        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        }

        // 範囲ダメージ
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, attackRadius, targetLayer[player]);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Monster") || hit.CompareTag("House"))
            {
                Monsters monsters = hit.GetComponent<Monsters>();
                if (monsters == null) continue;

                if (hit.CompareTag("House"))
                {
                    Attack(monsters, atk * 0.5f);
                }
                else
                {
                    Attack(monsters);
                    KnockBack(hit.gameObject);
                }
            }
        }

        // 弾を削除
        Destroy(gameObject);
    }

    [SerializeField] float knockBackPower = 10;
    void KnockBack(GameObject hit)
    {
        Rigidbody2D rb = hit.GetComponent<Rigidbody2D>();
        GameObject mon = hit;

        Vector2 monUp = mon.transform.up;
        Vector2 myLeft = -transform.right;
        Vector2 dire;
        if (player == 0)
        {
            dire = new Vector2(1, 1).normalized;
        }
        else
        {
            dire = new Vector2(-1, 1).normalized;

        }

        rb.AddForce(dire * knockBackPower, ForceMode2D.Impulse);
    }

}

