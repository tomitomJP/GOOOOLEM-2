using UnityEngine;

public class CannonBall : MonoBehaviour
{
    public float lifeTime = 10f;          // 弾の寿命
    public GameObject explosionPrefab;   // 爆発エフェクトのPrefab

    void Start()
    {
        // 5秒後に自動で消える（撃ちっぱなし対策）
        Destroy(gameObject, lifeTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 爆発エフェクトを生成
        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        }

        // 弾を削除
        Destroy(gameObject);
    }
}
