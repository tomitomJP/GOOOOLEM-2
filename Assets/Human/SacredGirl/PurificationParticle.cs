using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PurificationParticle : MonoBehaviour
{
    [SerializeField] GameObject par0;
    [SerializeField] GameObject par1;
    [SerializeField] Light2D holyLight;
    [SerializeField] AudioClip se;
    [SerializeField] AudioClip se2;

    IEnumerator Start()
    {
        Vector3 holyScale = holyLight.gameObject.transform.localScale;
        holyLight.gameObject.transform.localScale = Vector3.zero;

        holyLight.gameObject.transform.DOScale(holyScale, 0.3f);
        yield return new WaitForSeconds(0.14f);
        StartCoroutine(PlaySE());
        StartCoroutine(PlaySE2());

        par0.SetActive(true);
        par1.SetActive(true);

        float duration = 3f;
        float elapsed = 0f;


        while (elapsed < duration)
        {
            holyLight.intensity = Mathf.Lerp(10f, 15f, Mathf.PingPong(elapsed * 20f, 1f));
            elapsed += Time.deltaTime;
            yield return null;
        }

        holyLight.intensity = 120;
        yield return new WaitForSeconds(1.3f);

        holyScale.x = 0;
        holyLight.gameObject.transform.DOScale(holyScale, 0.3f);
        yield return new WaitForSeconds(0.35f);
        Destroy(gameObject);
    }

    IEnumerator PlaySE(int steps = 12, float duration = 3f, float startPitch = 0.7f, float endPitch = 2.5f)
    {
        float stepDuration = duration / steps;  // 各ステップの間隔
        for (int i = 0; i < steps; i++)
        {
            // ピッチを段階的に上げる
            float pitch = Mathf.Lerp(startPitch, endPitch, (float)i / (steps - 1));

            // 1回だけ再生
            AudioManager.PlaySEWithPitch(se, pitch, 0.1f);
            // 後半ほど早くなるようにステップ時間を短くする
            float dynamicStep = stepDuration * Mathf.Pow(0.5f, (float)i / steps); // 徐々に早く
            yield return new WaitForSeconds(dynamicStep);
        }

        // 最後の音を少し長めに残す
        yield return new WaitForSeconds(se.length);
    }

    IEnumerator PlaySE2()
    {
        yield return new WaitForSeconds(1.4f);
        AudioManager.PlaySE(se2, 0.5f);

    }

    void Update()
    {

    }
}
