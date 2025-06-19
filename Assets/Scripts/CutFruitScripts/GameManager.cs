using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR;

[DefaultExecutionOrder(-1)]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("��Ч")]
    [SerializeField] private AudioClip bgm;
    [SerializeField] private AudioClip gameOverSound;
    [SerializeField] private AudioClip newLifeSound;
    [SerializeField] private AudioClip coinSound;
    private AudioSource audioSource;

    [Header("��Ϸ���")]
    [SerializeField] private Blade blade;
    [SerializeField] private Spawner spawner;
    [SerializeField] private Image fadeImage;

    [Header("UI���")]
    [SerializeField] private Text scoreText;
    [SerializeField] private Text livesText;
    [SerializeField] private Text timeText;  //ʱ��ģʽ����ʱ
    [SerializeField] private Text coinText;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject revivePanel;
    [SerializeField] private GameObject pausePanel;

    [Header("����ϵͳ")]
    [SerializeField] private GameObject bronzeMedal;
    [SerializeField] private GameObject silverMedal;
    [SerializeField] private GameObject goldMedal;
    [SerializeField] private int bronzeThreshold = 100;
    [SerializeField] private int silverThreshold = 300;
    [SerializeField] private int goldThreshold = 500;


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
            //��ʼ����Ч���
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.loop = true;
            DontDestroyOnLoad(this.gameObject);
        }

       
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
        //if (bgm != null)
        //{
        //    audioSource.clip = bgm;
        //    audioSource.Play();
        //}

        //��ʾ��ʼ����
        ShowStartMenu();
    }

    /// <summary>
    /// ��ʾ��ʼ����
    /// </summary>
    private void ShowStartMenu()
    {
        SceneManager.LoadScene("StartMenu");
        //Time.timeScale = 0f;
    }

    /// <summary>
    /// ��ʾ������
    /// </summary>
    public void ShowMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
        Time.timeScale = 0f;
    }

    public void NewGame(GameMode mode)
    {
        currentGameMode = mode;
    }
}
