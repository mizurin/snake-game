using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
public class Cube : MonoBehaviour {
    public GameObject normal;
    public GameObject head;
    public GameObject tail;
    public GameObject food;
    public GameObject block;
    public GameObject enemy;
    public GameObject tff_Heal_1_oneShot;
    public Rigidbody rb;
    GameObject instantiateEffect;
    public int cnt = 0;
    Vector3 latestPos;
    Vector3 offset;
    //　出現させるエフェクト
    [SerializeField] private GameObject effectObject;
    //　エフェクトを消す秒数
    [SerializeField] private float deleteTime;
    private float endTime=0f;
    //public int length = 0;
    public bool IS_normal { get; private set; }
    public bool IS_head { get; private set; }
    public bool IS_tail { get; private set; }
    public bool IS_food { get; private set; }
    public bool IS_block { get; private set; }
    bool eat = false;
    bool eat1 = false;

    public int up = 0;
    float add = 1.0f;
    bool is_brake=false;
    IEnumerator tail_length;
    
    // Use this for initialization
    void Awake()
    {
       
        normal = transform.Find("normal").gameObject;
        head = transform.Find("head").gameObject;
        tail = transform.Find("tail").gameObject;
        food = transform.Find("food").gameObject;
        block = transform.Find("block").gameObject;
        enemy = transform.Find("enemy").gameObject;

        normal.SetActive(true);
        head.SetActive(false);
        tail.SetActive(false);
        food.SetActive(false);
        block.SetActive(false);
        enemy.SetActive(false);


    }
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        tail_length = become_tail(0);
        latestPos = rb.transform.position;
    }
    // Update is called once per frame
    void Update () {
        IS_normal = normal.activeSelf;
        IS_head= head.activeSelf;
        IS_tail = tail.activeSelf;
        IS_food = food.activeSelf;
        IS_block = block.activeSelf;

    }
    public int check_eat(int length)
    {
        if (food.activeSelf == true)
        {
            food.SetActive(false);
            tail.SetActive(true);
            
            length++;
            eat = true;
            eat1 = true;
            
            //　ゲームオブジェクト登場時にエフェクトをインスタンス化
            //var instantiateEffect = GameObject.Instantiate(effectObject, transform.position + new Vector3(0f, 4f, 0f), Quaternion.identity) as GameObject;
            //Destroy(instantiateEffect, deleteTime);
            //var instantiateEffect = GameObject.Instantiate(tff_Heal_1_oneShot, transform.position + offset, Quaternion.identity) as GameObject;
            // GameObject obj = Instantiate(tff_Heal_1_oneShot) as GameObject;
            //obj.transform.SetParent(transform);
        }
        return length;
    }
    public bool check_block()
    {
        if(head.activeSelf==true && block.activeSelf == true)
        {
            //normal.SetActive(true);
            block.SetActive(false);

            return true;
        }
        else
        {
            return false;
        }
    }
    public void set_food()
    {
        normal.SetActive(false);
        food.SetActive(true);
    }
    
    public void set_block()
    {
        normal.SetActive(false);
        block.SetActive(true);
    }
    public bool become_enemy()
    {
        
        if (food.activeSelf == false && tail.activeSelf == false && block.activeSelf == false&& head.activeSelf == false)
        {
            normal.SetActive(false);
            enemy.SetActive(true);
            return true;
        }
        else
        {
            return false;
        }
    }
    public void quit_enemy()
    {
        if (tail.activeSelf == false &&block.activeSelf==false)
        {
            normal.SetActive(true);
        }
       
        enemy.SetActive(false);
    }
    public bool check_enemy()
    {

        if (enemy.activeSelf == true)
        {
            //enemy.SetActive(false);
            return true;
        }
        else
        {
            return false;
        }
    }
    public bool become_head()
    {
        if (head.activeSelf == false)
        {
            normal.SetActive(false);
            head.SetActive(true);
            if (tail.activeSelf == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
        
    }

    public IEnumerator become_tail(float length)
    {
        tail.SetActive(true);
        head.SetActive(false);
        normal.SetActive(false);
        yield return new WaitForSeconds(length);
        if (head.activeSelf == false&&food.activeSelf==false)
        {
            tail.SetActive(false);
            normal.SetActive(true);
        }
        
    }
    public void OnBrake()
    {
        // コルーチンをIEnumeratorの位置から開始（再開）
        // StartCoroutine(tail_length);
        is_brake = true;
    }

    public void OffBrake()
    {
        // コルーチンを停止
        StopCoroutine(tail_length);
    }


    public void become_normal()
    {       
        if (head.activeSelf == false)
        {
            normal.SetActive(true);
            tail.SetActive(false);
        }
       
    }
    public void reset()
    {
        if (food.activeSelf == false && tail.activeSelf == true && block.activeSelf == false)
        {
            normal.SetActive(true);
            tail.SetActive(false);
        }

    }

    public void Upstage(bool tmp)
    {
        if(tmp == true)
        {
            up = 1;//up
        }
        else
        {
            up = 2;//down
        }
        
    }
    public void GoToGameOver()
    {
        SceneManager.LoadScene("GameOver");
    }
    void FixedUpdate()
    {

        if (up == 0)
        {

        }
        else if (up == 1)
        {
            //rb.AddForce(Vector3.up * 5f, ForceMode.Impulse);
            rb.AddForce(Vector3.up * 70f, ForceMode.Force);

            up = 0;
        }
        else if (up == 2)
        {
            //rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
            rb.AddForce(Vector3.down * 200f, ForceMode.Force);

            up = 0;
        }
        else if (up == 3)
        {
            //rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
            rb.AddForce(Vector3.down * 250f, ForceMode.Force);

            up = 0;
        }

        if (eat1 == true)
        {
            if (eat == true)
            {
                instantiateEffect = GameObject.Instantiate(tff_Heal_1_oneShot, offset, Quaternion.identity) as GameObject;
                Destroy(instantiateEffect, deleteTime);
                eat = false;
            }
            endTime += Time.deltaTime;
            

            /*
            var instantiateEffect = GameObject.Instantiate(tff_Heal_1_oneShot, offset, Quaternion.identity) as GameObject;
            Destroy(instantiateEffect, deleteTime);
            endTime += Time.deltaTime;
            */
            if (endTime <= 2f)
            {
                offset = this.transform.position;
                offset.y = this.transform.position.y + 4.5f;
                offset.x = this.transform.position.x - 1f;

                instantiateEffect.transform.position = offset;
            }
            if (endTime >= 2f)
            {
                eat1 = false;
                endTime = 0f;
            }
            
        }

        if (is_brake)
        {
            if (rb.transform.position.y - latestPos.y < 0)
            {
                rb.AddForce(Vector3.up * -(this.transform.position.y - latestPos.y) * 1f * add, ForceMode.Force);
                
                add = add * 1.1f;
                // Debug.Log(this.transform.position.y - latestPos.y);
                //Debug.Log("brake_on");
            }
            else
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                //rb.AddForce(Vector3.up * 50f, ForceMode.Force);
                is_brake = false;
                add = 1f;
               // Debug.Log("brake_off");
               // Debug.Log(is_brake);


            }
        }
        latestPos = this.transform.position;

    }
}
