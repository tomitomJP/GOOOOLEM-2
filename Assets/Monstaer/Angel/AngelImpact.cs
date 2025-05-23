using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngelImpact : MonoBehaviour
{
    public Monsters angel;
    [SerializeField] float spd = 6;
    [SerializeField] float lifetime = 6;
    void Start()
    {

    }

    [SerializeField] float scaleTime = 0.5f;
    float scaleTimer = 0f;

    void Update()
    {
        transform.Translate(Time.deltaTime * transform.right * spd);

        lifetime -= Time.deltaTime;
        if (lifetime < 0)
        {
            Destroy(gameObject);
        }

        scaleTimer += Time.deltaTime;
        transform.localScale = Vector3.Lerp(Vector3.one * 0.1f, Vector3.one, Mathf.Clamp(scaleTimer / scaleTime, 0, 1));

    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Monster") || collision.gameObject.CompareTag("House"))
        {
            Monsters monsters = collision.gameObject.GetComponent<Monsters>();

            monsters.ApplyStatus(new Monsters.StatusManager("BlooderSpdRateDown", true, Monsters.StatusManager.StatusType.spdRate, 0.5f, -0.8f));
            monsters.ApplyStatus(new Monsters.StatusManager("BlooderAtkSpdRateDown", true, Monsters.StatusManager.StatusType.atkSpdRate, 0.5f, -0.8f));

            angel.Attack(monsters, angel.atk / 3);
        }
    }


}
