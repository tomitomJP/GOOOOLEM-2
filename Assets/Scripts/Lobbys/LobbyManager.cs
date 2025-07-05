using System.Collections;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour
{
    public enum Mode
    {
        title,
        menu,
    }
    public Mode mode = Mode.title;

    [SerializeField] GameObject anyKeyPress;
    [SerializeField] GameObject MenuWindw;
    ControllerManager controllerManager;

    [SerializeField] InputManagerLobby inputManagerLobby;
    Messager messager;

    public bool canOnButton = true;
    public float canOnButtonCT = 1;

    void Start()
    {
        controllerManager = GameObject.FindWithTag("ControllerManager").GetComponent<ControllerManager>();
        messager = GameObject.FindWithTag("Messager").GetComponent<Messager>();

    }

    // Update is called once per frame
    void Update()
    {
        SwitchMode();
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }


    }

    void SwitchMode()
    {
        if (mode == Mode.title)
        {
            if (Input.anyKeyDown)
            {
                Debug.Log("開く");

                SwitchMode(true);
            }
            else
            {
                //SwitchMode(false);

            }
        }
    }
    void SwitchMode(bool boo)
    {
        if (boo == true)
        {
            mode = Mode.menu;
            anyKeyPress.SetActive(false);
            MenuWindw.SetActive(true);
        }
        else
        {
            mode = Mode.title;
            anyKeyPress.SetActive(true);
            MenuWindw.SetActive(false);
        }
    }

    void ReconnectController()
    {
        controllerManager.PairWithDevice[0] = null;
        controllerManager.PairWithDevice[1] = null;

        var playerInputs = FindObjectsOfType<PlayerInput>();

        inputManagerLobby.currentPlayerCount = 0;
        inputManagerLobby.joinedDevices = default;
        inputManagerLobby.joinedDevices = new InputDevice[inputManagerLobby.maxPlayerCount];

        foreach (var playerInput in playerInputs)
        {
            Destroy(playerInput.gameObject);
        }
    }


    public void StartVSMButton()
    {
        if (!canOnButton) { return; }
        canOnButton = false;
        Debug.Log("Monstar");
        StartCoroutine(StartGame());

    }

    IEnumerator StartGame()
    {
        if (inputManagerLobby.currentPlayerCount == 2)
        {
            messager.ViewText("ゲームを開始します", 1);
            yield return new WaitForSeconds(1f);


            yield return new WaitForSeconds(0.2f);
            SceneManager.LoadScene("Game");

        }
        else
        {
            messager.ViewText("コントローラーを二つ接続してください", 1);
            yield return new WaitForSeconds(1f);
            canOnButton = true;

        }

    }

    public void StartVSHButton()
    {
        if (!canOnButton) { return; }
        canOnButton = false;
        Debug.Log("ニンゲン");
    }

    public void ResetControllerButton()
    {
        if (!canOnButton) { return; }
        Debug.Log("再接続");
        ReconnectController();

    }

    public void CloseButton()
    {
        Debug.Log("閉じる");
        SwitchMode(false);

    }
}
