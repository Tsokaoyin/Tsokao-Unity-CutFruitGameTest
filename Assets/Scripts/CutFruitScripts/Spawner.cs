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

    //���������
    private ObjectPool objectPool;

    private void Awake()
    {
        spawnArea = GetComponent<Collider>();
        objectPool = ObjectPool.Instance;

        //��ʼ�������
        InitializeObjectPool();
    }

    //��ʼ�������
    private void InitializeObjectPool()
    {
        //Ϊÿ��ˮ��Ԥ���ض���
        foreach(GameObject prefab in fruitPrefabs)
        {
            objectPool.CreatePool(prefab, 10);
        }

        //Ϊը��Ԥ���ض���
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

            //ʹ�ö���ػ�ȡ����
            GameObject fruit = objectPool.GetFromPool(prefab, position, rotation);

            //�����Զ�����
            StartCoroutine(DelayedReturnToPool(fruit, maxLifeTime));

            float force=Random .Range(minForce, maxForce);
            fruit.GetComponent<Rigidbody>().AddForce(fruit.transform.up * force, ForceMode.Impulse);

            yield return new WaitForSeconds(Random.Range(minSpawnDelay, maxSpawnDelay));
        }
    }

    private IEnumerator DelayedReturnToPool(GameObject obj,float delay)
    {
        yield return new WaitForSeconds(delay);

        //�������Ƿ���Ȼ������δ���и�
        if (obj!=null&&obj.activeSelf)
        {
            objectPool.ReturnToPool(obj);
            
            //���ö���״̬
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
