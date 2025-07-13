using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using DG.Tweening;


public class ControllManager : MonoBehaviour
{
    public int playerNumber = 0;
    public Vector2 PlayerPointer;
    [SerializeField] float speed = 10f;

    Transform pointer;
    PazzleManager pazzleManager;
    GameObject[] Houses;
    Transform BattleField;
    Transform MonstersPearent;

    [SerializeField] GameObject[] golemHead;
    [SerializeField] GameObject[] Monsters;

    private GameInput controls;  // InputSystem用のコントロール
    private Vector2 moveInput;
    private bool choiceInput;

    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction choiceAction;
    private InputAction openRule;
    [SerializeField] private bool choiceDown = false;
    [SerializeField] private bool choiceUp = false;
    [SerializeField] GameObject brokenText;
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] GameObject soulSpwanPar;
    [SerializeField] GameObject brokenPeaceParticle;

    Canvas canvas;

    GameManager gameManager;
    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["Move"];
        choiceAction = playerInput.actions["Choice"];
        openRule = playerInput.actions["OpenRule"];

        choiceAction.performed += ctx => choiceDown = true;   // 押した瞬間
        choiceAction.canceled += ctx => choiceUp = true;     // 離した瞬間
    }

    IEnumerator Start()
    {
        canvas = GameObject.FindWithTag("Canvas").GetComponent<Canvas>();
        pazzleManager = GameObject.Find("Field_" + playerNumber.ToString()).GetComponent<PazzleManager>();
        gameManager = GameObject.FindWithTag("GameController").GetComponent<GameManager>();

        gameManager.playerInputs[playerNumber] = playerInput;


        transform.SetParent(pazzleManager.transform);
        pointer = pazzleManager.pointer;
        Houses = pazzleManager.Houses;
        BattleField = pazzleManager.BattleField;
        MonstersPearent = pazzleManager.MonstersPearent;
        pazzleManager.controllManager = this;

        while (pazzleManager.enabled == false)
        {
            yield return null;
        }
        yield return pazzleManager.PeaceSet();
        CanCheckPeace = true;
    }

    void Update()
    {
        if (gameManager.gameOver) { return; }
        // MoveとChoiceのアクションを取得
        moveInput = moveAction.ReadValue<Vector2>();  // Move: WASD または Gamepad Left Stick
        choiceInput = choiceAction.triggered;  // Choice: Space または Gamepad A



        if (!CanCheckPeace)
        {
            choiceDown = false;
            choiceUp = false;
        }

        LineView();
        SelectPeace();
        PointMove();
        CheckPeaceDown();
        CheckPeace();
        CheckPeaceUp();
        DrawCircle2D(pointer.position, pointerSize);
        OpenMonstarsRule();
    }

    Vector2 GetFourDirection(Vector2 input)
    {
        if (input == Vector2.zero) return Vector2.zero;

        if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
        {
            return input.x > 0 ? Vector2.right : Vector2.left;
        }
        else
        {
            return input.y > 0 ? Vector2.up : Vector2.down;
        }
    }

    void MovePointer(Vector2 dir)
    {
        Vector3 move = dir * 0.5f; // 1マス移動
        pointer.localPosition += move;

        float X = Mathf.Clamp(pointer.localPosition.x, -2.5f, 2.5f);
        float Y = Mathf.Clamp(pointer.localPosition.y, -2.5f, 2.5f);

        pointer.localPosition = new Vector2(X, Y);
    }


    void OpenMonstarsRule()
    {

        if (openRule == null) return;

        // 押された瞬間かどうか
        if (openRule.WasPressedThisFrame())
        {
            pazzleManager.MonstarsRule.transform.DOLocalMove(pazzleManager.MonstarsRulePos[0], 0.5f);
            Debug.Log("Press");
        }

        // 離された瞬間かどうか
        if (openRule.WasReleasedThisFrame())
        {
            pazzleManager.MonstarsRule.transform.DOLocalMove(pazzleManager.MonstarsRulePos[1], 0.5f);
            Debug.Log("Release");
        }
    }



    // 変数定義（クラス内に入れてください）
    private Vector2 moveDirection = Vector2.zero;
    private float moveDelay = 0.5f;       // 最初の連続移動までの待ち時間
    private float repeatRate = 0.1f;      // 押し続けた場合の連続間隔
    private float moveTimer = 0f;
    private bool isHolding = false;
    Vector2 canDirection;

    // Update() 内で moveInput 読み取り後に呼ばれる
    void PointMove()
    {
        Vector2 input = GetFourDirection(moveInput);

        if (input != moveDirection)
        {
            moveDirection = input;
            moveTimer = moveDelay;
            isHolding = true;

            if (moveDirection != Vector2.zero && CanMoveDirection(moveDirection))
            {
                MovePointer(moveDirection);
            }
        }
        else if (moveDirection != Vector2.zero && isHolding)
        {
            moveTimer -= Time.deltaTime;
            if (moveTimer <= 0f && CanMoveDirection(moveDirection))
            {
                MovePointer(moveDirection);
                moveTimer = repeatRate;
            }
        }

        if (input == Vector2.zero)
        {
            moveDirection = Vector2.zero;
            isHolding = false;
        }
    }

    // 方向に基づいて pointer を動かす（1マス）
    bool CanMoveDirection(Vector2 dir)
    {
        if (checkingPeace.Count == 0) return true;

        // 横方向の制限
        if (dir == Vector2.right)
            return canDirection.x == 1 || canDirection.x == 2;
        if (dir == Vector2.left)
            return canDirection.x == -1 || canDirection.x == 2;

        // 縦方向の制限
        if (dir == Vector2.up)
            return canDirection.y == 1 || canDirection.y == 2;
        if (dir == Vector2.down)
            return canDirection.y == -1 || canDirection.y == 2;

        return false;
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
            choiceUp = false;
            choiceDown = false;
            Collider2D col;

            if (selectingPeace != null)
            {
                Peace p = selectingPeace.gameObject.GetComponent<Peace>();
                if (p.peaceNumber != 4)
                {
                    p.check = true;
                    SelectPeaceNumber = p.peaceNumber;
                    pazzleManager.BrickCount(checkingPeace.Count);
                    checkingPeace.Add(selectingPeace);
                    canDirection = pazzleManager.HilightPeace(checkingPeace, SelectPeaceNumber);
                }

            }
        }

        if (!CanCheckPeace)
        {
            choiceDown = false;
        }
    }

    [SerializeField] float pointerSize = 1;
    public List<GameObject> checkingPeace = new List<GameObject>();

    [SerializeField] float chainDis = 1.25f;
    [SerializeField] bool CanCheckPeace = true;
    [SerializeField] int SelectPeaceNumber = -1;

    void CheckPeace()
    {
        if (choiceAction.IsPressed() && CanCheckPeace && checkingPeace.Count > 0)
        {
            Collider2D col = Physics2D.OverlapPoint(pointer.position);

            if (selectingPeace != null)
            {
                Peace p = selectingPeace.gameObject.GetComponent<Peace>();
                Peace pPrev = null;
                if (checkingPeace[checkingPeace.Count - 1] != null)
                {
                    pPrev = checkingPeace[checkingPeace.Count - 1].GetComponent<Peace>();
                }

                if (p != null && pPrev != null)
                {
                    float dis = Vector2.Distance(p.transform.position, pPrev.transform.position);
                    bool isPrevPeace = checkingPeace.Count > 1 && selectingPeace == checkingPeace[checkingPeace.Count - 2];

                    if (
                        (p.check == false && chainDis >= dis && (SelectPeaceNumber == p.peaceNumber || 4 == p.peaceNumber)) // 新規選択
                        || isPrevPeace // 戻る操作を許可
                    )
                    {
                        if (isPrevPeace)
                        {
                            // 戻る時はチェックを解除して一つ戻る
                            pPrev.check = false;
                            checkingPeace.RemoveAt(checkingPeace.Count - 1);
                        }
                        else
                        {
                            p.check = true;
                            checkingPeace.Add(selectingPeace);
                        }

                        pazzleManager.BrickCount(checkingPeace.Count);
                        canDirection = pazzleManager.HilightPeace(checkingPeace, SelectPeaceNumber);
                    }
                }

            }
        }
        else
        {
            choiceDown = false;
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
        else if (!CanCheckPeace)
        {
            choiceUp = false;
        }
    }

    IEnumerator deletePeace(List<GameObject> DeletingPeace)
    {
        pazzleManager.ResetHilightPeace();

        CanCheckPeace = false;
        pazzleManager.blackScreen.SetActive(true);
        pazzleManager.BrickCountOff();

        bool CanSpawnGolem = false;
        int goleBodyCount = 0;
        int peaceNumber = -1;
        for (int i = 0; i < DeletingPeace.Count; i++)
        {
            DeletingPeace[i].GetComponent<SpriteRenderer>().enabled = false;
            Peace PACE = DeletingPeace[i].GetComponent<Peace>();
            PACE.check = false;

            DeletingPeace[i].GetComponent<SpriteRenderer>().sortingOrder = 16;


            ParticleSystem particleSystem = Instantiate(brokenPeaceParticle, DeletingPeace[i].transform.position, Quaternion.identity).GetComponent<ParticleSystem>();
            var main = particleSystem.main;
            main.startColor = Color.white;


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

        gameManager.resultDatas[playerNumber].deletePeaceCount += DeletingPeace.Count;
        BrokenText text = Instantiate(brokenText, DeletingPeace[DeletingPeace.Count - 1].transform.position, Quaternion.identity, canvas.transform).GetComponent<BrokenText>();
        text.num = DeletingPeace.Count.ToString();
        text.Spwan = CanSpawnGolem;

        yield return new WaitForSeconds(0.2f);


        int count = 0;
        while (DeletingPeace.Count > 0)
        {
            count++;
            //  Debug.Log(count);
            Vector2 pos = DeletingPeace[0].transform.position;
            if (count >= 6 && DeletingPeace.Count == 1)
            {
                Instantiate(soulSpwanPar, pos, Quaternion.identity);

                Instantiate(pazzleManager.peaces[4], pos, Quaternion.identity, pazzleManager.peacePearent.transform);
            }
            Destroy(DeletingPeace[0]);
            DeletingPeace.RemoveAt(0);
        }

        if (CanSpawnGolem /*&& goleBodyCount >= 3*/)
        {
            gameManager.resultDatas[playerNumber].spawnCount++;
            pazzleManager.house.DoorAnimTrigger();
            int MonstaersNum = Mathf.Clamp(goleBodyCount /*- 3*/, 0, 9);//-1.92
            Vector3 pos = Houses[playerNumber].transform.position;
            pos.y = -5f;
            GameObject monstaer = Instantiate(Monsters[MonstaersNum], pos, Quaternion.identity, MonstersPearent);

            Monsters monsters = monstaer.GetComponent<Monsters>();
            monsters.player = playerNumber;
            switch (peaceNumber)
            {
                case 0:
                    monsters.spdRate *= 1.15f;
                    break;
                case 1:
                    monsters.atkRate *= 1.3f;
                    break;
                case 2:
                    monsters.atkSpdRate *= 1.25f;
                    break;
                case 3:
                    monsters.hp *= 1.3f;
                    break;
            }
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

    void LineView()
    {
        if (checkingPeace.Count >= 2)
        {
            lineRenderer.positionCount = checkingPeace.Count;
            Vector3[] points = new Vector3[checkingPeace.Count];
            for (int i = 0; i < checkingPeace.Count; i++)
            {
                points[i] = checkingPeace[i].gameObject.transform.position;
            }
            lineRenderer.SetPositions(points);

        }
        else
        {
            lineRenderer.positionCount = 0;
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

            Debug.DrawLine(pointA, pointB, color, 0f, false);
        }
    }
}
