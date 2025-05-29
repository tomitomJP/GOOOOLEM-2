using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Unity.Mathematics;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] Text[] Ready;
    [SerializeField] Outline[] ReadyOutLine;
    [SerializeField] Text[] Status;
    [SerializeField] PlayerJoinManager playerJoinManager;
    [SerializeField] Text CountDownText;
    [SerializeField] PazzleManager[] pazzleManager;
    [SerializeField] GameObject ReadyCanvas;

    IEnumerator Start()
    {
        ReadyCanvas.SetActive(true);
        Status[1].text = "";

        Status[0].text = "PRESS THE A BUTTON";

        StartCoroutine(effectColor(0));
        while (playerJoinManager.currentPlayerCount == 0)
        {
            yield return null;
        }
        Status[0].text = "OK";

        Status[1].text = "PRESS THE A BUTTON";

        StartCoroutine(effectColor(1));
        while (playerJoinManager.currentPlayerCount == 1)
        {
            yield return null;
        }
        Status[1].text = "OK";

        float time;
        float timer;

        timer = 0;
        time = 0.5f;
        string number = "3";

        CountDownText.text = number;
        while (timer <= time)
        {
            CountDownText.fontSize = (int)Mathf.Lerp(30, 120, timer / time);
            timer += Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(0.5f);

        timer = 0;
        time = 0.5f;
        number = "2";

        CountDownText.text = number;
        while (timer <= time)
        {
            CountDownText.fontSize = (int)Mathf.Lerp(30, 120, timer / time);
            timer += Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(0.5f);

        timer = 0;
        time = 0.5f;
        number = "1";

        CountDownText.text = number;
        while (timer <= time)
        {
            CountDownText.fontSize = (int)Mathf.Lerp(30, 120, timer / time);
            timer += Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(0.5f);

        timer = 0;
        time = 0.5f;
        number = "START";

        CountDownText.text = number;
        while (timer <= time)
        {
            CountDownText.fontSize = (int)Mathf.Lerp(30, 60, timer / time);
            timer += Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(0.5f);


        ReadyCanvas.SetActive(false);
        pazzleManager[0].enabled = true;
        pazzleManager[1].enabled = true;

    }

    IEnumerator effectColor(int num)
    {
        int player = num;
        Color[] colors = { Color.white, Color.black };
        int i = 0;
        while (playerJoinManager.currentPlayerCount == player)
        {
            ReadyOutLine[player].effectColor = colors[i];
            i = (i + 1) % 2;
            yield return new WaitForSeconds(0.6f);
        }
    }

    [SerializeField] Text RedGameOverText;
    [SerializeField] Text BlueGameOverText;
    [SerializeField] House houseRed;
    [SerializeField] House houseBlue;
    [SerializeField] GameObject blackScreen;
    [SerializeField] Text GameOverMessage;
    public bool gameOver = false;
    public bool CanRemake = false;
    [SerializeField] private InputAction playerJoinInputAction = default;

    void Update()
    {
        if (!gameOver)
        {
            if (houseRed.hp <= 0)
            {
                StartCoroutine(GameOver(false));
            }
            if (houseBlue.hp <= 0)
            {
                StartCoroutine(GameOver(true));
            }
        }
    }


    IEnumerator GameOver(bool RedWin)
    {
        gameOver = true;
        blackScreen.SetActive(true);
        if (RedWin)
        {
            StartCoroutine(GameOverWin(RedGameOverText));
            StartCoroutine(GameOverLose(BlueGameOverText));
        }
        else
        {
            StartCoroutine(GameOverWin(BlueGameOverText));
            StartCoroutine(GameOverLose(RedGameOverText));
        }

        yield return new WaitForSeconds(2f);
        GameOverMessage.text = "Aボタンを押して再マッチ";

        CanRemake = true;



    }
    void OnEnable()
    {
        // Aボタン = buttonSouth（Xbox：A / PS：× / Switch：B）
        playerJoinInputAction.Enable();

        playerJoinInputAction.performed += OnAPressed;
    }

    void OnDisable()
    {
        playerJoinInputAction.performed -= OnAPressed;
        playerJoinInputAction.Disable();
    }

    void OnAPressed(InputAction.CallbackContext context)
    {
        // ゲームオーバー状態でのみ再マッチ受け付ける
        if (gameOver)
        {
            GameOverMessage.text = "再マッチを確認";
            Invoke("Remake", 1f);
        }
    }

    void Remake()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    IEnumerator GameOverWin(Text winText)
    {
        winText.text = "VICTORY";
        winText.color = Color.green;
        Transform TectPos = winText.transform;

        float time;
        float timer;

        timer = 0;
        time = 0.5f;
        while (time >= timer)
        {
            float t = timer / time;
            winText.fontSize = (int)math.lerp(0, 120, t);
            timer += Time.deltaTime;
            yield return null;
        }

        timer = 0;
        time = 0.2f;
        while (time >= timer)
        {
            float t = timer / time;
            winText.fontSize = (int)math.lerp(120, 80, t);
            timer += Time.deltaTime;
            yield return null;
        }

        timer = 0;
        time = 0.2f;
        while (time >= timer)
        {
            float t = timer / time;
            winText.fontSize = (int)math.lerp(80, 100, t);
            timer += Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator GameOverLose(Text winText)
    {
        winText.text = "DEFEAT";

        Transform TectPos = winText.transform;

        float time;
        float timer;

        TectPos.eulerAngles = new Vector3(0, 0, -25);
        TectPos.position = new Vector3(TectPos.position.x, 3);

        timer = 0;
        time = 0.3f;
        while (time >= timer)
        {
            float t = timer / time;
            winText.fontSize = (int)math.lerp(100, 90, t);
            TectPos.position = Vector2.Lerp(new Vector3(TectPos.position.x, 3), new Vector3(TectPos.position.x, -0.5f), t);
            timer += Time.deltaTime;
            yield return null;
        }

        timer = 0;
        time = 0.2f;
        while (time >= timer)
        {
            float t = timer / time;
            winText.fontSize = (int)math.lerp(90, 100, t);
            TectPos.position = Vector2.Lerp(new Vector3(TectPos.position.x, -0.5f), new Vector3(TectPos.position.x, 0.5f), t);
            timer += Time.deltaTime;
            yield return null;
        }

    }

}


