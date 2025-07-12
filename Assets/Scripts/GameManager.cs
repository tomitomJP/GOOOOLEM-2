using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] int playerCount = 2;
    [SerializeField] Text[] Ready;
    [SerializeField] PlayerJoinManager playerJoinManager;
    [SerializeField] Text CountDownText;
    [SerializeField] PazzleManager[] pazzleManager;
    [SerializeField] GameObject ReadyCanvas;
    [SerializeField] GameObject GameOverBlackScreen;
    [SerializeField] GameObject RuleCanvas;
    public PlayerInput[] playerInputs = new PlayerInput[2];
    bool[] choiceDown = new bool[2];
    bool[] choiceUp = new bool[2];
    bool[] ReadyOk = new bool[2];


    [SerializeField] AudioSource BGM;
    IEnumerator Start()
    {
        BGM = GameObject.FindWithTag("SE_Monster").GetComponent<AudioSource>();

        for (int i = 0; i < 2; i++)
        {
            int index = i; // i をキャプチャして固定
            playerInputs[index].actions["Choice"].performed += ctx => choiceDown[index] = true;
            playerInputs[index].actions["Choice"].canceled += ctx => choiceUp[index] = true;
        }


        while (!(ReadyOk[0] && ReadyOk[1]))
        {
            for (int i = 0; i < 2; i++)
            {
                if (choiceDown[i])
                {
                    ReadyOk[i] = true;
                    Ready[i].text = "OK";
                }
            }

            yield return null;
        }

        RuleCanvas.SetActive(false);

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

        for (int i = 0; i < playerCount; i++)
        {
            pazzleManager[i].enabled = true;

        }

        BGM.gameObject.SetActive(true);

        ReadyCanvas.SetActive(false);

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

            pazzleManager[1].gameObject.AddComponent<Rigidbody2D>();
        }
        else
        {
            StartCoroutine(GameOverWin(BlueGameOverText));
            StartCoroutine(GameOverLose(RedGameOverText));
            pazzleManager[0].gameObject.AddComponent<Rigidbody2D>();

        }

        yield return new WaitForSeconds(5f);
        GameOverBlackScreen.SetActive(true);
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
        if (gameOver & CanRemake)
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
            winText.fontSize = (int)math.lerp(0, 100, t);
            timer += Time.deltaTime;
            yield return null;
        }

        timer = 0;
        time = 0.2f;
        while (time >= timer)
        {
            float t = timer / time;
            winText.fontSize = (int)math.lerp(100, 60, t);
            timer += Time.deltaTime;
            yield return null;
        }

        timer = 0;
        time = 0.2f;
        while (time >= timer)
        {
            float t = timer / time;
            winText.fontSize = (int)math.lerp(60, 80, t);
            timer += Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator GameOverLose(Text winText)
    {
        winText.text = "COLLAPSED";

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
            winText.fontSize = (int)math.lerp(100, 70, t);
            TectPos.position = Vector2.Lerp(new Vector3(TectPos.position.x, 3), new Vector3(TectPos.position.x, -0.5f), t);
            timer += Time.deltaTime;
            yield return null;
        }

        timer = 0;
        time = 0.2f;
        while (time >= timer)
        {
            float t = timer / time;
            winText.fontSize = (int)math.lerp(70, 80, t);
            TectPos.position = Vector2.Lerp(new Vector3(TectPos.position.x, -0.5f), new Vector3(TectPos.position.x, 0.5f), t);
            timer += Time.deltaTime;
            yield return null;
        }

    }

}


