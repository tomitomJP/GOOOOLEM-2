using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Peace : MonoBehaviour
{

    public bool check = false;
    public bool selecting = false;
    public int peaceNumber = 0;
    [SerializeField] SpriteRenderer checkingSprite;
    [SerializeField] SpriteRenderer selectingSprite;
    [SerializeField] float fallingTime = 0.1f;
    [SerializeField] LayerMask targetMask;
    SpriteRenderer peaceSprite;
    void Start()
    {
        peaceSprite = GetComponent<SpriteRenderer>();
        StartCoroutine(Fall());
    }

    // Update is called once per frame
    void Update()
    {
        if (check)
        {
            checkingSprite.enabled = true;
            peaceSprite.sortingOrder = 4;
        }
        else if (selecting)
        {
            selectingSprite.enabled = true;
            peaceSprite.sortingOrder = 4;
        }
        else
        {
            if (check == false)
            {
                checkingSprite.enabled = false;

            }
            if (selecting == false)
            {
                selectingSprite.enabled = false;
            }
            peaceSprite.sortingOrder = 0;
        }
    }

    IEnumerator Fall()
    {
        while (true)
        {
            Debug.DrawRay(transform.position, Vector3.down * 0.5f, Color.green, 0.2f);

            RaycastHit2D[] hit = Physics2D.RaycastAll(transform.position, Vector3.down, 0.5f, targetMask);

            if (hit != null)
            {
                bool fragg = true;
                for (int i = 0; i < hit.Length; i++)
                {
                    if (hit[i].collider.gameObject == gameObject)
                    {
                        continue;
                    }
                    else
                    {
                        fragg = false;
                        break;
                    }
                }
                if (fragg)
                {
                    //transform.position -= new Vector3(0, 0.5f, 0);

                    Vector2 A = transform.position;
                    Vector2 B = transform.position - new Vector3(0, 0.5f, 0);

                    float time = fallingTime;
                    float timer = 0;
                    int maxcount = 0;
                    while (time >= timer && maxcount < 50)
                    {
                        transform.position = Vector2.Lerp(A, B, timer / time);
                        timer += Time.deltaTime;
                        yield return null;
                    }

                    transform.position = B;

                }
            }
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

            Debug.DrawLine(pointA, pointB, color, 0.2f, false);
        }
    }
}
