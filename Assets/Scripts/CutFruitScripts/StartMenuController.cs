using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartMenuController : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private AudioClip menuMusic;
    [SerializeField] private AudioClip buttonSound;

    private void Start()
    {
        //���Ų˵�����
        AudioSource audioSource = GameManager.Instance.GetComponent<AudioSource>();
        if(menuMusic!=null)
        {
            audioSource.clip = menuMusic;
            audioSource.Play();
        }

        //���ÿ�ʼ��ť�¼�
        startButton.onClick.AddListener
        (() =>
         {
            //���Ű�ť��Ч
            if (buttonSound != null)
            {
                audioSource.PlayOneShot(buttonSound);
            }
            //GameManager.Instance.ShowMainMenu();
         }
        );
    }
}
