using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SlideButton : MonoBehaviour
{
    [SerializeField] private GameObject[] illustrations;
    [SerializeField] private Transform rightStartPos; // 右から入る開始位置
    [SerializeField] private Transform centerPos;     // 中央の表示位置（終点）
    [SerializeField] private Transform leftEndPos;    // 左へスライドして消える位置
    [SerializeField] private float slideDuration = 0.5f;

    private int currentIndex = 0;
    private GameObject currentObj;
    private GameObject nextObj;

    public void Onclick()
    {
        SlideToNext();
        Debug.Log("ボタンが押された");

    }

    public void Start()
    {
        currentObj = Instantiate(illustrations[0], transform);
        currentObj.transform.position = centerPos.position;

    }

    public void SlideToNext()
    {
        if (currentIndex + 1 >= illustrations.Length) return;

        currentIndex++;

        // 次の画像は右の開始位置にセット
        nextObj = Instantiate(illustrations[currentIndex], transform);
        nextObj.transform.position = rightStartPos.position;

        // 現在の画像を左の終了位置にスライド
        currentObj.transform.DOMove(leftEndPos.position, slideDuration);

        // 次の画像を中央にスライド
        nextObj.transform.DOMove(centerPos.position, slideDuration).OnComplete(() =>
        {
            Destroy(currentObj);
            currentObj = nextObj;
            nextObj = null;
        });
    }
}
