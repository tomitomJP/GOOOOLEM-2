using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance = null;
    [SerializeField] AudioSource se;
    [SerializeField] AudioSource bgm;

    public float bgmVolume = 1;
    public float seVolume = 1;

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


    public static void PlaySEWithPitch(AudioClip clip, float pitch, float volumeScale = 1)
    {
        instance._PlaySEWithPitch(clip, pitch, volumeScale);
    }

    public void _PlaySEWithPitch(AudioClip clip, float pitch, float volumeScale)
    {
        // 一時的なAudioSource作成
        GameObject tempGO = new GameObject("TempAudio");
        AudioSource tempSource = tempGO.AddComponent<AudioSource>();

        tempSource.clip = clip;
        tempSource.pitch = pitch;
        tempSource.volume = seVolume * volumeScale; // 🔊 音量設定（0.0〜1.0）

        tempSource.Play();

        // 再生後に自動破棄（長さに応じて調整）
        Destroy(tempGO, clip.length / pitch);
    }

    public static void PlaySE(AudioClip clip, float volumeScale = 1)
    {
        instance.se.PlayOneShot(clip, instance.seVolume * volumeScale);
    }

    public static void PlayBGM(AudioClip clip, bool loop = true)
    {
        instance._PlayBGM(clip, loop);
    }

    public void _PlayBGM(AudioClip clip, bool loop)
    {
        if (bgm.clip == clip && bgm.isPlaying) return;

        bgm.clip = clip;
        bgm.loop = loop;
        bgm.volume = bgmVolume;
        bgm.Play();
    }

    public static void StopBGM()
    {
        instance._StopBGM();
    }

    public void _StopBGM()
    {
        bgm.Stop();
    }


    public static void BgmOption(float pitch)
    {
        instance._BgmOption(pitch);
    }

    public void _BgmOption(float pitch)
    {
        if (bgm.pitch == pitch && bgm.isPlaying) return;

        bgm.pitch = pitch;
    }

}
