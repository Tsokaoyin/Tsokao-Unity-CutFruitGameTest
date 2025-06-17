using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-1)]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("音效")]
    [SerializeField] private AudioClip bgm;
    [SerializeField] private AudioClip gameOverSound;
    [SerializeField] private AudioClip newLifeSound;
    [SerializeField] private AudioClip coinSoun;
    private AudioSource audioSource;

    private void Awake()
    {
        //单例模式
        if (Instance!=null)
        {
            DestroyImmediate(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        //初始化音效组件
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.loop = true;
    }

    private void OnDestroy()
    {
        if(Instance==this)
        {
            Instance = null;
        }
    }

    private void Start()
    {
        //播放背景音乐
        if(bgm!=null)
        {
            audioSource.clip = bgm;
            audioSource.Play();
        }

        //显示开始界面
        //ShowStartMenu();
    }
}
