using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    //爆炸音效
    public AudioClip explodeSound;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            GetComponent<Collider>().enabled = false;

            //播放爆炸音效
            if(explodeSound!=null)
            {
                AudioSource.PlayClipAtPoint(explodeSound,transform.position);
            }

            ////统治游戏管理器炸弹爆炸
            //GameManager.Instance.BombExplode();

            ////回收到对象池
            //ObjecPool.Instance.ReturnToPool(gameObject);
        }
    }

    /// <summary>
    /// 重置炸弹状态
    /// </summary>
    public void ResetBomb()
    {
        GetComponent<Collider>().enabled=true;
    }
}
