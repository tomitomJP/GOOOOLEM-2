using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PazzleManager : MonoBehaviour
{
    [SerializeField] int fieldWidth = 10;
    [SerializeField] int fieldHeight = 10;
    public GameObject[] peaces;
    [SerializeField] float spacing = 1.1f; // ピースの間隔
    public GameObject peacePearent;

    Dictionary<Vector2Int, GameObject> grid = new Dictionary<Vector2Int, GameObject>();
    List<GameObject> destroyPeace = new List<GameObject>();
    public GameObject blackScreen;

    public Transform pointer;
    public GameObject[] Houses;
    public Transform BattleField;
    public Transform MonstersPearent;
    public ControllManager controllManager { get; set; }
    public House house;

    public Transform MonstarsRule;
    public Vector3[] MonstarsRulePos = new Vector3[2];
    public int soulSpawnRate = 10;
    void Start()
    {
        //StartCoroutine(PeaceSet());
    }

    [SerializeField] bool movepeace = false;
    void Update()
    {
        // PeaceSet();

        if (movepeace)
        {
            movepeace = false;
        }
    }

    public IEnumerator PeaceSet()
    {
        yield return new WaitForSeconds(0.3f);

        blackScreen.SetActive(true);
        int check = 1;
        int MaxCount = 0;
        while (peacePearent.transform.childCount != 121 && MaxCount < 100)
        {
            while (check > 0)
            {
                check = 0;
                for (int i = 0; i < 11; i++)
                {
                    Vector2 localPos = new Vector2((i * 0.5f) - 2.5f, 2.5f);
                    Vector2 worldPos = (Vector2)transform.TransformPoint(localPos);
                    //DrawCircle2D(worldPos, 0.25f);
                    if (Physics2D.OverlapPoint(worldPos, LayerMask.GetMask("Peace")) == null)
                    {
                        if (Random.Range(0, 108) <= soulSpawnRate)
                        {
                            Instantiate(peaces[4], worldPos, Quaternion.identity, peacePearent.transform);

                        }
                        else
                        {
                            Instantiate(peaces[Random.Range(0, peaces.Length - 1)], worldPos, Quaternion.identity, peacePearent.transform);

                        }
                        check++;
                    }
                }

                if (check > 0)
                {
                    yield return new WaitForSeconds(0.1f);
                }
                yield return null;

            }
            MaxCount++;

        }
        ResetHilightPeace();
        blackScreen.SetActive(false);


    }

    [SerializeField] Text brickCountText;
    public void BrickCount(int num)
    {
        StartCoroutine(BrickCountAnim(num));
    }
    public void BrickCountOff()
    {
        brickCountText.enabled = false;
    }

    IEnumerator BrickCountAnim(int num)
    {
        brickCountText.enabled = true;
        float time;
        float timer;

        brickCountText.text = num.ToString();
        timer = 0;
        time = 0.15f;
        while (time >= timer)
        {
            float t = timer / time;
            brickCountText.fontSize = (int)Mathf.Lerp(40, 50, t);
            timer += Time.deltaTime;
            yield return null;
        }

        timer = 0;
        time = 0.1f;
        while (time >= timer)
        {
            float t = timer / time;
            brickCountText.fontSize = (int)Mathf.Lerp(50, 40, t);
            timer += Time.deltaTime;
            yield return null;
        }


    }

    void DrawCircle2D(Vector3 center, float radius, int segments = 30, Color color = default)
    {
        if (color == default) color = Color.green;

        float angleStep = 360f / segments;
        for (int i = 0; i < segments; i++)
        {
            float angleA = Mathf.Deg2Rad * angleStep * i;
            float angleB = Mathf.Deg2Rad * angleStep * (i + 1);

            Vector3 pointA = center + new Vector3(Mathf.Cos(angleA), Mathf.Sin(angleA), 0) * radius;
            Vector3 pointB = center + new Vector3(Mathf.Cos(angleB), Mathf.Sin(angleB), 0) * radius;

            Debug.DrawLine(pointA, pointB, color, 0f, false);
        }
    }

    public void ResetHilightPeace()
    {
        for (int i = 0; i < peacePearent.transform.childCount; i++)
        {
            GameObject peaceG = peacePearent.transform.GetChild(i).gameObject;
            Peace peaceS = peaceG.GetComponent<Peace>();

            SpriteRenderer spr = peaceG.GetComponent<SpriteRenderer>();
            spr.color = new Color(spr.color.r, spr.color.g, spr.color.b, 1);
            if (peaceS.peaceNumber == 4)
            {
                peaceS.StartParticle(4);

            }
        }
    }


    public Vector2 HilightPeace(List<GameObject> peces, int number)
    {
        ResetHilightPeace();

        Vector2 canDire = Vector2.zero;

        List<GameObject> hilightPeaces = new List<GameObject>();
        hilightPeaces.AddRange(peces);
        int firstCount = hilightPeaces.Count - 1;
        for (int n = firstCount; n < hilightPeaces.Count; n++)
        {
            for (int i = 0; i < 4; i++)
            {
                Vector2 direction = Vector2.right;
                switch (i)
                {
                    case 0:
                        direction = Vector2.right;
                        break;
                    case 1:
                        direction = Vector2.up;
                        break;
                    case 2:
                        direction = Vector2.left;
                        break;
                    case 3:
                        direction = Vector2.down;
                        break;
                }
                Debug.DrawRay(hilightPeaces[n].transform.position, direction * 0.6f, Color.red, 0.5f); // 可視化（長さに注意）
                RaycastHit2D[] hit = Physics2D.RaycastAll(hilightPeaces[n].transform.position, direction, 0.6f, LayerMask.GetMask("Peace"));
                if (hit != null && hit.Length > 0)
                {
                    Debug.Log(hit.Length);
                    for (int j = 0; j < hit.Length; j++)
                    {
                        Peace peace = hit[j].collider.gameObject.GetComponent<Peace>();
                        Peace Apeace = hilightPeaces[n].GetComponent<Peace>();

                        if (peace != null)
                        {
                            if ((number == peace.peaceNumber || 4 == peace.peaceNumber || number == 4) && peace.check == false)
                            {
                                if (!hilightPeaces.Contains(peace.gameObject))
                                {
                                    hilightPeaces.Add(peace.gameObject);
                                    if (n == firstCount)
                                    {

                                        switch (i)
                                        {
                                            case 0: // 右
                                                if (canDire.x == -1 || canDire.x == 2) canDire.x = 2;
                                                else canDire.x = 1;
                                                break;
                                            case 2: // 左
                                                if (canDire.x == 1 || canDire.x == 2) canDire.x = 2;
                                                else canDire.x = -1;
                                                break;
                                            case 1: // 上
                                                if (canDire.y == -1 || canDire.y == 2) canDire.y = 2;
                                                else canDire.y = 1;
                                                break;
                                            case 3: // 下
                                                if (canDire.y == 1 || canDire.y == 2) canDire.y = 2;
                                                else canDire.y = -1;
                                                break;
                                        }
                                    }
                                }
                            }
                            else if (hilightPeaces.Count >= 2 && firstCount >= 1)
                            {
                                if (n == firstCount && peace.gameObject == hilightPeaces[firstCount - 1])
                                {

                                    switch (i)
                                    {
                                        case 0: // 右
                                            if (canDire.x == -1 || canDire.x == 2) canDire.x = 2;
                                            else canDire.x = 1;
                                            break;
                                        case 2: // 左
                                            if (canDire.x == 1 || canDire.x == 2) canDire.x = 2;
                                            else canDire.x = -1;
                                            break;
                                        case 1: // 上
                                            if (canDire.y == -1 || canDire.y == 2) canDire.y = 2;
                                            else canDire.y = 1;
                                            break;
                                        case 3: // 下
                                            if (canDire.y == 1 || canDire.y == 2) canDire.y = 2;
                                            else canDire.y = -1;
                                            break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }



        for (int i = 0; i < peacePearent.transform.childCount; i++)
        {
            GameObject peaceG = peacePearent.transform.GetChild(i).gameObject;
            Peace peaceS = peaceG.GetComponent<Peace>();
            if (!hilightPeaces.Contains(peaceG) && peaceS.check == false)
            {
                SpriteRenderer spr = peaceG.gameObject.GetComponent<SpriteRenderer>();
                spr.color = new Color(spr.color.r, spr.color.g, spr.color.b, 0.1f);
                peaceS.StartParticle(-1);

            }
            else
            {
                SpriteRenderer spr = peaceG.gameObject.GetComponent<SpriteRenderer>();
                spr.color = new Color(spr.color.r, spr.color.g, spr.color.b, 1f);
                if (peaceS.peaceNumber == 4)
                {
                    peaceS.StartParticle(number);
                }

            }


        }
        return canDire;
    }

}
