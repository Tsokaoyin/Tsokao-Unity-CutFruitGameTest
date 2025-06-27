using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//ȷ���ýű����ڵ���Ϸ��������Collider��������û�����Զ����
[RequireComponent(typeof(Collider))]
public class Spawner : MonoBehaviour
{
    private Collider spawnArea;  //���ɶ�����ײ�����С

    public GameObject[] fruitPrefabs;  //ˮ��Ԥ��������
    public GameObject bombPrefab;  //ը��Ԥ����
    [Range(0, 1f)]   //�������
    public float bombChanger = 0.05f;   //ը���������

    public float minSpawnDelay = 0.25f;  //��С�����ӳ�ʱ��
    public float maxSpawnDelay = 1f;  //��������ӳ�ʱ��

    public float minAngle = -15f;  //��С���ɽǶ�
    public float maxAngle = 15f;  //������ɽǶ�

    public float minForce = 18f;  //��С���ɵ���
    public float maxForce = 22f;  //���������

    public float maxLifeTime = 5f;  //���ɶ����������ֵ��������ʱ�佫������

    //���������
    private ObjectPool objectPool;


    private void Awake()
    {
        
        spawnArea = GetComponent<Collider>();  //��ʼ����ײ���
        //ȷ��ObjectPoolʵ������
        if(ObjectPool.Instance==null)
        {
            GameObject obj = new GameObject("ObjectPool");
            obj.AddComponent<ObjectPool>();
        }
        objectPool = ObjectPool.Instance;  //��ʼ�����������

        //��ʼ�������
        InitializeObjectPool();
    }

    /// <summary>
    /// ��ʼ�������
    /// </summary>
    private void InitializeObjectPool()
    {
        if(objectPool==null)
        {
            Debug.LogError("objectPool instance is unll in Spawner.InitializeObjectPool");
            return;
        }
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

    /// <summary>
    /// ���ɶ����Э�̷��������ϻ껷����ˮ����ը������
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
        //�������Ƿ��ڼ���״̬��SetActive��true����
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
