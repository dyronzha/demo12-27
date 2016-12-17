using UnityEngine;
using System.Collections;

public class C_Player : MonoBehaviour {

    //摩擦力變數
    private BoxCollider2D player_box;
    private BoxCollider2D wall_box;
    PhysicsMaterial2D wall_material;
    PhysicsMaterial2D player_material;
    //存檔變數
    public bool b_is_save = false;


    //技能相關變數宣告
    public GameObject O_mirror = null;
   public GameObject O_virtualplayer = null;
    public bool b_magic = false;
    public bool b_upside = false;
    private bool b_use_skill = false;
    public float f_shoot = 0f;
    GameObject O_tempmirror;
    GameObject O_tempvirtuall;

    //玩家物件相關變數
    public GameObject O_bullet = null;
    Rigidbody2D player_rig = null;
    Animator player_animator = null;
    public bool b_isground = true;
    private Transform t_ground_check;
    private Transform t_pic;
    private Collider2D player_coll;
    public GameObject O_dieline = null;
    public int i_hp;
    protected C_UIHP HP_ui;
    public string s_name = "player";

    //玩家運動變數
    private float f_speed = 0.0f;
    private bool b_jump = false;
    private float f_jump_speed = 0.0f;
    Vector3 last_position_vec3;
    Vector2 jump_vec2;
    public Vector3 between_cilling_vec3;
    public Vector3 between_virtuall_vec3;
    bool b_airmove = false;
    public LayerMask mask_layer;

    //重生變數
    public bool b_die = false;
    private Vector3 respawn_position_vec3;
    private float f_dietime = 0;



    public GameObject O_wall;

    // Use this for initialization
    void Awake()
    {
        b_die = false;
        f_jump_speed = 7.0f;
        f_speed = 8.0f;
        player_rig = GetComponent<Rigidbody2D>();
        t_ground_check = transform.Find("Groundcheck");
        t_pic = transform.Find("pic");
        player_animator = transform.GetChild(0).GetComponent<Animator>();
        jump_vec2 = new Vector2(0, f_jump_speed);
        player_coll = GetComponent<Collider2D>();
        respawn_position_vec3 = transform.position;
        b_jump = true;
        i_hp = 3;
        HP_ui = GameObject.Find("UI_HP").GetComponent<C_UIHP>();
        player_box = gameObject.GetComponent<BoxCollider2D>();
        player_material = player_box.sharedMaterial;
    }

    void Start()
    {
        //開始前都就先讀檔
        //給ui顯示現在的血量
        HP_ui.PresentHp(ref i_hp);

    }

    // Update is called once per frame
    void FixedUpdate()
    {

        if (!b_die)  //沒死
        {
            IsDie();    //判斷是生是死
            //Through(); //穿牆技能
            //TeleportLeft();  //左右瞬移技能
            Move();  //基本移動
            last_position_vec3 = transform.position; //記下最後位置
        }
        else//死了
        {
            transform.position = last_position_vec3;
            PlayerRespawn();//重生
        }


        // 紀錄現在位置，以判定下次是否移動

    }
    void Update()
    {
        Teleport(); //上下瞬移
        b_isground = Physics2D.Linecast(transform.position, t_ground_check.position, 1 << LayerMask.NameToLayer("ground"));
        //判斷在地上
        if (!b_die)  //沒死
        {
            if (Input.GetKey(KeyCode.W) && b_isground)//&& !b_magic
            {
                b_jump = true;
                JumpAct();
            }
            //射擊
           ShootAct();
            //射擊間格時間
            if (f_shoot < 3) f_shoot += Time.deltaTime;
        }
    }


    void Teleport()
    {
        //顛倒判定射線
        RaycastHit2D hit_cilling_ray = Physics2D.Raycast(transform.position, Vector2.up, 8.5f, mask_layer);
        RaycastHit2D hit_ground_ray = Physics2D.Raycast(transform.position, Vector2.up, -8.5f, mask_layer);
        Debug.DrawLine(transform.position, transform.position + (Vector3)Vector2.up * 8.5f);
        Debug.DrawLine(transform.position, transform.position + (Vector3)Vector2.up * -8.5f, Color.red);
        //碰到天花板並正向
        if (hit_cilling_ray && !b_upside)
        {
            //紀錄鏡子和虛像與玩家的距離
            between_cilling_vec3 = new Vector3(transform.position.x, (transform.position.y + hit_cilling_ray.point.y) / 2+0.5f , transform.position.z);
            between_virtuall_vec3 = new Vector3(transform.position.x, hit_cilling_ray.point.y - 0.5f, transform.position.z);

            //按鍵後產生鏡子和虛像，並紀錄用過技能
            if (Input.GetKeyDown(KeyCode.K) && !b_magic && b_isground)
            {
                O_tempmirror = Instantiate(O_mirror, between_cilling_vec3, Quaternion.identity) as GameObject;
                O_tempvirtuall = Instantiate(O_virtualplayer, between_virtuall_vec3, Quaternion.Euler(180, 0, 0)) as GameObject;
                b_magic = true;
            }

            //放開鍵瞬間移動，改變重力方向，紀錄為顛倒，技能初始
            else if (Input.GetKeyUp(KeyCode.K) && b_magic && b_isground)
            {
                transform.localScale = new Vector3(0.7f, -0.7f, 1f);
                transform.position = between_virtuall_vec3;
                player_rig.gravityScale = -3.0f;
                b_magic = false;
                b_upside = true;
            }
        }

        //顛倒後
        else if (hit_ground_ray && b_upside)
        {
            between_cilling_vec3 = new Vector3(transform.position.x, (transform.position.y + hit_ground_ray.point.y) / 2-0.5f , transform.position.z);
            between_virtuall_vec3 = new Vector3(transform.position.x, hit_ground_ray.point.y + 0.5f, transform.position.z);

            if (Input.GetKeyDown(KeyCode.K) && !b_magic && b_isground)
            {
                O_tempmirror = Instantiate(O_mirror, between_cilling_vec3, Quaternion.identity) as GameObject;
                O_tempvirtuall = Instantiate(O_virtualplayer, between_virtuall_vec3, Quaternion.identity) as GameObject;
                b_magic = true;
            }

            //瞬間移動
            else if (Input.GetKeyUp(KeyCode.K) && b_magic && b_isground)
            {
                transform.localScale = new Vector3(0.7f, 0.7f, 1f);
                transform.position = between_virtuall_vec3;
                player_rig.gravityScale = 3.0f;
                b_magic = false;
                b_upside = false;
            }
        }
        else if ((!hit_cilling_ray || !hit_ground_ray) && b_magic)
        {
            Destroy(O_tempmirror, 0f);
            Destroy(O_tempvirtuall, 0f);
            b_magic = false;
        }
    }

    //跳
    void JumpAct()
    {
        if (!b_upside && b_jump)
        {
            player_rig.velocity = new Vector2(player_rig.velocity.x, f_jump_speed);
            b_jump = false;
        }
        else if (b_upside && b_jump)
        {
            player_rig.velocity = new Vector2(player_rig.velocity.x, -f_jump_speed);
            b_jump = false;
        }
    }

    void Move()
    {
        //空中撞到牆速度為0
        if (b_airmove) f_speed = 0;
        else f_speed = 3.5f;
        //橫向移動
        if (!b_upside)
        {
            if (Input.GetKey(KeyCode.A))
            {
                transform.localScale = new Vector3(-0.7f, 0.7f, 0.7f);//轉向用

                player_rig.velocity = new Vector2(-f_speed, player_rig.velocity.y); //速度等於speed
                player_animator.SetBool("walk", true);  //動畫開關

            }
            else if (Input.GetKey(KeyCode.D))
            {
                transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);//轉向用
                player_rig.velocity = new Vector2(f_speed, player_rig.velocity.y);
                player_animator.SetBool("walk", true);
            }
        }

        else if (b_upside)
        {
            if (Input.GetKey(KeyCode.A))
            {
                transform.localScale = new Vector3(-0.7f, -0.7f, 0.7f);//轉向用
                player_rig.velocity = new Vector2(-f_speed, player_rig.velocity.y);
                player_animator.SetBool("walk", true);

            }
            else if (Input.GetKey(KeyCode.D))
            {
                transform.localScale = new Vector3(0.7f, -0.7f, 0.7f);//轉向用
                player_rig.velocity = new Vector2(f_speed, player_rig.velocity.y);
                player_animator.SetBool("walk", true);
            }
        }

        if (!(Input.GetKey(KeyCode.D)) && (!Input.GetKey(KeyCode.A)))
        {
            player_animator.SetBool("walk", false);
        }
    }


    void ShootAct()
    {
        GameObject vbullet;
        Rigidbody2D vrigidbody;
        Vector3 v3,v3_position;
        Vector2 v2, input;
        float angle;
        v3 = Camera.main.WorldToScreenPoint(transform.position);  //自己位置轉成螢幕座標
        v2 = new Vector2(v3.x, v3.y); //再轉乘二維向量
        input = new Vector2(Input.mousePosition.x, Input.mousePosition.y); //紀錄滑鼠位置
        Vector2 normalied = ((input - v2)).normalized;  //滑鼠與自己的向量差正規化
        angle = Mathf.Atan2(-(input - v2).x, (input - v2).y) * Mathf.Rad2Deg;
        //算向量差與x軸的夾角的餘角(因為是讓子彈原是90度開始轉)
        v3_position = transform.position + new Vector3(transform.lossyScale.x,0.7f,0);
        if ((Input.GetMouseButtonDown(1) && f_shoot > 0.5f) || (Input.GetMouseButtonDown(1) && f_shoot == 0))//射子彈
        {
            vbullet = Instantiate(O_bullet,v3_position, Quaternion.Euler(0f, 0f, 0f)) as GameObject;
            vrigidbody = vbullet.GetComponent<Rigidbody2D>();
            vrigidbody.velocity = new Vector3(normalied.x * 25, normalied.y * 25, 0.0f);
            vbullet.transform.rotation = Quaternion.Euler(0f, 0f, angle);
            i_hp--;
            HP_ui.PresentHp(ref i_hp);
            f_shoot = 0;
        }
    }


    //void Through()
    //{
    //    if (Input.GetKey(KeyCode.H))
    //    {
    //        Physics2D.IgnoreCollision(player_coll, O_wall.GetComponent<Collider2D>(), true);
    //    }
    //    else
    //    {
    //        Physics2D.IgnoreCollision(player_coll, O_wall.GetComponent<Collider2D>(), false);
    //    }
    //}

    //void TeleportLeft()
    //{
    //    if (Input.GetKeyDown(KeyCode.J))
    //    {
    //        b_use_skill = true;
    //        Instantiate(O_virtualplayer, transform.position + new Vector3(5f, -0.5f, 0), Quaternion.identity);
    //    }
    //    else if (Input.GetKeyUp(KeyCode.J))
    //    {
    //        transform.position = transform.position + new Vector3(5f, -0.5f, 0);
    //        b_use_skill = false;
    //    }
    //}

    //腳色重生
    void PlayerRespawn()
    {
        f_dietime += Time.deltaTime;
        Debug.Log(f_dietime);
        if (f_dietime > 1.3f)
        {
            this.player_rig.velocity = new Vector2(0, 0);
            b_die = false;
            f_dietime = 0;
        }
    }

    //受傷
    void GetHurt()
    {
        i_hp--;
        HP_ui.PresentHp(ref i_hp);
    }

    //判斷掉落死亡
    void IsDie()
    {
        if (transform.position.y < O_dieline.transform.position.y)
        {
            b_die = true;
        }
    }


    void OnCollisionStay2D(Collision2D coll)
    {
        //遍歷每一碰撞點，判斷
        foreach (ContactPoint2D con in coll.contacts)
        {
            if (!b_isground && Mathf.Sign(con.normal.x) == - (Mathf.Sign(transform.localScale.x)) && coll.gameObject.tag == "floor")
            {
                b_airmove = true;
            }
            else
            {
                b_airmove = false;
            }
        }
    }
    void OnCollisionExit2D(Collision2D coll) {
        b_airmove = false;
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        Debug.Log("gg");
        if (collider.tag == "hp_props")
        {
            i_hp++;
            HP_ui.PresentHp(ref i_hp);
            Destroy(collider.gameObject);
        }
        else if (collider.tag == "save_point")
        {
            b_is_save = true;
            Debug.Log(b_is_save);
            Destroy(collider.gameObject);
        }

    }
}
