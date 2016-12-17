using UnityEngine;
using System.Collections;

public class camerafollow : MonoBehaviour
{
    //視窗內人物移動邊界變數
    private Transform right_border, left_border = null;
    private float BtwTop, BtwBottom, btwfront, btwback;
    private bool TouchTop, TouchDown, y_axis_change;

    //攝影機移動範圍
    private Vector3 range;

    //角色變數
    private Transform target;
    SpriteRenderer sp;
    private C_Player playerclass;
    private Vector3 playertop, playerbottom, FixedPosition;

    // Use this for initialization
    void Awake()
    {
        target = GameObject.Find("player").transform;
        right_border = this.gameObject.transform.GetChild(0);
        left_border = this.gameObject.transform.GetChild(1);
        range = new Vector3(-10.0f, 46.0f,0.0f);
        playerclass = target.GetComponent<C_Player>();
        sp = target.transform.GetChild(0).GetComponent<SpriteRenderer>();
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

      //  if (Input.GetMouseButtonDown(0)) {
            Debug.Log(playerbottom + "\n" + playertop);
            Debug.Log(transform.position);
       // }

        //跟隨玩家
        FollowPlayer();

        //限制視窗可移動範圍
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, range.x, range.y), transform.position.y, transform.position.z);
    }

    //RESET
    void reset()
    {
        transform.position = new Vector3(playerbottom.x + btwback, transform.position.y, transform.position.z);
    }

    //跟隨玩家
    void FollowPlayer() {
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
        if (playertop.x > right_border.position.x && target.transform.localScale.x == 1)
        {
            transform.position = new Vector3(playertop.x + btwfront, transform.position.y, transform.position.z);
        }
        else if (playerbottom.x < left_border.position.x && target.transform.localScale.x == -1)
        {
            transform.position = new Vector3(playerbottom.x + btwback, transform.position.y, transform.position.z);
        }
    }
}





 //if (Camera.main.WorldToScreenPoint(playertop).y > 130.0f  && transform.position.y<playertop.y - 3.3f)
 //               {
 //                  BtwTop = transform.position.y - playertop.y;
 //                   transform.position = new Vector3(transform.position.x, playertop.y -3.3f, transform.position.z);
 //               }
 //               else if (Camera.main.WorldToScreenPoint(playerbottom).y< 15.0f && transform.position.y> 0)
 //               {
 //               //BtwBottom = transform.position.y - playerbottom.y;
 //               BtwBottom = transform.position.y - playerbottom.y;
 //               transform.position = new Vector3(transform.position.x, playerbottom.y + 4.36f, transform.position.z);
 //               }

 //if (Camera.main.WorldToScreenPoint(playertop).x >195.0f && target.transform.localScale.x == 1)
 //       {
 //           if (Camera.main.WorldToScreenPoint(playertop).x< 196.0f) btwfront = transform.position.x - playertop.x;
 //           //Debug.Log(btwfront);
 //           transform.position = new Vector3(playertop.x + btwfront, transform.position.y, transform.position.z);
 //       }
 //       else if (Camera.main.WorldToScreenPoint(playerbottom).x< 45.0f && target.transform.localScale.x==-1)
 //       {
 //           if(Camera.main.WorldToScreenPoint(playertop).x > 44.0f) btwback = transform.position.x - playerbottom.x;
 //           //Debug.Log(btwback);
 //           transform.position = new Vector3(playerbottom.x + btwback, transform.position.y, transform.position.z);
 //       }