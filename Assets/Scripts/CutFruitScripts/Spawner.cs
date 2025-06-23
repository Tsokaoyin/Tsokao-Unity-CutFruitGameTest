using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Spawner : MonoBehaviour
{
    private Collider spawnArea;

    public GameObject[] fruitPrefabs;
    public GameObject bombPrefab;
    [Range(0, 1f)]
    public float bombChanger = 0.05f;

    public float minSpawnDelay = 0.25f;
    public float maxSpawnDelay = 1f;

    public float minAngle = -15f;
    public float maxAngle = 15f;

    public float minForce = 18f;
    public float maxForce = 22f;

    public float maxLifeTime = 5f;

    //对象池引用
    private ObjectPool objectPool;

    private void Awake()
    {
        spawnArea = GetComponent<Collider>();
        objectPool = ObjectPool.Instance;

        //初始化对象池
        InitializeObjectPool();
    }

    //初始化对象池
    private void InitializeObjectPool()
    {
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
