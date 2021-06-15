using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.UI;

public class GameController: MonoBehaviour
{
    [SerializeField] public GameObject mainCamera;
    [SerializeField] public GameObject otherCamera;
    private const float CELL_SIZE = 1f;   // セルのサイズ  
    private GameObject camera1;
    public GameObject player;
    public GameObject lave;
    public GameObject gauge;
   // public Button brake;
    public GameObject brake;


    public CameraShake shake;
    AudioSource audioSource;  // ここコピペ

    public static int[] stage_x;
    public static int[] stage_y;

    public List<AudioClip> audioClip = new List<AudioClip>();  // ここコピペ
    public AudioMixer mix;
    int Height = 14;    //横
    int Width = 9;     //縦
    int green = 40;
    int yellow = 20;
    int red = 20;
    int music_cnt = 0;
    int music_change = 1;
    int music_plus = 3;
    int food_limit = 5;
    int score =0;
    float music_time = 0f;
    float original_volume;
    bool can;
    bool push = false;
    bool brake_on = false;
    bool col = false;
    bool music_is_stop = false;
    bool music_fadeout = false;
    bool music_start = false;
    public GameObject cellPrefab;         // セルのプレハブ  
    public GameObject greenwallPrefab;         // 壁のプレハブ  
    public GameObject yellowwallPrefab;         // 壁のプレハブ  
    public GameObject redwallPrefab;         // 壁のプレハブ  
    public UnityEngine.UI.Text heightLabel;
    public UnityEngine.UI.Text speedLabel;
    public UnityEngine.UI.Text scoreLabel;

    public UnityEngine.UI.Button button;
    public int head_x = 0;
    public int head_z = 0;
    public int enemy_x = 5;
    public int enemy_z = 3;
    int cnt = 0;//食べた個数
    int cnt_max = 1;//何個食べたらステージが上昇するか
    int cnt_block = 5;//blockの数
    private Cube[,] cells;                // グリッド状のセル  
    private Cube cell;
    float time = 0.0f;
    float enemy_time = 0.0f;

    public int direction = 5;//最初の方向
    Vector3 speed;
    public int length = 1;   //体の長さ
    float span = 0.4f;  //更新速度
    float enemy_span = 1f;  //更新速度

    float start_span = 0.4f;  //更新速度
    Vector3 speedmeter;
    Vector3 latestPos;
    public AudioMixerSnapshot fast;
    public AudioMixerSnapshot slow;
    // Use this for initialization  
    

    private void Awake()
    {
        GetComponent<AudioSource>().Stop();
    }
    void Start()
    {
        enemy_x = 5;
        enemy_z = 3;
        
        this.gauge = GameObject.Find("gauge");
        //this.brake = GameObject.Find("Canvas/brake").GetComponent<Button>();
        this.brake = GameObject.Find("Canvas/brake");

        start_span = span;

        // グリッド状にセルを作成  
        cells = new Cube[Height, Width];

        for (int x = 0; x < Height; x++)
        {
            for (int z = 0; z < Width; z++)
            {
                // セルを作成  
                GameObject obj = Instantiate(cellPrefab) as GameObject;
                obj.transform.SetParent(transform);

                // 位置を設定  
                float xPos = (x - Height * 0.5f) * CELL_SIZE;
                float zPos = (z - Width * 0.5f) * CELL_SIZE;
                obj.transform.localPosition = new Vector3(xPos, (red+yellow+3)*10f, zPos);

                // Cellをセット  
                cells[x, z] = obj.GetComponent<Cube>();
                //cells[x, z].Upstage(false);
            }
        }
        //shake.Upstage(false);

        cell = cells[head_x, head_z];
        cell.become_head();
        cell = cells[enemy_x, enemy_z];
        cell.become_enemy();


        initial_block();
        feed(Height, Width);
        //blocking(Height, Width, cnt_block);
        cnt = 0;
        //Debug.Log(cnt_max);

        for(int i = 0; i < red; i++)
        {
            GameObject obj = Instantiate(redwallPrefab) as GameObject;
            obj.transform.SetParent(transform);

            int yPoz = i*10 - 45;
            obj.transform.localPosition = new Vector3(-3f,yPoz, -2);

        }
        for (int i = 0; i < yellow; i++)
        {
            GameObject obj = Instantiate(yellowwallPrefab) as GameObject;
            obj.transform.SetParent(transform);

            int yPoz = i * 10 - 45+red*10;
            obj.transform.localPosition = new Vector3(-3f, yPoz, -2);

        }
        for (int i = 0; i < green; i++)
        {
            GameObject obj = Instantiate(greenwallPrefab) as GameObject;
            obj.transform.SetParent(transform);

            int yPoz = i * 10 - 45 + red * 10+yellow*10;
            obj.transform.localPosition = new Vector3(-3f, yPoz, -2);

        }
        camera1 = Camera.main.gameObject;

        Transform trans = camera1.transform;
        Vector3 pos1 = trans.position;
        pos1.y = (red+yellow)*10+43;    // カメラ初期座標
        camera1.transform.position = pos1;
        float cnt_height = camera1.transform.position.y - lave.transform.position.y;
        heightLabel.text = cnt_height.ToString();
        audioSource = gameObject.AddComponent<AudioSource>();  // ここコピペ
        //GetComponent<AudioSource>().Pause();
        original_volume = GetComponent<AudioSource>().volume;
        var button = brake.GetComponent<Button>();
        var colors = button.colors;
        colors.normalColor = new Color(125f / 255f, 125f / 255f, 115f / 255f, 255f / 255f);
        button.colors = colors;
        //brake.interactable = false;
        //brake.SetActive (false);
        this.gauge.GetComponent<Image>().fillAmount=0f;
    }
    private void Update()
    {
       

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            direction = 0;

        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            direction = 1;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            direction = 2;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {

            direction = 3;
        }
        if (direction >= 4)//入力があるまで停止
        {
            return;
        }
        Transform trans = camera1.transform;
        Vector3 pos1 = trans.position;
        pos1.y = cells[0, 0].transform.position.y+13;
        camera1.transform.position = pos1;
        if (push)
        {
            Debug.Log("clicked");
            feed(Height, Width);
            push = false;
            brake_on = true;
            span = start_span;
            for (int x = 0; x < Height; x++)
            {
                for (int z = 0; z < Width; z++)
                {
                    cells[x, z].OnBrake();
                    shake.OnBrake();

                }
            }
            this.gauge.GetComponent<Image>().fillAmount =0;
            var button = brake.GetComponent<Button>();
            var colors = button.colors;
            colors.normalColor = new Color(136f / 255f, 22f / 106f, 19f / 255f, 25f / 255f);
            button.colors = colors;
            //brake.interactable = false;
            //brake.SetActive(true);
            GameObject.Find("Canvas").transform.Find("brake").gameObject.SetActive(false);
            cnt = 0;
        }
    }
    
    
    void FixedUpdate()
    {
       
        if (direction >= 4)//入力があるまで停止
        {
            return;
        }
        if (music_cnt <= 0 && music_is_stop == false)
        {
            if (music_start == false)
            {
                music_start = true;
                GetComponent<AudioSource>().Play();

            }
            if (music_fadeout == false)
            {
                original_volume = GetComponent<AudioSource>().volume;
                music_fadeout = true;
            }
            
            
            if (GetComponent<AudioSource>().volume <= 0)
            {
                GetComponent<AudioSource>().Pause();
                music_is_stop = true;
                music_fadeout = false;
                GetComponent<AudioSource>().volume= original_volume;
                //Debug.Log("v");

            }
        }
        if (music_fadeout == true)
        {
            
            GetComponent<AudioSource>().volume -= 0.01f;
            //Debug.Log("a");
            if (music_cnt > 0){
                GetComponent<AudioSource>().volume = original_volume;
                music_fadeout = false;

            }
        }

        if (music_cnt > 0 && music_is_stop ==true)
        {
            GetComponent<AudioSource>().UnPause();
            music_is_stop = false;
        }
        if (music_cnt > music_change)
        {
            /*
            mix.TransitionToSnapshots(
             new UnityEngine.Audio.AudioMixerSnapshot[] { slow, fast },
             new float[] { 0, 1 },0);
             */
            //GetComponent<AudioSource>().volume /= 0.5f;
            fast.TransitionTo(0);
            //GetComponent<AudioSource>().volume = original_volume;

        }

        else
        {
            /*
            mix.TransitionToSnapshots(
             new UnityEngine.Audio.AudioMixerSnapshot[] { fast, slow },
             new float[] { 0, 1 }, 0);
             */
            //GetComponent<AudioSource>().volume /= 0.5f;
            slow.TransitionTo(0);
            //GetComponent<AudioSource>().volume = original_volume;

        }
        //Debug.Log(music_cnt);

        cell = cells[head_x, head_z];
        /*
        if (cnt >= cnt_max)
        {
          
            StartCoroutine(UpDown(true));
            //cnt_max++;
            cnt = 0;
            Debug.Log(cnt_max);
        }
        */
        float cnt_height = camera1.transform.position.y - lave.transform.position.y-13;
        //int cnt1 = cnt_max - cnt;
        //scoreLabel.text = cnt1.ToString();

        heightLabel.text = cnt_height.ToString();

        Vector3 pos1 = cells[0, 0].transform.position;
        speed = ((pos1 - latestPos) / Time.deltaTime);
        latestPos = pos1;

        speedLabel.text = speed.y.ToString();


        //Gameover判定
        Transform myTransform6 = cells[0,0].transform;
        Vector3 pos6 = myTransform6.position;
        Transform Translave = lave.transform;
        Vector3 pos5 = Translave.position;
        if (pos5.y > pos6.y)
        {
            GoToGameOver();
        }

        time = time + Time.deltaTime;
        if (time >= span)
        {
            
            {
                StartCoroutine(cell.become_tail(length*span));
                if (direction == 1)
                {
                    try
                    {
                        cell = cells[head_x, head_z + 1];
                        head_z++;
                    }
                    catch
                    {
                        col = true;
                    }
                }
                else if (direction == 3)
                {
                    try
                    {
                        cell = cells[head_x + 1, head_z];
                        head_x++;
                    }
                    catch
                    {
                        col = true;

                    }
                }
                else if (direction == 0)
                {
                    try
                    {
                        cell = cells[head_x - 1, head_z];
                        head_x--;
                    }
                    catch
                    {
                        col = true;
                    }
                }
                else if (direction == 2)
                {
                    try
                    {
                        cell = cells[head_x, head_z - 1];
                        head_z--;
                    }
                    catch
                    {
                        col = true;
                    }
                }
                can = cell.become_head();
               

                if (col == true)
                {
                    StartCoroutine(UpDown(false));
                    //length--;
                    for (int x = 0; x < Height; x++)
                    {
                        for (int z = 0; z < Width; z++)
                        {
                            cells[x, z].reset();
                            if (cells[x, z].head.activeSelf == true)
                            {
                                cells[x, z].normal.SetActive(false);
                            }

                        }
                    }
                    col = false;
                }
                if (length != cell.check_eat(length))
                {
                    length++;
                    cnt++;
                    feed(Height, Width);
                    //audioSource.PlayOneShot(audioClip[0]);  // ここコピペ
                    music_cnt += music_plus;
                    score += length;
                    scoreLabel.text = score.ToString();
                    this.gauge.GetComponent<Image>().fillAmount += 1f/food_limit;
                    StartCoroutine(UpDown(true));

                    if (food_limit <= cnt)
                    {
                        var button = brake.GetComponent<Button>();
                        var colors = button.colors;
                        colors.normalColor = new Color(136f / 255f, 255f / 255f, 102f / 255f, 255f / 255f);
                        button.colors = colors;
                        //brake.interactable = true;
                       // brake.SetActive(true);
                        GameObject.Find("Canvas").transform.Find("brake").gameObject.SetActive(true);

                    }
                }
                if (cell.check_block() == true)
                {
                    StartCoroutine(UpDown(false));
                    blocking(Height, Width, 1);
                    span *= 0.96f;
                }
                if (can == true)
                {
                    //UpDown(true);
                    StartCoroutine(UpDown(false));

                }
                if (cell.check_enemy() == true)
                {
                    for (int x = 0; x < Height; x++)
                    {
                        for (int z = 0; z < Width; z++)
                        {
                            cells[x, z].up = 3;


                        }
                    }
                }
            }
            time = 0f;
            if (length < 0)
            {
                length = 0;
            }
            if (brake_on == true&&speed.y>=-0.5)
            {
                for (int x = 0; x < Height; x++)
                {
                    for (int z = 0; z < Width; z++)
                    {
                        cells[x, z].reset();

                    }
                }
                brake_on = false;
                length = 0;
            }
            if (span < 0.5f)
            {
              //  span +=0.002f;      //時間経過で減速
            }
            music_time += Time.deltaTime;
            if (music_time >= 0.028f && music_cnt>0)
            {
                music_cnt = music_cnt-1;
                music_time = 0f;
            }
        }
        enemy_time = enemy_time + 0.01f;

        if (enemy_span < enemy_time)
        {
            enemy_time = 0;
            Debug.Log(enemy_x);
            Debug.Log(enemy_z);

            for (int i = 0; i < 1; i++)
            {
                double ran = Random.value;
                if (ran<=0.25f)
                {
                    try
                    {
                        cell = cells[enemy_x, enemy_z + 1];

                        if (cell.become_enemy() == true)
                        {
                             cell = cells[enemy_x, enemy_z ];      
                            cell.quit_enemy();

                            enemy_z++;
                            break;
                        }
                       
                    }
                    catch
                    {
                        
                    }
                }
                else if (0.25f<ran &&ran<= 0.5f)
                {
                    try
                    {
                        cell = cells[enemy_x + 1, enemy_z];
                        cell.quit_enemy();

                        if (cell.become_enemy() == true)
                        {
                           cell = cells[enemy_x , enemy_z];
                            cell.quit_enemy();
                            enemy_x++;
                        }
                        
                    }
                    catch
                    {
                       
                    }
                }
                else if (0.5f < ran && ran <= 0.75f)
                {
                    try
                    {
                        cell = cells[enemy_x - 1, enemy_z];
                        cell.quit_enemy();

                        if (cell.become_enemy() == true)
                        {
                            cell = cells[enemy_x , enemy_z];
                            cell.quit_enemy();
                            enemy_x--;
                           
                        }
                       
                    }
                    catch
                    {
                       
                    }
                }
                else if (0.75f<ran)
                {
                    try
                    {
                        cell = cells[enemy_x, enemy_z - 1];
                        cell.quit_enemy();

                        if (cell.become_enemy() == true)
                        {
                            cell = cells[enemy_x, enemy_z ];
                            enemy_z--;
                            cell.quit_enemy();
                        }
                        
                    }
                    catch
                    {
                       
                    }
                }
               
            }
        }
       
    }
   
    public void Set_cells(int Height,int Width,int cnt_max)
    {
        cells = new Cube[Height, Width];
        int head_x = Random.Range(1, Height);
        int head_z = Random.Range(1, Width);
        for (int x = 0; x < Height; x++)
        {
            for (int z = 0; z < Width; z++)
            {
                // セルを作成  
                GameObject obj = Instantiate(cellPrefab) as GameObject;
                obj.transform.SetParent(transform);

                // 位置を設定  
                float xPos = (x - Height * 0.5f) * CELL_SIZE;
                float zPos = (z - Width * 0.5f) * CELL_SIZE;
                obj.transform.localPosition = new Vector3(xPos, 0f, zPos);

                // Cellをセット  
                cells[x, z] = obj.GetComponent<Cube>();
            }
        }
        cell = cells[head_x, head_z];
        cell.become_head();
        feed(Height, Width);
        blocking(Height, Width, cnt_max);
        
    }

    public void GoToGameOver()
    {
        SceneManager.LoadScene("GameOver");
    }

    public void feed(int max_x, int max_y)
    {
        int miss_cnt = 0;
        while (true){
            int set_x = Random.Range(0, max_x);
            int set_y = Random.Range(0, max_y);
            try
            {
                Cube cell = cells[set_x, set_y];
                if (cell.food.activeSelf == false && cell.tail.activeSelf == false && cell.head.activeSelf == false&&cell.block.activeSelf==false&& cell.enemy.activeSelf == false)
                {
                    cell.set_food();
                    
                    break;
                }
            }
            catch { }
            miss_cnt++;
            if (miss_cnt > 10000)
            {
                break;
            }
           
        }
    }
    public void blocking(int max_x, int max_y,int cnt)
    {
        for (int i = 0; i < cnt; i++)
        {
            int miss_cnt = 0;

            while (true)
            {
                int set_x = Random.Range(0, max_x);
                int set_y = Random.Range(0, max_y);
                try
                {
                    Cube cell = cells[set_x, set_y];
                    if (cell.food.activeSelf == false && cell.tail.activeSelf == false && cell.head.activeSelf == false &&cell.block.activeSelf==false)
                    {
                        cell.set_block();
                        break;

                    }
                }
                catch { }
                miss_cnt++;
                if (miss_cnt > 10000)
                {
                    break;
                }
            }
            
        }
    }

    public void initial_block()
    {
        int num_stage = 0;
        int loop_cnt = 0;
        int[] num_block_x;
        int[] num_block_y;

        num_stage = PlayerPrefs.GetInt("STAGE", 0);
        if (num_stage == 1)
        {
            loop_cnt = 4;
            //num_block_x =new int[]{ 1,1,7,7};
            //num_block_y = new int[]{ 1, 12, 1, 12};
            num_block_x = new int[] { 1, 1, 12, 12 };
            num_block_y = new int[] { 1, 7, 7, 1 };
        }
        else if (num_stage == 2)
        {
            loop_cnt = 8;
            //num_block_x =new int[]{ 1,1,7,7};
            //num_block_y = new int[]{ 1, 12, 1, 12};
            num_block_x = new int[] { 5, 6, 6, 6,7,7,7,8 };
            num_block_y = new int[] { 4, 3, 4, 5,3,4,5,4 };
        }
        else if (num_stage == 3)
        {
            loop_cnt = 12;
            //num_block_x =new int[]{ 1,1,7,7};
            //num_block_y = new int[]{ 1, 12, 1, 12};
            num_block_x = new int[] { 4, 4, 4, 4, 4, 4, 9, 9,9,9,9,9 };
            num_block_y = new int[] { 1,2,3,5,6,7,1,2,3,5,6,7};
        }
        else
        {
            loop_cnt = 4;
            num_block_x = new int[] { 1, 1, 7, 7 };
            num_block_y = new int[] { 1, 12, 1, 12 };
        }
        for (int i = 0; i < loop_cnt; i++)
        {
            Cube cell = cells[num_block_x[i], num_block_y[i]];
            cell.set_block();
        }

     
    }
    /*
    public void initial_block(int [] x,int [] y)
    {
        int length = x.GetLength(0);
        for(int i=0; i < length; i++)
        {
            int set_x = x[i];
            int set_y = y[i];

            Cube cell = cells[set_x, set_y];
            cell.set_block();
            /*
            if (cell.food.activeSelf == false && cell.tail.activeSelf == false && cell.head.activeSelf == false && cell.block.activeSelf == false)
            {
                cell.set_block();
            }
            
           
        }
             
    }*/
    public void FadeScene()
    {
        FadeManager.Instance.LoadScene("Main", 1.5f);
    }

    IEnumerator UpDown(bool up)
    {
        camera1 = Camera.main.gameObject;

        Transform trans = camera1.transform;
        Vector3 pos1 = trans.position;
        Transform myTransform = mainCamera.transform;
        Vector3 pos = myTransform.position;
        Vector3 posA = myTransform.eulerAngles;
        otherCamera.transform.position = pos;
        otherCamera.transform.eulerAngles = posA;

        yield return new WaitForSeconds(0);
        
      
        for (int x = 0; x < Height; x++)
        {
            for (int z = 0; z < Width; z++)
            {
                if (up == true)
                {
                    cells[x, z].Upstage(true);
                    shake.Upstage(true);
                }
                else
                {
                    cells[x, z].Upstage(false);
                    shake.Upstage(false);
                }

                
            }
        }
    }
    public void PushDown()
    {
        push = true;
    }

    public void PushUp()
    {
        push = false;
    }

}