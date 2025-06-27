using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance { get; private set; }

    /*
     * 类型定义：Dictionary<GameObject,Queue<GameObject>>表明这是一个字典，键（Key）是GameObject类型，通常
     * 代表预制体（Prefab）；值（Value）是Queue<GameObject>类型，即游戏对象的队列。队列遵循先进先出（FIFO）原则。
     * 变量名objectPools用于存储不同预制体对应的对象池。每个预制体对应一个队列，
     * 队列中存放该预制体实例化后的游戏对象。
     * 初始化：new Dictionary<GameObject,Queue<GameObject>>()在声明变量时创建了一个新的字典实例。
     */
    private Dictionary<GameObject, Queue<GameObject>> objectPools = new Dictionary<GameObject, Queue<GameObject>>();
    /*
     * 类型定义：Dictionary<GameObject,GameObject>也是一个字典，键和值都是GameObject类型。一般键代表实例化后的游戏对象，
     * 值代表该游戏对象对应的预制体。
     * 变量名：prefabLookup用于通过实例化的游戏对象快速查找器对应的预制体。
     * 初始化：new Dictionary<GameObject,GameObject>()在声明变量的同时创建了一个新的字典实例。
     */
    private Dictionary<GameObject, GameObject> prefabLookup = new Dictionary<GameObject, GameObject>();

    private void Awake()
    {
        if(Instance!=null&&Instance!=this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    /// <summary>
    /// 创建对象池
    /// </summary>
    /// <param name="prefab">需要放入对象池的预制体</param>
    /// <param name="poolSize">对象池的初始大小</param>
    public void CreatePool(GameObject prefab,int poolSize)
    {
        //检查objectPools字典中是否已经存在该预制体对应的对象池
        //ContainsKey():这是Dictionary类的一个方法，作用是检查字典中是否存在指定键。
        //prefab是传入的指定参数，通常代表一个预制体游戏对象。若objectPools字典里存在以prefab为键的对象，
        //该方法会返回true，反之则返回false。
        if(!objectPools.ContainsKey(prefab))
        {
            //如果不存在，则为该预制体添加一个新的对象池，对象池使用队列来存储游戏对象
            objectPools.Add(prefab, new Queue<GameObject>());

            //循环poolSize次，创建指定数量的游戏对象并添加到对象池中
            for(int i=0;i<poolSize;i++)
            {
                //使用Instantiate方法实例化预制体，创建一个新的游戏对象
                GameObject obj = Instantiate(prefab);
                //将新创建的游戏对象设置为非激活状态，这样它在场景中就不会显示或执行逻辑
                obj.SetActive(false);
                //将新创建的游戏对象添加到该预制体对应的对象池队列中。
                objectPools[prefab].Enqueue(obj);

                //将实例化的游戏对象和对应的预制体添加到 prefabLookup 字典中，方便后续查找
                prefabLookup.Add(obj, prefab);
            }
        }
    }

    /// <summary>
    /// 从对象池获取对象
    /// </summary>
    /// <param name="prefab">要获取的对象的预制体模板</param>
    /// <param name="position">对象实例化后的位置</param>
    /// <param name="rotation">对象实例化后的旋转角度</param>
    /// <returns></returns>
    public GameObject GetFromPool(GameObject prefab,Vector3 position,Quaternion rotation)
    {
        //检查对象池是否存在,若字典中已存在该预制体的池，则继续检查池中是否有可用对象。
        if (objectPools.ContainsKey(prefab))
        {
            //从池中获取可用对象
            if (objectPools[prefab].Count>0)
            {
                //从队列中取出一个对象（Dequeue()），设置其位置和旋转。
                GameObject obj = objectPools[prefab].Dequeue();
                obj.transform.position = position;
                obj.transform.rotation = rotation;
                //激活对象（SetActive(true)），使其重新参与游戏逻辑。
                obj.SetActive(true);

                //重置对象
                if(obj.CompareTag("Fruit"))
                {
                    obj.GetComponent<Fruit>().ResetFruit();
                }
                else if(obj.CompareTag("Bomb"))
                {
                    obj.GetComponent<Bomb>().ResetBomb();
                }

                return obj;
            }
            else
            {
                //池中没有可用对象，创建新对象
                GameObject obj = Instantiate(prefab, position, rotation);
                prefabLookup.Add(obj, prefab);
                return obj;
            }
        }
        else
        {
            //如果池不存在，创建新池
            CreatePool(prefab, 3);
            return GetFromPool(prefab, position, rotation);
        }
    }

    /// <summary>
    /// 返回对象池
    /// </summary>
    /// <param name="obj">被返回的对象</param>
    public void ReturnToPool(GameObject obj)
    {
        if(prefabLookup.ContainsKey(obj))
        {
            GameObject prefab = prefabLookup[obj];

            if(objectPools.ContainsKey(prefab))
            {
                obj.SetActive(false);
                objectPools[prefab].Enqueue(obj);
            }
        }
        else
        {
            //如果对象不再池中，销毁它
            Destroy(obj);
        }
    }

    //将所有对象回收到对象池
    public void ReturnAllToPool()
    {
        //查找所有水果和炸弹
        Fruit[] fruits = FindObjectsOfType<Fruit>();
        Bomb[] bombs = FindObjectsOfType<Bomb>();

        foreach(Fruit fruit in fruits)
        {
            ReturnToPool(fruit.gameObject);
        }

        foreach(Bomb bomb in bombs)
        {
            ReturnToPool(bomb.gameObject);
        }
    }

}
