using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fruit : MonoBehaviour
{
    public GameObject whole;  //������ˮ��
    public GameObject sliced;  //�и���ˮ��

    private Rigidbody fruitRigidbody;  //ˮ���������
    private Collider fruitCollider;   //ˮ����ײ���
    private ParticleSystem juiceEffect;  //ˮ��������Ч

    //ˮ�����ͺͷ���
    public enum FruitTpye { Apple, Banana, Orange, Watermelon, Bomb }
    public FruitTpye fruitType = FruitTpye.Apple;
    public int points = 1;  //ˮ���÷�

    //�и���Ч
    public AudioClip sliceSound;

    private void Awake()
    {
        //��ʼ��ˮ�����
        fruitRigidbody = GetComponent<Rigidbody>();
        fruitCollider = GetComponent<Collider>();
        juiceEffect = GetComponentInChildren<ParticleSystem>();
    }

    private void Slice(Vector3 direction,Vector3 position,float force)
    {
        //���ӷ���
        GameManager.Instance.AddCoins(points);

        //�����и���Ч
        if(sliceSound!=null)
        {
            AudioSource.PlayClipAtPoint(sliceSound, transform.position);
        }

        fruitCollider.enabled = false;  //�ر�ˮ����ײ����
        whole.SetActive(false);  //�ر�����ˮ������

        sliced.SetActive(true);  //���и�ˮ������
        juiceEffect.Play();  //�����и���Ч

        //�����и��ĽǶȣ�������ת��Ϊ�Ƕ�
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        //�����и���������ת�Ƕȣ�����ֻ������Z�����ת��X��Y����0�ȣ����и������峯���и��
        sliced.transform.rotation = Quaternion.Euler(0f, 0f, angle);

        //��ȡ�и�����弰���������������ϵ�Rigidbody�����������Щ����洢��һ�������С�
        Rigidbody[] slices = sliced.GetComponentsInChildren<Rigidbody>();

        //�����и���ÿ����Ƭ
        foreach(Rigidbody slice in slices)
        {
            //����Ƭ���ٶ�����Ϊԭˮ�����ٶȣ������и�ǰ���ٶȵ�������
            slice.velocity = fruitRigidbody.velocity;
            //��������Ƭһ�������������ķ�����direction��������С��force���������õ���position������
            slice.AddForceAtPosition(direction * force, position, ForceMode.Impulse);
        }

        //�ӳٻ�����Ƭ
        StartCoroutine(DelayedReturnToPool());
    }

    private IEnumerator DelayedReturnToPool()
    {
        yield return new WaitForSeconds(2f);

        //����ˮ��״̬
        whole.SetActive(true);
        sliced.SetActive(false);
        fruitCollider.enabled = true;

        //���յ������
        ObjectPool.Instance.ReturnToPool(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            Blade blade = other.GetComponent<Blade>();
            Slice(blade.direction, blade.transform.position, blade.sliceForce);
        }
    }

    //����ˮ��״̬
    public void ResetFruit()
    {
        whole.SetActive(true);
        sliced.SetActive(false);
        fruitCollider.enabled = true;
        gameObject.SetActive(true);
    }
}
