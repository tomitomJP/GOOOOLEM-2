using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class SoloManager : MonoBehaviour
{
    public static SoloManager instance;
    [SerializeField] TextAsset[] nameFile;  // 0=男性, 1=女性, 2=中性
    public string[][] names = new string[3][];

    [SerializeField] GameObject[] humans;

    private List<int> availableCharacters = new List<int>();
    private List<List<string>> availableNames = new List<List<string>>();
    [SerializeField] int humanLevel = 0;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {



        StartCoroutine(GameLoop());
    }

    IEnumerator GameLoop()
    {
        while (true)
        {
            if (GameManager.GetMonsters(GameManager.type.human).Count == 0)
            {
                humanLevel++;
                _SetHT("勇者一行が魔王城前に登場！");
                InstantiateMembers();
            }

            yield return null;
        }
    }

    void InstantiateMembers()
    {

        availableCharacters = new List<int>();
        availableNames = new List<List<string>>();
        // 名前ファイルを読み込む
        for (int i = 0; i < names.Length; i++)
        {
            names[i] = nameFile[i].text.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            List<string> nameList = new List<string>(names[i]);
            Shuffle(nameList);
            availableNames.Add(nameList);
        }

        // キャラクター番号リストを作成してシャッフル
        for (int i = 0; i < humans.Length; i++)
            availableCharacters.Add(i);
        Shuffle(availableCharacters);

        // 1人目は humans[0] 固定
        InstantiateMember(0);

        // 残り3人はランダム
        for (int i = 0; i < 3; i++)
        {
            InstantiateMember(-1); // -1はランダム
        }
    }

    void InstantiateMember(int fixedIndex)
    {
        int charIndex;
        if (fixedIndex >= 0)
        {
            charIndex = fixedIndex;
            availableCharacters.Remove(charIndex);
        }
        else
        {
            // ランダムで被らないキャラクターを選ぶ
            charIndex = availableCharacters[0];
            availableCharacters.RemoveAt(0);
        }

        Human human = Instantiate(humans[charIndex], transform.position, Quaternion.identity).GetComponent<Human>();

        // 名前を取得（性別に応じたリストから被りなしで）
        int sexIndex = (int)human.sex;
        string name = availableNames[sexIndex][0];
        availableNames[sexIndex].RemoveAt(0);

        human.name = name;
        human.level = humanLevel;
    }

    // シャッフル用ユーティリティ
    void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int r = Random.Range(i, list.Count);
            T temp = list[i];
            list[i] = list[r];
            list[r] = temp;
        }
    }

    [SerializeField] GameObject humanTextWindow;
    [SerializeField] Transform WindowParent;
    [SerializeField] Transform startPos;
    [SerializeField] Transform endPos;

    [SerializeField] List<GameObject> HTWindows = new List<GameObject>();
    List<GameObject> HTAnimOrder = new List<GameObject>();


    public static void SetHT(string message, string color = "#FFFFFF", Human sayer = null, string p1 = "A")
    {
        instance._SetHT(message, color, sayer, p1);
    }

    void _SetHT(string message, string color = "#FFFFFF", Human sayer = null, string p1 = "A")
    {
        GameObject ht = Instantiate(humanTextWindow, startPos.position, Quaternion.identity, WindowParent);
        Text tessageText = ht.GetComponentInChildren<Text>();
        tessageText.text = "";
        if (sayer != null)
        {

            tessageText.text = $"<color=#FFA500>Lv{sayer.level} {sayer.name} {sayer.job}</color> \n";
        }
        tessageText.text += $"<color={color}>{message}</color>";


        StartCoroutine(HTAnim(ht));
    }

    IEnumerator HTAnim(GameObject obj)
    {

        HTAnimOrder.Add(obj);
        while (obj != HTAnimOrder[0])
        {
            yield return null;
        }

        for (int i = 0; i < HTWindows.Count; i++)
        {
            Debug.Log(i);
            float newY = HTWindows[i].transform.localPosition.y + 80;
            HTWindows[i].transform.DOLocalMoveY(newY, 0.15f);
        }
        HTWindows.Add(obj);
        obj.transform.DOMove(endPos.position, 0.2f);
        yield return new WaitForSeconds(0.4f);
        HTAnimOrder.Remove(obj);

        yield return new WaitForSeconds(10f);

        HTWindows.Remove(obj);
        Destroy(obj);

    }

    /// <summary>
    /// Colorを#RRGGBB形式の文字列に変換
    /// </summary>
    public static string ToHex(Color color)
    {
        int r = Mathf.RoundToInt(color.r * 255f);
        int g = Mathf.RoundToInt(color.g * 255f);
        int b = Mathf.RoundToInt(color.b * 255f);
        return $"#{r:X2}{g:X2}{b:X2}";
    }

    /// <summary>
    /// Colorを#RRGGBBAA形式の文字列に変換
    /// </summary>
    public static string ToHexWithAlpha(Color color)
    {
        int r = Mathf.RoundToInt(color.r * 255f);
        int g = Mathf.RoundToInt(color.g * 255f);
        int b = Mathf.RoundToInt(color.b * 255f);
        int a = Mathf.RoundToInt(color.a * 255f);
        return $"#{r:X2}{g:X2}{b:X2}{a:X2}";
    }
}
