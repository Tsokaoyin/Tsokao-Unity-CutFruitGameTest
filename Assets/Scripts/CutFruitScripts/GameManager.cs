using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-1)]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("��Ч")]
    [SerializeField] private AudioClip bgm;
    [SerializeField] private AudioClip gameOverSound;
    [SerializeField] private AudioClip newLifeSound;
    [SerializeField] private AudioClip coinSoun;
    private AudioSource audioSource;

    private void Awake()
    {
        //����ģʽ
        if (Instance!=null)
        {
            DestroyImmediate(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        //��ʼ����Ч���
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
        //���ű�������
        if(bgm!=null)
        {
            audioSource.clip = bgm;
            audioSource.Play();
        }

        //��ʾ��ʼ����
        //ShowStartMenu();
    }
}
