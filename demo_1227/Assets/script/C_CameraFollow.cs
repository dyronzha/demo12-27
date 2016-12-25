using UnityEngine;
using System.Collections;

public class C_CameraFollow : MonoBehaviour {
    //視窗內人物移動邊界變數
    private Transform right_border, left_border = null;
    private float BtwTop, BtwBottom, btwfront, btwback;
    private bool TouchTop, TouchDown, y_axis_change;

    //攝影機移動範圍
    private Vector3 range_x,range_y;

    //角色變數
    private Transform target;
    SpriteRenderer sp;
    private C_Player playerclass;
    private Vector3 playertop, playerbottom, FixedPosition, tele_move_vec3;
    // Use this for initialization
    void Awake()
    {
        
        target = GameObject.Find("Player").transform;
        right_border = this.gameObject.transform.GetChild(0);
        left_border = this.gameObject.transform.GetChild(1);
        range_x = new Vector3(16.88f, 34.2f, 0.0f);
        range_y = new Vector3(15.05f, 22.2f, 0.0f);
        playerclass = target.GetComponent<C_Player>();
        sp = target.transform.GetChild(2).GetComponent<SpriteRenderer>();
        playerbottom = sp.bounds.min;
        playertop = sp.bounds.max;
        FixedPosition = transform.position;
        //紀錄自定義範圍和攝影機範圍的向量差
        BtwTop = transform.position.y - right_border.position.y;
        BtwBottom = transform.position.y - left_border.position.y;
        btwfront = transform.position.x - right_border.position.x;
        btwback = transform.position.x - left_border.position.x;
        TouchTop = TouchDown = y_axis_change = true;
    }

    // Update is called once per frame
    void Update()
    {
        //抓取角色圖片座標
        playerbottom = sp.bounds.min;
        playertop = sp.bounds.max;
        //transform.position.x * 0.9f + target.transform.position.x * 0.1f

        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log(playertop.x  + "\n" + right_border.position.x);
            Debug.Log(transform.position);
             }

            //跟隨玩家
            FollowPlayer();

        //限制視窗可移動範圍
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, range_x.x, range_x.y), Mathf.Clamp(transform.position.y, range_y.x, range_y.y), transform.position.z);
    }

    //RESET
    void reset()
    {
        transform.position = new Vector3(playerbottom.x + btwback, transform.position.y, transform.position.z);
    }

    //跟隨玩家
    void FollowPlayer()
    {
        if (!playerclass.b_isground)  //玩家離地
        {
            //玩家y軸超出範圍
            if (playertop.y > right_border.position.y && transform.position.y < playertop.y + BtwTop)
            {
                //讓攝影機移到目前玩家位置加一開始紀錄的向量差
                transform.position = new Vector3(transform.position.x, playertop.y + BtwTop, transform.position.z);
                y_axis_change = true;  //往上超出邊界
            }
            else if (playerbottom.y < left_border.position.y && transform.position.y > 0)
            {
                transform.position = new Vector3(transform.position.x, playerbottom.y + BtwBottom, transform.position.z);
                y_axis_change = true;  //往下超出邊界
            }
        }
        else //玩家在地板上，讓攝影機回到玩家身上
        {
            Vector3 back_player;
            //正向時
            if (Mathf.Abs(target.transform.position.y + 2.1f - this.transform.position.y) > 0.1f && !playerclass.b_upside)
            {
                //玩家與攝影機的向量差並單位化
                back_player = (target.transform.position + new Vector3(0, 1.8f, 0) - this.transform.position).normalized;
                //慢慢讓攝影機加到位置
                this.transform.position += new Vector3(0, back_player.y * 0.5f, 0);
            }
            //倒向時
            else if (Mathf.Abs(target.transform.position.y - 2.1f - this.transform.position.y) > 0.3f && playerclass.b_upside)
            {
                back_player = (target.transform.position + new Vector3(0, -2.1f, 0) - this.transform.position).normalized;
                this.transform.position += new Vector3(0, back_player.y * 0.5f, 0);
            }
        }

        //超出左右邊界
        if (playertop.x > right_border.position.x && target.transform.localScale.x >0)
        {
            transform.position = new Vector3(playertop.x + btwfront, transform.position.y, transform.position.z);
        }
        else if (playerbottom.x < left_border.position.x && target.transform.localScale.x <0)
        {
            transform.position = new Vector3(playerbottom.x + btwback, transform.position.y, transform.position.z);
        }
    }
    public void TeleMove() {
        tele_move_vec3 = new Vector3(target.transform.position.x+ (Mathf.Sign(target.transform.localScale.x) * 2.5f), transform.position.y, transform.position.z);
        Vector3 btw_tele = (tele_move_vec3 - transform.position).normalized*0.5f;
        if (Vector3.Distance(transform.position, tele_move_vec3) > 1.0f) {
            transform.position += btw_tele;
        }
    }
    public void ResetPos() {
        transform.position = new Vector3(target.transform.position.x, target.transform.position.y,transform.position.z) ;
    }
}
