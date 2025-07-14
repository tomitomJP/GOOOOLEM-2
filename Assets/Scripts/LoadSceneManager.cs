using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadSceneManager : MonoBehaviour
{
    static LoadSceneManager instance;
    [SerializeField] SpriteRenderer fadeCanvas;
    [SerializeField] float fadeDuration = 1f;
    [SerializeField] GameObject LoadingParticle;
    [SerializeField] AudioClip icatchSE;


    void Awake()
    {
        // Singletonとして保持
        if (instance == null)
        {
            fadeCanvas.gameObject.SetActive(true);
            instance = this;
            DontDestroyOnLoad(gameObject); // シーンをまたいで保持
            StartCoroutine(StartFade());
        }
        else
        {
            Destroy(gameObject);
        }
    }

    static public void FadeLoadScene(string sceneName)
    {
        if (instance != null)
        {
            instance.StartCoroutine(instance.FadeAndLoad(sceneName));
        }
        else
        {
            Debug.LogError("LoadSceneManagerのインスタンスが存在しません！");
        }
    }

    IEnumerator StartFade()
    {
        yield return new WaitForSeconds(0.5f);

        // フェードイン
        yield return Fade(0f);
        fadeCanvas.gameObject.SetActive(false);

    }

    IEnumerator FadeAndLoad(string sceneName, bool OnParticle = true)
    {
        fadeCanvas.gameObject.SetActive(true);

        GameObject par = null;

        if (OnParticle)
        {
            AudioManager.PlaySE(icatchSE);
            Vector2 pos = GameObject.FindWithTag("MainCamera").transform.position;
            par = Instantiate(LoadingParticle, new Vector3(pos.x, pos.y, 0), Quaternion.identity, transform);
            yield return new WaitForSeconds(0.5f);
        }

        // フェードアウト
        yield return Fade(1f);

        if (OnParticle) Destroy(par);
        // シーン読み込み
        SceneManager.LoadScene(sceneName);
        yield return new WaitForSeconds(0.5f);

        // フェードイン
        yield return Fade(0f);
        fadeCanvas.gameObject.SetActive(false);

    }

    IEnumerator Fade(float targetAlpha)
    {
        Color startColor = fadeCanvas.color;
        Color targetColor = new Color(startColor.r, startColor.g, startColor.b, targetAlpha);
        float time = 0f;

        while (time < fadeDuration)
        {
            fadeCanvas.color = Color.Lerp(startColor, targetColor, time / fadeDuration);
            time += Time.unscaledDeltaTime;
            yield return null;
        }

        fadeCanvas.color = targetColor;
    }
}
