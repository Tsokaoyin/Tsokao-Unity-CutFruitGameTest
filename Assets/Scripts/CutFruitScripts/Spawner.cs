using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//确保该脚本所在的游戏对象上有Collider组件，如果没有则自动添加
[RequireComponent(typeof(Collider))]
public class Spawner : MonoBehaviour
{
    private Collider spawnArea;  //生成对象碰撞区域大小

    public GameObject[] fruitPrefabs;  //水果预制体数组
    public GameObject bombPrefab;  //炸弹预制体
    [Range(0, 1f)]   //随机比例
    public float bombChanger = 0.05f;   //炸弹随机比例

    public float minSpawnDelay = 0.25f;  //最小生成延迟时间
    public float maxSpawnDelay = 1f;  //最大生成延迟时间

    public float minAngle = -15f;  //最小生成角度
    public float maxAngle = 15f;  //最大生成角度

    public float minForce = 18f;  //最小生成的力
    public float maxForce = 22f;  //最大生成力

    public float maxLifeTime = 5f;  //生成对象最大生命值，超过改时间将被回收

    //对象池引用
    private ObjectPool objectPool;


    private void Awake()
    {
        
        spawnArea = GetComponent<Collider>();  //初始化碰撞组件
        //确保ObjectPool实例存在
        if(ObjectPool.Instance==null)
        {
            GameObject obj = new GameObject("ObjectPool");
            obj.AddComponent<ObjectPool>();
        }
        objectPool = ObjectPool.Instance;  //初始化对象池引用

        //初始化对象池
        InitializeObjectPool();
    }

    /// <summary>
    /// 初始化对象池
    /// </summary>
    private void InitializeObjectPool()
    {
        if(objectPool==null)
        {
            Debug.LogError("objectPool instance is unll in Spawner.InitializeObjectPool");
            return;
        }
        //为每种水果预加载对象
        foreach(GameObject prefab in fruitPrefabs)
        {
            objectPool.CreatePool(prefab, 10);
        }

        //为炸弹预加载对象
        objectPool.CreatePool(bombPrefab, 5);
    }

    private void OnEnable()
    {
        StartCoroutine(Spawn());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    /// <summary>
    /// 生成对象的协程方法，不断魂环生成水果或炸弹对象
    /// </summary>
    /// <returns></returns>
    private IEnumerator Spawn()
    {
        yield return new WaitForSeconds(2f);
        
        while(enabled)
        {
            GameObject prefab = fruitPrefabs[Random.Range(0, fruitPrefabs.Length)];

            if(Random.value<bombChanger)
            {
                prefab = bombPrefab;
            }

            Vector3 position = new Vector3
            {
                x = Random.Range(spawnArea.bounds.min.x, spawnArea.bounds.max.x),
                y = Random.Range(spawnArea.bounds.min.y, spawnArea.bounds.max.y),
                z=Random.Range(spawnArea.bounds.min.z,spawnArea.bounds.max.z)
            };

            Quaternion rotation = Quaternion.Euler(0f, 0f, Random.Range(minAngle, maxAngle));

            //使用对象池获取对象
            GameObject fruit = objectPool.GetFromPool(prefab, position, rotation);

            //设置自动回收
            StartCoroutine(DelayedReturnToPool(fruit, maxLifeTime));

            float force=Random .Range(minForce, maxForce);
            fruit.GetComponent<Rigidbody>().AddForce(fruit.transform.up * force, ForceMode.Impulse);

            yield return new WaitForSeconds(Random.Range(minSpawnDelay, maxSpawnDelay));
        }
    }

    private IEnumerator DelayedReturnToPool(GameObject obj,float delay)
    {
        yield return new WaitForSeconds(delay);

        //检查对象是否任然存在且未被切割
        //检查对象是否处于激活状态（SetActive（true））
        if (obj!=null&&obj.activeSelf)
        {
            objectPool.ReturnToPool(obj);
            
            //重置对象状态
            if(obj.CompareTag("Fruit"))
            {
                obj.GetComponent<Fruit>().ResetFruit();
            }
            else if(obj.CompareTag("Bomb"))
            {
                obj.GetComponent<Bomb>().ResetBomb();
            }
        }
    }
}
