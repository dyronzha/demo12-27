using UnityEngine;
using System.Collections;

public class _CPlayer : MonoBehaviour {
    float f_speed;
    bool b_upside;
    Rigidbody2D player_rig;
    Animator player_animator;
	// Use this for initialization
	void Awake () {
        player_rig = gameObject.GetComponent<Rigidbody2D>();
        player_animator = transform.GetChild(0).GetComponent<Animator>();
        f_speed = 3.0f;
	}
	
	// Update is called once per frame
	void Update () {
        Move();
	}
    void Move() {
        if (!b_upside)
        {
            if (Input.GetKey(KeyCode.A))
            {
                transform.localScale = new Vector3(-0.75f, 0.75f, 1f);//轉向用

                player_rig.velocity = new Vector2(-f_speed, player_rig.velocity.y); //速度等於speed
                player_animator.SetBool("walk", true);  //動畫開關
            }
            else if (Input.GetKey(KeyCode.D))
            {
                transform.localScale = new Vector3(0.75f, 0.75f, 1f);//轉向用
                player_rig.velocity = new Vector2(f_speed, player_rig.velocity.y);
                player_animator.SetBool("walk", true);
            }
        }

        else
        {
            if (Input.GetKey(KeyCode.A))
            {
                transform.localScale = new Vector3(-0.75f, -0.75f, 1f);//轉向用
                player_rig.velocity = new Vector2(-f_speed, player_rig.velocity.y);
                player_animator.SetBool("walk", true);

            }
            else if (Input.GetKey(KeyCode.D))
            {
                transform.localScale = new Vector3(0.75f, -0.75f, 1f);//轉向用
                player_rig.velocity = new Vector2(f_speed, player_rig.velocity.y);
                player_animator.SetBool("walk", true);
            }
        }

            if (!(Input.GetKey(KeyCode.A)) && (!Input.GetKey(KeyCode.D)))
            {
            player_animator.SetBool("walk", false);
            }
     }
}
