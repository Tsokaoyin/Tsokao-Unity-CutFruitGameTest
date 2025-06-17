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
        //播放菜单音乐
        AudioSource audioSource = GameManager.Instance.GetComponent<AudioSource>();
        if(menuMusic!=null)
        {
            audioSource.clip = menuMusic;
            audioSource.Play();
        }

        //设置开始按钮事件
        startButton.onClick.AddListener
        (() =>
         {
            //播放按钮音效
            if (buttonSound != null)
            {
                audioSource.PlayOneShot(buttonSound);
            }
            //GameManager.Instance.ShowMainMenu();
         }
        );
    }
}
