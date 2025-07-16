using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine.Rendering.Universal;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    [SerializeField] int playerCount = 2;
    [SerializeField] Text[] Ready;
    [SerializeField] PlayerJoinManager playerJoinManager;
    [SerializeField] Text CountDownText;
    [SerializeField] PazzleManager[] pazzleManager;
    [SerializeField] GameObject ReadyCanvas;
    [SerializeField] GameObject BlackScreenObj;
    [SerializeField] GameObject RuleCanvas;
    public PlayerInput[] playerInputs = new PlayerInput[2];
    bool[] choiceDown = new bool[2];
    bool[] choiceUp = new bool[2];
    bool[] ReadyOk = new bool[2];

    [SerializeField] GameObject GameCanvas;
    [SerializeField] ResultVeiwer resultVeiwer;

    [SerializeField] AudioClip PlayBgm;
    [SerializeField] AudioClip deathMatchStartSe;
    [SerializeField] AudioClip ReadyBgm;
    [SerializeField] AudioClip ResultBgm;
    [SerializeField] AudioClip scoreSE;
    [SerializeField] AudioClip FinishSE;
    [SerializeField] AudioClip GameStartCount;
    [SerializeField] AudioClip GameStart;
    [SerializeField] AudioClip TimerRemind;


    [SerializeField] Text timerText;
    [SerializeField] float timer = 100;
    public ResultVeiwer.resultData[] resultDatas = new ResultVeiwer.resultData[2];

    public enum Mode
    {
        ready,
        play,
        deathMatchReady,
        deathMatch,
    }

    public Mode mode = Mode.ready;
    IEnumerator Start()
    {
        //StartCoroutine(DeathMatchStart());
        yield return new WaitForSeconds(0.5f);
        AudioManager.PlayBGM(ReadyBgm);

        BlackScreenObj.SetActive(true);

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
        AudioManager.StopBGM();
        yield return new WaitForSeconds(0.5f);


        float time;
        float timer;

        timer = 0;
        time = 0.5f;
        string number = "3";

        CountDownText.text = number;
        AudioManager.PlaySE(GameStartCount);
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
        AudioManager.PlaySE(GameStartCount);
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
        AudioManager.PlaySE(GameStartCount);
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
        AudioManager.PlaySE(GameStart);
        AudioManager.PlayBGM(PlayBgm);

        while (timer <= time)
        {
            CountDownText.fontSize = (int)Mathf.Lerp(30, 60, timer / time);
            timer += Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(0.5f);

        mode = Mode.play;


        for (int i = 0; i < playerCount; i++)
        {
            pazzleManager[i].enabled = true;

        }

        ReadyCanvas.SetActive(false);
        BlackScreenObj.SetActive(false);

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
    bool timer10 = false;

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

            if (mode == Mode.play)
            {
                if (timer <= 0)
                {
                    timer = 0;
                    StartCoroutine(DeathMatchStart());
                    timerText.enabled = false;
                    mode = Mode.deathMatchReady;
                }
                if (timer < 10 && timer10 == false)
                {
                    timer10 = true;
                    AudioManager.BgmOption(1.05f);
                    StartCoroutine(Timer10());
                }
                if (timer > 0) timer -= Time.deltaTime;
            }



            timerText.text = Mathf.RoundToInt(timer / 60).ToString("D2") + ":" + Mathf.RoundToInt(timer % 60).ToString("D2");
        }
    }

    IEnumerator Timer10()
    {
        int originalSize = timerText.fontSize;
        DOTween.To(() => timerText.fontSize,
                   x => timerText.fontSize = x,
                   Mathf.RoundToInt(originalSize * 1.5f),
                   0.1f);
        timerText.DOColor(Color.red, 0.1f);

        AudioManager.PlaySEWithPitch(TimerRemind, 1);
        yield return new WaitForSeconds(0.5f);

        AudioManager.PlaySEWithPitch(TimerRemind, 1.05f);
        yield return new WaitForSeconds(0.25f);
        AudioManager.PlaySEWithPitch(TimerRemind, 1.1f);

        DOTween.To(() => timerText.fontSize,
                   x => timerText.fontSize = x,
                   Mathf.RoundToInt(originalSize * 1.5f),
                   0.5f)
               .OnComplete(() =>
               {
                   DOTween.To(() => timerText.fontSize,
                              x => timerText.fontSize = x,
                              originalSize,
                              0.1f);
               });
        yield return new WaitForSeconds(0.5f);

    }

    [SerializeField] Text DeathMatchTitleText;
    [SerializeField] Text DeathMatchSubTitleText;
    [SerializeField] GameObject[] DMchargePar = new GameObject[2];
    [SerializeField] GameObject[] DMbeamPar = new GameObject[2];
    [SerializeField] GameObject hibana;
    [SerializeField] Slider DMslider;
    [SerializeField] GameObject[] Lights;
    Light2D beamLight;
    [SerializeField] float maxGlowValue = 15;

    public Monsters[] DMGolem = new Monsters[2];
    public float[] DMGolemPowers = new float[2];
    [SerializeField] AudioClip beamCharge;
    [SerializeField] AudioClip beamShoot;


    IEnumerator DeathMatchStart()
    {

        GameObject[] monstras = GameObject.FindGameObjectsWithTag("Monster");

        foreach (GameObject monstar in monstras)
        {
            monstar.GetComponent<Monsters>().spdRate = 0;
            monstar.GetComponent<Monsters>().atkSpdRate = 0;

        }

        BlackScreenObj.SetActive(true);
        AudioManager.StopBGM();
        AudioManager.PlaySE(deathMatchStartSe, 0.4f);
        DeathMatchTitleText.enabled = true;
        DeathMatchSubTitleText.enabled = true;

        DeathMatchTitleText.transform.DOScale(Vector3.one * 0.8f, 0.3f);
        DeathMatchTitleText.transform.DORotate(new Vector3(0, 0, 1820), 0.3f, RotateMode.FastBeyond360);
        yield return new WaitForSeconds(0.3f);
        DeathMatchTitleText.transform.DOScale(Vector3.one, 0.3f);
        yield return new WaitForSeconds(3f);

        monstras = GameObject.FindGameObjectsWithTag("Monster");

        foreach (GameObject monstar in monstras)
        {
            monstar.GetComponent<Monsters>().Damaged(1000);
        }

        for (int i = 0; i < 2; i++)
        {
            DMGolem[i] = pazzleManager[i].controllManager.MonstatSpawn(10);
        }

        yield return new WaitForSeconds(3f);

        for (int i = 0; i < 2; i++)
        {
            DMGolem[i].spd = 0;
        }

        DeathMatchTitleText.DOFade(0, 0.5f);
        yield return new WaitForSeconds(0.6f);

        BlackScreenObj.SetActive(false);

        AudioManager.PlayBGM(PlayBgm);
        AudioManager.BgmOption(1.05f);

        mode = Mode.deathMatch;
        DMslider.gameObject.SetActive(true);


        GameObject[] beamPars = new GameObject[2];

        float DMtime = 15;
        float DMtimer = 0;

        for (int i = 0; i < 2; i++)
        {
            beamPars[i] = Instantiate(DMchargePar[i], DMGolem[i].transform.position, Quaternion.identity);
        }
        AudioManager.PlaySE(beamCharge, 0.7f);
        while (DMtime >= DMtimer)
        {
            for (int i = 0; i < 2; i++)
            {
                DMGolem[i].GetComponent<Goolem>().GolemBeamCharge();
            }
            DMslider.value = 1 - (DMtimer / DMtime);
            DMtimer += Time.deltaTime;
            yield return null;
        }

        DMslider.gameObject.SetActive(false);
        DeathMatchSubTitleText.enabled = false;

        mode = Mode.deathMatchReady;

        for (int i = 0; i < Lights.Length; i++)
        {
            //Lights[i].SetActive(false);
        }




        GameObject _hibana = Instantiate(hibana, new Vector2(0, DMGolem[0].transform.position.y), Quaternion.identity);
        beamLight = _hibana.GetComponent<Light2D>();

        float lightTggleTime = 0.02f;
        float lightTggleTimer = 0;
        bool lightFragg = true;

        for (int i = 0; i < 2; i++)
        {
            Destroy(beamPars[i]);
            beamPars[i] = Instantiate(DMbeamPar[i], DMGolem[i].transform.position, Quaternion.identity);

        }


        for (int m = 0; m < 4; m++)
        {
            DMtime = 2;
            DMtimer = 0;
            AudioManager.PlaySE(beamShoot);
            while (DMtime >= DMtimer)
            {
                for (int i = 0; i < 2; i++)
                {
                    DMGolem[i].GetComponent<Goolem>().GolemBeamCharge();

                    ParticleSystem p = beamPars[i].GetComponent<ParticleSystem>();
                    Vector2 pos = p.gameObject.transform.InverseTransformPoint(_hibana.transform.position);

                    var vel = p.velocityOverLifetime;
                    vel.orbitalOffsetX = new ParticleSystem.MinMaxCurve(pos.x);
                    vel.orbitalOffsetY = new ParticleSystem.MinMaxCurve(pos.y);

                }

                if (lightTggleTimer >= lightTggleTime)
                {
                    if (lightFragg) beamLight.intensity = maxGlowValue;
                    if (!lightFragg) beamLight.intensity = 0;

                    lightFragg = !lightFragg;
                    lightTggleTimer = 0;
                }
                lightTggleTimer += Time.deltaTime;
                DMtimer += Time.deltaTime;
                yield return null;
            }

            _hibana.transform.DOMove(new Vector2(Random.Range(DMGolem[0].transform.position.x + 1, DMGolem[1].transform.position.x - 1f), _hibana.transform.position.y), 0.3f);
        }

        if (DMGolemPowers[0] > DMGolemPowers[1])
        {
            _hibana.transform.DOMove(new Vector2(15, _hibana.transform.position.y), 0.3f);
            beamPars[1].SetActive(false);
        }
        else
        {
            _hibana.transform.DOMove(new Vector2(-15, _hibana.transform.position.y), 0.3f);
            beamPars[0].SetActive(false);

        }

        DMtime = 1;
        DMtimer = 0;
        AudioManager.PlaySE(beamShoot);
        while (DMtime >= DMtimer)
        {
            for (int i = 0; i < 2; i++)
            {
                DMGolem[i].GetComponent<Goolem>().GolemBeamCharge();

                ParticleSystem p = beamPars[i].GetComponent<ParticleSystem>();
                Vector2 pos = p.gameObject.transform.InverseTransformPoint(_hibana.transform.position);

                var vel = p.velocityOverLifetime;
                vel.orbitalOffsetX = new ParticleSystem.MinMaxCurve(pos.x);
                vel.orbitalOffsetY = new ParticleSystem.MinMaxCurve(pos.y);

                var main = p.main;
                main.startSize = 3f;

            }

            if (lightTggleTimer >= lightTggleTime)
            {
                if (lightFragg) beamLight.intensity = maxGlowValue * 1.3f;
                if (!lightFragg) beamLight.intensity = 0;

                lightFragg = !lightFragg;
                lightTggleTimer = 0;
            }
            lightTggleTimer += Time.deltaTime;
            DMtimer += Time.deltaTime;
            yield return null;
        }

        if (DMGolemPowers[0] > DMGolemPowers[1])
        {
            DMGolem[1].Damaged(10000);
            houseBlue.Damaged(10000);
        }
        else
        {
            DMGolem[0].Damaged(10000);
            houseRed.Damaged(10000);

        }

        yield return new WaitForSeconds(0.3f);
        beamPars[0].SetActive(false);
        beamPars[1].SetActive(false);

    }

    IEnumerator DeathMatchFinish()
    {
        Debug.Log("デスマッチ終わる");
        yield return null;
    }



    IEnumerator GameOver(bool RedWin)
    {
        AudioManager.PlaySE(FinishSE, 0.8f);
        AudioManager.StopBGM();
        //BlackScreenObj.SetActive(true);
        mode = Mode.ready;
        timerText.enabled = false;

        gameOver = true;
        blackScreen.SetActive(true);
        GameCanvas.SetActive(false);
        if (RedWin)
        {
            StartCoroutine(GameOverWin(RedGameOverText));
            StartCoroutine(GameOverLose(BlueGameOverText));
            pazzleManager[1].gameObject.AddComponent<Rigidbody2D>();

            resultDatas[0].won = true;

        }
        else
        {
            StartCoroutine(GameOverWin(BlueGameOverText));
            StartCoroutine(GameOverLose(RedGameOverText));
            pazzleManager[0].gameObject.AddComponent<Rigidbody2D>();
            resultDatas[1].won = true;

        }
        AudioManager.FadeInBGM(ResultBgm, 5f);

        yield return new WaitForSeconds(3f);
        resultVeiwer.gameObject.SetActive(true);
        resultVeiwer.resultDatas = resultDatas;
        blackScreen.SetActive(false);



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
        winText.text = "WIN";
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
        winText.text = "LOSE";

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


