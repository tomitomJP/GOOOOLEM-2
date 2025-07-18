using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using Unity.Burst.Intrinsics;
using UnityEngine;
using UnityEngine.UI;

public class Monsters : MonoBehaviour
{

    public Vector3 rayOrigin;
    public SpriteRenderer spriteRenderer;
    public Sprite[] moveSprites;//歩く用のスプライトを入れる
    public Sprite[] atkSprites;//攻撃用のスプライトを入れる

    public float atk = 50;//攻撃力
    public float hp = 100;
    public float spd = 1;//移動速度
    public float spdRate = 1;//バフやデバフを受けた時ここの値が変更される。ステータスマネージャーを介して変更するため、直接変更してはならない(移動速度)
    public float atkSpdRate = 1;//バフやデバフを受けた時ここの値が変更される。ステータスマネージャーを介して変更するため、直接変更してはならない(攻撃速度)
    public float atkRate = 1;//バフやデバフを受けた時ここの値が変更される。ステータスマネージャーを介して変更するため、直接変更してはならない(攻撃力)

    public int player = 0;//左は0,右は1
    public float enemyDistance = 1;//敵を見つける距離(攻撃の当たる距離。攻撃の当たる距離のみを変更したい場合はAttack関数とEnemyCheck関数をオーバーライドして書き換える)
    public float allyDistance = 0.3f;//使用していない
    public float atkCT = 1;//攻撃のクールタイム
    public float aktCTimer = 0;

    public LayerMask myLayer;
    public LayerMask enemyLayer;

    public float stanTIme = 0;

    public enum Mode
    {
        idle,
        move,
        atk,
        atkCT,
        stan,
    }

    public Mode mode = Mode.move;


    public float MoveAniTime = 0.3f;
    public float MoveAniTimer = 0f;
    int MoveAniSpriteNum = 0;

    public GameObject damageText { get; set; }
    public Canvas canvas { get; set; }
    public GameManager gameManager { get; set; }
    public AudioClip defaultAtkSE;

    void Update()
    {

    }

    public virtual void Move()//移動と異動アニメーションを管理
    {
        if (mode == Mode.move && stanTIme <= 0)
        {
            transform.Translate(Vector3.right * Time.deltaTime * spd * Mathf.Clamp(spdRate, 0, 100));
        }

        if (stanTIme > 0)
        {
            stanTIme -= Time.deltaTime;
        }

        if (MoveAniTimer >= MoveAniTime)
        {
            MoveAniSpriteNum = (MoveAniSpriteNum + 1) % moveSprites.Length;
            spriteRenderer.sprite = moveSprites[MoveAniSpriteNum];
            MoveAniTimer = 0;
            spriteRenderer.color = Color.white;
            ;
        }
        else
        {
            MoveAniTimer += Time.deltaTime * Mathf.Clamp(spdRate, 0, 2);
        }

    }

    public virtual void StartSetup()//継承先のStart関数に入れる
    {
        canvas = GameObject.FindWithTag("DamageTextCanvas").GetComponent<Canvas>();
        gameManager = GameObject.FindWithTag("GameController").GetComponent<GameManager>();

        spriteRenderer = GetComponent<SpriteRenderer>();
        // float X = (Random.Range(0, 11) - 5) * 0.1f;
        float Y = Random.Range(0, 4) * 0.1f;
        rayOrigin.y = 0.0f;
        //enemyDistance *= Random.Range(0.8f, 1.1f);
        if (player == 1)
        {
            transform.eulerAngles = new Vector3(0, 180, 0);

            myLayer = LayerMask.GetMask("Monster1");
            gameObject.layer = LayerMask.NameToLayer("Monster1");
            enemyLayer = LayerMask.GetMask("Monster0");
        }
        else
        {
            myLayer = LayerMask.GetMask("Monster0");
            gameObject.layer = LayerMask.NameToLayer("Monster0");
            enemyLayer = LayerMask.GetMask("Monster1");

        }

        damageText = Resources.Load<GameObject>("DamageText"); // Resources/Enemy.prefab をロード
    }

    public virtual void Updating()//継承先のUpdate関数に入れる
    {
        //Allycheck();
        EnemyCheck();
        if (mode != Mode.atk)
        {
            Move();
        }
        else
        {
            MoveAniTimer = MoveAniTime;
        }

        if (0 >= hp)
        {
            Dead();
        }
        UpdateStatuses();
    }

    public void Dead()
    {
        gameManager.resultDatas[(player + 1) % 2].killCount++;

        spriteRenderer.enabled = false;
        gameObject.GetComponent<BoxCollider2D>().enabled = false;
        Destroy(gameObject);
    }

    public float Attack(Monsters target, float damage = -810)//引数に攻撃対象のMonstrsコンポーネントを入れる
    {
        if (damage == -810) damage = atk;
        if (target == null || !target.gameObject.activeSelf) { return 0; }
        if (hp > 0)
        {
            target.Damaged(damage * atkRate);
            if (target.hp >= 0)
            {
                target.DamagedAnimTrigger();
            }
            return damage * atkRate;
        }
        return 0;

    }

    public virtual void Damaged(float damage)
    {
        AudioManager.PlaySE(defaultAtkSE, 0.3f);

        hp -= damage;
        Text _damageText = Instantiate(damageText, transform.position, Quaternion.identity, canvas.transform).GetComponent<Text>();
        _damageText.text = damage.ToString("F0");
    }

    public void Healed(float healValue)
    {
        hp -= healValue;
        Text _damageText = Instantiate(damageText, transform.position, Quaternion.identity, canvas.transform).GetComponent<Text>();
        _damageText.color = Color.green;
        _damageText.text = "+" + healValue.ToString("F0");
    }


    public void DamagedAnimTrigger()
    {
        StartCoroutine(DamagedAnim());
    }


    public IEnumerator DamagedAnim()
    {
        bool flag = true;

        for (int i = 0; i < 10; i++)
        {
            flag = !flag;
            spriteRenderer.color = flag ? new Color(1, 1, 1, 0.5f) : Color.white;
            yield return Wait(0.05f, 2);
        }

        spriteRenderer.color = Color.white;

    }

    public virtual void EnemyCheck()
    {
        //Vector2 origin = (Vector2)transform.position + rayOrigin;

        if (player == 1)
        {
            //origin = (Vector2)transform.position + new Vector2(rayOrigin.x * -1, rayOrigin.y);
        }

        Vector2 direction = transform.right;

        Debug.DrawRay(transform.position + rayOrigin, direction * enemyDistance, Color.red, 0.1f); // 可視化（長さに注意）

        RaycastHit2D hit = Physics2D.Raycast(transform.position + rayOrigin, direction, enemyDistance, enemyLayer);
        if (hit.collider != null)
        {
            Monsters targetData = hit.collider.gameObject.GetComponent<Monsters>();
            if (targetData != null && gameObject != hit.collider.gameObject)
            {
                if (targetData.player != player)
                {
                    if (mode != Mode.atk && aktCTimer <= 0)
                    {
                        StartCoroutine(AtkMotion(targetData));
                        aktCTimer = atkCT;
                    }
                }
            }
        }

        if (aktCTimer > 0 && mode != Mode.atk)
        {
            aktCTimer = Mathf.Clamp(aktCTimer - (Time.deltaTime * atkSpdRate), 0, Mathf.Infinity);
            if (aktCTimer <= 0)
            {
                mode = Mode.move;
            }
            else
            {
                mode = Mode.atkCT;
            }
        }
    }

    public void Allycheck()
    {
        Vector2 origin = transform.position + rayOrigin;

        if (player == 1)
        {
            origin = (Vector2)transform.position + new Vector2(rayOrigin.x * -1, rayOrigin.y);
        }

        Vector2 direction = transform.right;

        Debug.DrawRay(origin, direction * allyDistance, Color.red, 0.5f); // 可視化（長さに注意）

        RaycastHit2D hit = Physics2D.Raycast(origin, direction, allyDistance, LayerMask.GetMask("Monster"));
        if (hit.collider != null)
        {
            Monsters targetData = hit.collider.gameObject.GetComponent<Monsters>();
            if (targetData != null && gameObject != hit.collider.gameObject)
            {
                if (targetData.player == player)
                {
                    if (mode != Mode.atk)
                    {
                        mode = Mode.idle;
                    }
                }
            }
        }
    }

    public virtual IEnumerator AtkMotion(Monsters target)//攻撃アニメーションなど
    {
        int num = 0;
        if (atkSprites.Length < 2)
        {
            Debug.LogWarning("攻撃スプライトが足りません");
            yield break;
        }
        mode = Mode.atk;
        mode = Mode.move;
    }
    /* protected WaitForSeconds Wait(float duration, float rate = 1)
     {
         return new WaitForSeconds(duration / Mathf.Clamp(rate, 0.001f, 100));
     }*/
    public IEnumerator Wait(float duration, int type = 0)
    {
        float time = duration;
        float timer = 0;
        while (time >= timer)
        {
            float rate = 1;
            switch (type)
            {
                case 0:
                    rate = atkSpdRate;
                    break;
                case 1:
                    rate = spdRate;
                    break;
                case >= 2:
                    rate = 1;
                    break;
            }
            timer += Time.deltaTime * Mathf.Clamp(rate, 0.001f, 5);
            yield return null;
        }
    }

    [System.Serializable]
    public class StatusManager
    {
        public string id;
        public bool stack = true;

        public enum StatusType
        {
            spdRate,
            atkRate,
            atkSpdRate,
            heal,
            stan,
        }

        public StatusType type;
        public float time = 0f;
        public float value = 1f;

        public StatusManager(string id, bool stack, StatusType type, float time, float value)
        {
            this.id = id;
            this.stack = stack;
            this.type = type;
            this.time = time;
            this.value = value;
        }

        public bool UpdateTimer(float deltaTime)
        {
            time -= deltaTime;
            return time <= 0;
        }
    }

    public List<StatusManager> sm = new List<StatusManager>();

    public void ApplyStatusTarget(Monsters monsters, StatusManager newStatus)
    {
        monsters.ApplyStatus(newStatus);
    }


    public void ApplyStatus(StatusManager newStatus)
    {
        if (!newStatus.stack)
        {
            var existing = sm.Find(s => s.id == newStatus.id);
            if (existing != null)
            {
                existing.time = newStatus.time;
                return;
            }
        }

        sm.Add(newStatus);
        ApplyStatusEffect(newStatus);
    }

    public void UpdateStatuses()
    {
        for (int i = sm.Count - 1; i >= 0; i--)
        {
            if (sm[i].UpdateTimer(Time.deltaTime))
            {
                RemoveStatusEffect(sm[i]);
                sm.RemoveAt(i);
            }
        }
    }

    public void ApplyStatusEffect(StatusManager status)
    {
        switch (status.type)
        {
            case StatusManager.StatusType.spdRate:
                spdRate += status.value;
                break;
            case StatusManager.StatusType.atkRate:
                atkRate += status.value;
                break;
            case StatusManager.StatusType.atkSpdRate:
                atkSpdRate += status.value;
                break;
            case StatusManager.StatusType.heal:
                hp += status.value;
                Healed(status.value);
                break;
            case StatusManager.StatusType.stan:
                mode = Mode.stan;
                break;
        }
    }

    public void RemoveStatusEffect(StatusManager status)
    {
        switch (status.type)
        {
            case StatusManager.StatusType.spdRate:
                spdRate -= status.value;
                break;
            case StatusManager.StatusType.atkRate:
                atkRate -= status.value;
                break;
            case StatusManager.StatusType.atkSpdRate:
                atkSpdRate -= status.value;
                break;
            case StatusManager.StatusType.stan:
                mode = Mode.move;
                break;
        }
    }
}
