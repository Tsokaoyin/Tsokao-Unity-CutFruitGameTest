using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fruit : MonoBehaviour
{
    public GameObject whole;  //完整的水果
    public GameObject sliced;  //切割后的水果

    private Rigidbody fruitRigidbody;  //水果刚体组件
    private Collider fruitCollider;   //水果碰撞组件
    private ParticleSystem juiceEffect;  //水果破碎特效

    //水果类型和分数
    public enum FruitTpye { Apple, Banana, Orange, Watermelon, Bomb }
    public FruitTpye fruitType = FruitTpye.Apple;
    public int points = 1;  //水果得分

    //切割音效
    public AudioClip sliceSound;

    private void Awake()
    {
        //初始化水果组件
        fruitRigidbody = GetComponent<Rigidbody>();
        fruitCollider = GetComponent<Collider>();
        juiceEffect = GetComponentInChildren<ParticleSystem>();
    }

    private void Slice(Vector3 direction,Vector3 position,float force)
    {
        //增加分数
        GameManager.Instance.AddCoins(points);

        //播放切割音效
        if(sliceSound!=null)
        {
            AudioSource.PlayClipAtPoint(sliceSound, transform.position);
        }

        fruitCollider.enabled = false;  //关闭水果碰撞开关
        whole.SetActive(false);  //关闭整个水果对象

        sliced.SetActive(true);  //打开切割水果对象
        juiceEffect.Play();  //播放切割特效

        //计算切割方向的角度，将弧度转换为角度
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        //设置切割后物体的旋转角度，这里只设置绕Z轴的旋转，X和Y保持0度，让切割后的物体朝向切割方向。
        sliced.transform.rotation = Quaternion.Euler(0f, 0f, angle);

        //获取切割后物体及其所有子物体身上的Rigidbody组件，并将这些组件存储在一个数组中。
        Rigidbody[] slices = sliced.GetComponentsInChildren<Rigidbody>();

        //遍历切割后的每个切片
        foreach(Rigidbody slice in slices)
        {
            //将切片的速度设置为原水果的速度，保持切割前后速度的连贯性
            slice.velocity = fruitRigidbody.velocity;
            //给给个切片一个冲量，冲量的方向有direction决定，大小由force决定，作用点由position决定。
            slice.AddForceAtPosition(direction * force, position, ForceMode.Impulse);
        }

        //延迟回收切片
        StartCoroutine(DelayedReturnToPool());
    }

    private IEnumerator DelayedReturnToPool()
    {
        yield return new WaitForSeconds(2f);

        //重置水果状态
        whole.SetActive(true);
        sliced.SetActive(false);
        fruitCollider.enabled = true;

        //回收到对象池
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

    //重置水果状态
    public void ResetFruit()
    {
        whole.SetActive(true);
        sliced.SetActive(false);
        fruitCollider.enabled = true;
        gameObject.SetActive(true);
    }
}
