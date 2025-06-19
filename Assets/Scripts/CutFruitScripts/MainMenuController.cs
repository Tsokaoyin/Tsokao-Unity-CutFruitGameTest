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

    [Header("��Ϸ����")]
    [SerializeField] private int timeModeDuration = 90;  //ʱ��ģʽʱ�����룩

    private void Start()
    {
        //��ʾ��Һ���ߵ÷�
        coinsText.text = $"Coins:{PlayerPrefs.GetInt("coins,0")}";
        highScoreText.text = $"High Score:{PlayerPrefs.GetInt("highScore", 0)}";

        //����ʱ�䰴ť
        infiniteModeButton.onClick.AddListener(() => StartGame(GameViewRenderMode.mode));
    }

    /// <summary>
    /// ��ʼ��Ϸ
    /// </summary>
    /// <param name="mode"></param>
    private void StartGame(GameViewRenderMode mode)
    {
        //���Ű�ť��Ч
        AudioSource audioSource = GameManager.Instance.GetComponent<AudioSource>();
        if(buttonSound!=null)
        {
            audioSource.PlayOneShot(buttonSound);
        }

        //��ʼ��Ϸ
        GameManager.Instance.NewGame();
    }    
}
