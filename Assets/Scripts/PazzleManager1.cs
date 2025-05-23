using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PazzleManager_1 : MonoBehaviour
{
    [SerializeField] int fieldWidth = 10;
    [SerializeField] int fieldHeight = 10;
    [SerializeField] GameObject[] peaces;
    [SerializeField] float spacing = 1.1f; // ピースの間隔
    [SerializeField] GameObject peacePearent;

    Dictionary<Vector2Int, GameObject> grid = new Dictionary<Vector2Int, GameObject>();
    List<GameObject> destroyPeace = new List<GameObject>();

    void Start()
    {
        PeaceSet();
    }

    [SerializeField] bool movepeace = false;
    void Update()
    {
        if (movepeace)
        {
            StartShift();
            movepeace = false;
        }
    }

    void PeaceSet()
    {
        grid.Clear();

        Vector2Int[] directions = new Vector2Int[]
        {
            Vector2Int.right,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.up
        };

        int x = fieldWidth / 2;
        int y = fieldHeight / 2;
        int dir = 0;
        int steps = 1;

        int count = 0;
        int total = fieldWidth * fieldHeight;

        // 中心からピースを置き始める
        GameObject first = InstantiateRandomPeace(x, y);
        grid[new Vector2Int(x, y)] = first;
        count++;

        while (count < total)
        {
            for (int i = 0; i < 2; i++) // step数を2回繰り返して方向を変える
            {
                for (int j = 0; j < steps; j++)
                {
                    x += directions[dir].x;
                    y += directions[dir].y;

                    if (x >= 0 && x < fieldWidth && y >= 0 && y < fieldHeight)
                    {
                        GameObject obj = InstantiateRandomPeace(x, y);
                        grid[new Vector2Int(x, y)] = obj;
                        count++;
                        if (count >= total) return;
                    }
                }
                dir = (dir + 1) % 4;
            }
            steps++;
        }
    }

    GameObject InstantiateRandomPeace(int x, int y)
    {
        if (peaces.Length == 0) return null;

        GameObject prefab = peaces[Random.Range(0, peaces.Length)];
        GameObject _peace = Instantiate(prefab);
        StartCoroutine(AnimatePieceOnStart(_peace, x, y)); // 生成時のアニメーション
        _peace.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f); // ピースのサイズを0.5に設定
        return _peace;
    }

    // ピース生成時のアニメーション
    IEnumerator AnimatePieceOnStart(GameObject piece, int x, int y)
    {
        Vector3 targetPos = new Vector3(
            (x - fieldWidth / 2) * spacing,
            (y - fieldHeight / 2) * spacing,
            0f
        );

        piece.transform.SetParent(peacePearent.transform);
        piece.transform.localPosition = targetPos + new Vector3(0, 0, -10); // 画面外から少しずらす
        piece.transform.localScale = Vector3.zero; // 最初はスケールを0にしておく

        // ピースが画面内に出現するアニメーション
        float duration = 0.5f;
        float time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / duration);
            piece.transform.localPosition = Vector3.Lerp(piece.transform.localPosition, targetPos, t);
            piece.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one * 0.5f, t); // サイズもアニメーション
            yield return null;
        }

        piece.transform.localPosition = targetPos;
        piece.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f); // サイズを最終的に0.5に設定
    }

    void MovePeace(GameObject peace, int x, int y)
    {
        Vector3 pos = new Vector3(
            (x - fieldWidth / 2) * spacing,
            (y - fieldHeight / 2) * spacing,
            0f
        );

        peace.transform.SetParent(peacePearent.transform);
        peace.transform.localPosition = pos;
        peace.transform.eulerAngles = new Vector3(0, 0, 45);
    }

    // 👇 呼び出すと全体を中心にずらし、空いたマスに新ピースを生成する
    public void StartShift()
    {
        StartCoroutine(ShiftAllToCenterAndFill());
    }

    IEnumerator ShiftAllToCenterAndFill()
    {
        Dictionary<Vector2Int, GameObject> newGrid = new Dictionary<Vector2Int, GameObject>();
        Vector2Int center = new Vector2Int(fieldWidth / 2, fieldHeight / 2);

        List<IEnumerator> moveCoroutines = new List<IEnumerator>();
        List<GameObject> toBeDestroyed = new List<GameObject>();

        // 1. 全ピースを中心に向けて1マス移動（アニメ付き）
        foreach (var kvp in grid)
        {
            Vector2Int oldPos = kvp.Key;
            GameObject piece = kvp.Value;

            Vector2Int offset = (center - oldPos);
            Vector2Int direction = new Vector2Int(
                Mathf.Clamp(offset.x, -1, 1),
                Mathf.Clamp(offset.y, -1, 1)
            );
            Vector2Int newPos = oldPos + direction;

            if (newGrid.ContainsKey(newPos))
            {
                // 新しい位置にピースがすでにある場合、後から来たピースを消去
                GameObject existingPiece = newGrid[newPos];
                if (existingPiece != null)
                {
                    //Destroy(existingPiece); // 先に存在するピースを削除
                    destroyPeace.Add(existingPiece);
                    existingPiece.GetComponent<SpriteRenderer>().sortingOrder = -1;
                }
            }

            newGrid[newPos] = piece;

            Vector3 targetPos = new Vector3(
                (newPos.x - fieldWidth / 2) * spacing,
                (newPos.y - fieldHeight / 2) * spacing,
                0f
            );

            moveCoroutines.Add(MovePeaceSmooth(piece, targetPos));
        }

        // すべての移動コルーチンを並列で実行
        foreach (var coroutine in moveCoroutines)
        {
            StartCoroutine(coroutine);
        }

        // アニメーション時間待機
        yield return new WaitForSeconds(0.35f);

        for (int i = destroyPeace.Count - 1; 0 <= i; i--)
        {
            Destroy(destroyPeace[i]);
        }
        destroyPeace.Clear();

        // 2. 空きマスを補完
        for (int x = 0; x < fieldWidth; x++)
        {
            for (int y = 0; y < fieldHeight; y++)
            {
                Vector2Int pos = new Vector2Int(x, y);
                if (!newGrid.ContainsKey(pos))
                {
                    GameObject newPiece = InstantiateRandomPeace(x, y);
                    newGrid[pos] = newPiece;
                }
            }
        }

        // 最終的なgrid更新
        grid = newGrid;
    }

    IEnumerator MovePeaceSmooth(GameObject piece, Vector3 targetPos, float duration = 0.3f)
    {
        if (piece == null) yield break;

        Vector3 startPos = piece.transform.localPosition;
        float time = 0f;

        while (time < duration)
        {
            if (piece == null) yield break;

            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / duration);
            piece.transform.localPosition = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }

        if (piece != null)
        {
            piece.transform.localPosition = targetPos;
        }
    }
}
