using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

public class MainMenuController : MonoBehaviour
{
    [Header("UI���")]
    [SerializeField] private Button infiniteModeButton;  //����ģʽ��ť
    [SerializeField] private Button timeModeButton;  //ʱ��ģʽ��ť
    [SerializeField] private Text coinsText;  //�����ʾ
    [SerializeField] private Text highScoreText;  //�߷�����ʾ
    [SerializeField] private AudioClip buttonSound;  //��ť����
    [SerializeField] private AudioClip MainMenuMusic;  //����������
    AudioSource audioSource;

    [Header("��Ϸ����")]
    [SerializeField] private int timeModeDuration = 90;  //ʱ��ģʽʱ�����룩

    private void Start()
    {
        //��ʾ��Һ���ߵ÷�
        coinsText.text = $"{PlayerPrefs.GetInt("coins",0)}";
        highScoreText.text = $"{PlayerPrefs.GetInt("highScore",0)}";

        //����ʱ�䰴ť
        if(infiniteModeButton!=null)
        {
            infiniteModeButton.onClick.AddListener(() => StartGame(GameMode.Infinite));
        }
        if(timeModeButton!=null)
        {
            timeModeButton.onClick.AddListener(() => StartGame(GameMode.TimeLimit));
        }

        //����ʱ��ģʽʱ��
        GameManager.Instance.timeLimit = timeModeDuration;

        audioSource = GameManager.Instance.GetComponent<AudioSource>();
        audioSource.clip = MainMenuMusic;
        audioSource.Play();
        audioSource.loop = true;
    }

    /// <summary>
    /// ��ʼ��Ϸ
    /// </summary>
    /// <param name="mode">��Ϸģʽ</param>
    private void StartGame(GameMode mode)
    {
        //���Ű�ť��Ч
         audioSource = GameManager.Instance.GetComponent<AudioSource>();
        if(buttonSound!=null)
        {
            audioSource.PlayOneShot(buttonSound);
        }
        audioSource.Pause();
        //��ʼ��Ϸ
        GameManager.Instance.NewGame(mode);
    }    
}
