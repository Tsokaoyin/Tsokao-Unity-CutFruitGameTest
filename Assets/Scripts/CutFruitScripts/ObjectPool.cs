using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance { get; private set; }

    /*
     * ���Ͷ��壺Dictionary<GameObject,Queue<GameObject>>��������һ���ֵ䣬����Key����GameObject���ͣ�ͨ��
     * ����Ԥ���壨Prefab����ֵ��Value����Queue<GameObject>���ͣ�����Ϸ����Ķ��С�������ѭ�Ƚ��ȳ���FIFO��ԭ��
     * ������objectPools���ڴ洢��ͬԤ�����Ӧ�Ķ���ء�ÿ��Ԥ�����Ӧһ�����У�
     * �����д�Ÿ�Ԥ����ʵ���������Ϸ����
     * ��ʼ����new Dictionary<GameObject,Queue<GameObject>>()����������ʱ������һ���µ��ֵ�ʵ����
     */
    private Dictionary<GameObject, Queue<GameObject>> objectPools = new Dictionary<GameObject, Queue<GameObject>>();
    /*
     * ���Ͷ��壺Dictionary<GameObject,GameObject>Ҳ��һ���ֵ䣬����ֵ����GameObject���͡�һ�������ʵ���������Ϸ����
     * ֵ�������Ϸ�����Ӧ��Ԥ���塣
     * ��������prefabLookup����ͨ��ʵ��������Ϸ������ٲ�������Ӧ��Ԥ���塣
     * ��ʼ����new Dictionary<GameObject,GameObject>()������������ͬʱ������һ���µ��ֵ�ʵ����
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
    /// ���������
    /// </summary>
    /// <param name="prefab">��Ҫ�������ص�Ԥ����</param>
    /// <param name="poolSize">����صĳ�ʼ��С</param>
    public void CreatePool(GameObject prefab,int poolSize)
    {
        //���objectPools�ֵ����Ƿ��Ѿ����ڸ�Ԥ�����Ӧ�Ķ����
        //ContainsKey():����Dictionary���һ�������������Ǽ���ֵ����Ƿ����ָ������
        //prefab�Ǵ����ָ��������ͨ������һ��Ԥ������Ϸ������objectPools�ֵ��������prefabΪ���Ķ���
        //�÷����᷵��true����֮�򷵻�false��
        if(!objectPools.ContainsKey(prefab))
        {
            //��������ڣ���Ϊ��Ԥ�������һ���µĶ���أ������ʹ�ö������洢��Ϸ����
            objectPools.Add(prefab, new Queue<GameObject>());

            //ѭ��poolSize�Σ�����ָ����������Ϸ������ӵ��������
            for(int i=0;i<poolSize;i++)
            {
                //ʹ��Instantiate����ʵ����Ԥ���壬����һ���µ���Ϸ����
                GameObject obj = Instantiate(prefab);
                //���´�������Ϸ��������Ϊ�Ǽ���״̬���������ڳ����оͲ�����ʾ��ִ���߼�
                obj.SetActive(false);
                //���´�������Ϸ������ӵ���Ԥ�����Ӧ�Ķ���ض����С�
                objectPools[prefab].Enqueue(obj);

                //��ʵ��������Ϸ����Ͷ�Ӧ��Ԥ������ӵ� prefabLookup �ֵ��У������������
                prefabLookup.Add(obj, prefab);
            }
        }
    }

    /// <summary>
    /// �Ӷ���ػ�ȡ����
    /// </summary>
    /// <param name="prefab">Ҫ��ȡ�Ķ����Ԥ����ģ��</param>
    /// <param name="position">����ʵ�������λ��</param>
    /// <param name="rotation">����ʵ���������ת�Ƕ�</param>
    /// <returns></returns>
    public GameObject GetFromPool(GameObject prefab,Vector3 position,Quaternion rotation)
    {
        //��������Ƿ����,���ֵ����Ѵ��ڸ�Ԥ����ĳأ�������������Ƿ��п��ö���
        if (objectPools.ContainsKey(prefab))
        {
            //�ӳ��л�ȡ���ö���
            if (objectPools[prefab].Count>0)
            {
                //�Ӷ�����ȡ��һ������Dequeue()����������λ�ú���ת��
                GameObject obj = objectPools[prefab].Dequeue();
                obj.transform.position = position;
                obj.transform.rotation = rotation;
                //�������SetActive(true)����ʹ�����²�����Ϸ�߼���
                obj.SetActive(true);

                //���ö���
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
                //����û�п��ö��󣬴����¶���
                GameObject obj = Instantiate(prefab, position, rotation);
                prefabLookup.Add(obj, prefab);
                return obj;
            }
        }
        else
        {
            //����ز����ڣ������³�
            CreatePool(prefab, 3);
            return GetFromPool(prefab, position, rotation);
        }
    }

    /// <summary>
    /// ���ض����
    /// </summary>
    /// <param name="obj">�����صĶ���</param>
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
            //��������ٳ��У�������
            Destroy(obj);
        }
    }

    //�����ж�����յ������
    public void ReturnAllToPool()
    {
        //��������ˮ����ը��
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
