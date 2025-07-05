using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.UI;

public class Messager : MonoBehaviour
{
    Messager instance = null;
    Camera camera;
    [SerializeField] Canvas canvas;
    [SerializeField] GameObject MessageWindow;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void ViewText(string text, float duration)
    {
        camera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();

        Transform obj = Instantiate(MessageWindow, canvas.transform).transform;
        obj.GetChild(0).GetComponent<Text>().text = text;
        StartCoroutine(ViewText(obj, duration));


    }

    IEnumerator ViewText(Transform obj, float duration)
    {
        obj.DOScale(new Vector2(1, 1), 0.1f);
        yield return new WaitForSeconds(0.1f);

        yield return new WaitForSeconds(duration);

        obj.DOScale(new Vector2(1, 0), 0.1f);
        yield return new WaitForSeconds(0.1f);

        Destroy(obj.gameObject);

    }
}
