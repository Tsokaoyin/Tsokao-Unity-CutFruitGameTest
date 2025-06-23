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
    [SerializeField] private Text coinsText;
    [SerializeField] private GameObject gameOverPanel;  //��Ϸ�������
    [SerializeField] private GameObject revivePanel;  //��Ϸ�������
    [SerializeField] private GameObject pausePanel;  //��Ϸ��ͣ���

    [Header("����ϵͳ")]
    [SerializeField] private GameObject bronzeMedal;
    [SerializeField] private GameObject silverMedal;
    [SerializeField] private GameObject goldMedal;
    [SerializeField] private int bronzeThreshold = 100;
    [SerializeField] private int silverThreshold = 300;
    [SerializeField] private int goldThreshold = 500;

    //��Ϸ״̬
    public int score { get; private set; } = 0;
    private  int currentLives;
    private int initialLives=3;
    private int coins;
    private int sessionCoins = 0;  ///��ǰ��Ϸ��õĽ��
    private float gameTime = 0f;  //��Ϸʱ�䣨����ʱ��ģʽ��
    private float timeLimit = 90f;  //ʱ��ģʽ��ʱ������
    private bool isPaused = false;
    private bool isRevived = false;  //�Ƿ��Ѿ�����
    private GameMode currentGameMode = GameMode.Infinite;  //Ĭ������ʱ��ģʽ

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

        //���ؽ�Һ���߷���
        coins = PlayerPrefs.GetInt("coins", 0);
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

    /// <summary>
    /// ��ʼ��Ϸ
    /// </summary>
    /// <param name="mode"></param>
    public void NewGame(GameMode mode)
    {
        currentGameMode = mode;
        isRevived = false;
        sessionCoins = 0;

        //������Ϸ״̬
        Time.timeScale = 1f;
        score = 0;
        currentLives = initialLives;
        gameTime = timeLimit;

        //����UI
        scoreText.text = score.ToString();
        livesText.text = $"Lives:{currentLives}";
        coinsText.text = $"Coins:{coins}";
        timeText.text = $"Time:{Mathf.CeilToInt(gameTime)}";

        gameOverPanel.SetActive(false);
        revivePanel.SetActive(false);
        pausePanel.SetActive(false);
        UpdateMedalDisplay();

        //������
        ClearScene();

        blade.enabled = true;
        spawner.enabled = true;

        //������Ϸ����
        SceneManager.LoadScene("GameScene");
    }

    private void Update()
    {
        //ʱ��ģʽ����ʱ
        if(currentGameMode==GameMode.TimeLimit&&Time.timeScale>0)
        {
            gameTime -= Time.deltaTime;
            timeText.text = $"Time:{Mathf.CeilToInt(gameTime)}";

            if(gameTime<=0)
            {

            }
        }
    }

    private void GameOve()
    {
        
    }

    private void ClearScene()
    {
        //ʹ�ö���ػ���������Ϸ
    }

    /// <summary>
    /// ���ӷ���
    /// </summary>
    /// <param name="points">���������õķ���</param>
    public void IncreaseScore(int points)
    {
        score += points;
        scoreText.text = score.ToString();

        //ÿ100�ֻ��1���
        if(score%100==0)
        {
            AddCoins(1);
            sessionCoins++;
        }
    }

    public void AddCoins(int amout)
    {
        coins += amout;
        PlayerPrefs.SetInt("coins", coins);
        coinsText.text = $"Coins:{coins}";

        //���Ž����Ч
        if(coinSound!=null)
        {
            audioSource.PlayOneShot(coinSound);
        }
    }

    /// <summary>
    /// ���½�����ʾ
    /// </summary>
    private void UpdateMedalDisplay()
    {
        bronzeMedal.SetActive(score >= bronzeThreshold);
        silverMedal.SetActive(score >= silverThreshold);
        goldMedal.SetActive(score >= goldThreshold);
    }
}
