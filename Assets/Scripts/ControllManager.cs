using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControllManager : MonoBehaviour
{
    public int playerNumber = 0;
    public Vector2 PlayerPointer;
    [SerializeField] float speed = 10f;

    Transform pointer;
    PazzleManager pazzleManager;
    GameObject[] Houses;
    Transform BattleField;

    [SerializeField] GameObject[] golemHead;
    [SerializeField] GameObject[] Monsters;

    private GameInput controls;  // InputSystem用のコントロール
    private Vector2 moveInput;
    private bool choiceInput;

    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction choiceAction;

    [SerializeField] private bool choiceDown = false;
    [SerializeField] private bool choiceUp = false;

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["Move"];
        choiceAction = playerInput.actions["Choice"];

        choiceAction.performed += ctx => choiceDown = true;   // 押した瞬間
        choiceAction.canceled += ctx => choiceUp = true;     // 離した瞬間
    }

    void Start()
    {
        pazzleManager = GameObject.Find("Field_" + playerNumber.ToString()).GetComponent<PazzleManager>();
        transform.SetParent(pazzleManager.transform);
        pointer = pazzleManager.pointer;
        Houses = pazzleManager.Houses;
        BattleField = pazzleManager.BattleField;
    }

    void Update()
    {
        // MoveとChoiceのアクションを取得
        moveInput = moveAction.ReadValue<Vector2>();  // Move: WASD または Gamepad Left Stick
        choiceInput = choiceAction.triggered;  // Choice: Space または Gamepad A

        SelectPeace();
        PointMove();
        CheckPeaceDown();
        CheckPeace();
        CheckPeaceUp();
        DrawCircle2D(pointer.position, pointerSize);
    }

    void PointMove()
    {
        // 移動処理
        pointer.position += new Vector3(moveInput.x, moveInput.y, 0) * speed * Time.deltaTime;

        float X = Mathf.Clamp(pointer.localPosition.x, -2.5f, 2.5f);
        float Y = Mathf.Clamp(pointer.localPosition.y, -2.5f, 2.5f);

        pointer.localPosition = new Vector2(X, Y);
    }

    GameObject selectingPeace;
    GameObject selectingPeacePrev;

    void SelectPeace()
    {
        Collider2D col;

        if (Physics2D.OverlapCircle(pointer.position, 0, LayerMask.GetMask("Peace")))
        {
            col = Physics2D.OverlapCircle(pointer.position, 0, LayerMask.GetMask("Peace"));
            selectingPeace = col.gameObject;
        }

        if (selectingPeacePrev == null || selectingPeacePrev != selectingPeace)
        {
            if (selectingPeacePrev != null && selectingPeacePrev.activeSelf)
            {
                selectingPeacePrev.GetComponent<Peace>().selecting = false;
            }

            if (selectingPeace != null && selectingPeace.activeSelf)
            {
                selectingPeacePrev = selectingPeace;
                selectingPeace.GetComponent<Peace>().selecting = true;
            }
        }
    }

    void CheckPeaceDown()
    {
        if (choiceDown && CanCheckPeace)
        {
            choiceDown = false;
            Collider2D col;

            if (selectingPeace != null)
            {
                Peace p = selectingPeace.gameObject.GetComponent<Peace>();
                p.check = true;
                if (p.peaceNumber != 4)
                {
                    SelectPeaceNumber = p.peaceNumber;
                }
                checkingPeace.Add(selectingPeace);
            }
        }
    }

    [SerializeField] float pointerSize = 1;
    List<GameObject> checkingPeace = new List<GameObject>();

    [SerializeField] float chainDis = 1.25f;
    [SerializeField] bool CanCheckPeace = true;
    [SerializeField] int SelectPeaceNumber = -1;

    void CheckPeace()
    {
        if (choiceAction.IsPressed() && CanCheckPeace && checkingPeace.Count > 0)
        {
            Collider2D col = Physics2D.OverlapPoint(pointer.position);

            if (col != null)
            {
                Peace p = col.gameObject.GetComponent<Peace>();
                Peace pPrev = null;
                if (checkingPeace[checkingPeace.Count - 1] != null)
                {
                    pPrev = checkingPeace[checkingPeace.Count - 1].GetComponent<Peace>();
                }

                if (p != null && pPrev != null)
                {
                    float dis = Vector2.Distance(p.transform.position, pPrev.transform.position);
                    if (p.check == false && chainDis >= dis && (SelectPeaceNumber == p.peaceNumber || 4 == p.peaceNumber || -1 == SelectPeaceNumber))
                    {
                        p.check = true;
                        if (SelectPeaceNumber == -1)
                        {
                            SelectPeaceNumber = p.peaceNumber;
                        }
                        checkingPeace.Add(col.gameObject);
                    }
                }
            }
        }
    }

    void CheckPeaceUp()
    {
        if (choiceUp && CanCheckPeace && checkingPeace.Count > 0)
        {
            choiceUp = false;
            CanCheckPeace = false;
            StartCoroutine(deletePeace(new List<GameObject>(checkingPeace)));
        }
    }

    IEnumerator deletePeace(List<GameObject> DeletingPeace)
    {
        CanCheckPeace = false;
        bool CanSpawnGolem = false;
        int goleBodyCount = 0;
        int peaceNumber = -1;
        for (int i = 0; i < DeletingPeace.Count; i++)
        {
            DeletingPeace[i].GetComponent<SpriteRenderer>().enabled = false;
            Peace PACE = DeletingPeace[i].GetComponent<Peace>();
            PACE.check = false;

            if (PACE.peaceNumber == 4)
            {
                if (!CanSpawnGolem)
                {
                    CanSpawnGolem = true;
                }
                else
                {
                    goleBodyCount++;
                }
            }
            else
            {
                if (peaceNumber == -1)
                {
                    peaceNumber = PACE.peaceNumber;
                }
                goleBodyCount++;
            }

            yield return new WaitForSeconds(0.1f);
        }

        int count = 0;
        while (DeletingPeace.Count > 0)
        {
            count++;
            Debug.Log(count);
            Vector2 pos = DeletingPeace[0].transform.position;
            if (count >= 6 && DeletingPeace.Count == 1)
            {
                Instantiate(pazzleManager.peaces[4], pos, Quaternion.identity, pazzleManager.transform);
            }
            Destroy(DeletingPeace[0]);
            DeletingPeace.RemoveAt(0);
        }

        if (CanSpawnGolem && goleBodyCount >= 3)
        {
            int MonstaersNum = Mathf.Clamp(goleBodyCount - 3, 0, 9);
            GameObject monstaer = Instantiate(Monsters[MonstaersNum], Houses[playerNumber].transform.position, Quaternion.identity, BattleField);
            /*GameObject GOLEM = Instantiate(golemHead[playerNumber], Houses[playerNumber].transform.position, Quaternion.identity, BattleField);
            Golem Spr = GOLEM.GetComponent<Golem>();
            Spr.buffType = peaceNumber;
            Spr.player = playerNumber + 1;
            Spr.GolemSize = goleBodyCount;*/
        }

        checkingPeace.Clear();
        SelectPeaceNumber = -1;

        yield return new WaitForSeconds(0.5f);

        yield return pazzleManager.PeaceSet();

        CanCheckPeace = true;
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
}
