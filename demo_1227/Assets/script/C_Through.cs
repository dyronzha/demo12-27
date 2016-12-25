using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_Through : MonoBehaviour {

    public GameObject O_wall;
    private Collider2D player_coll;
    bool a = false;
    float timer = 0;
    void Awake() {
        O_wall = GameObject.Find("demo_center06");

        player_coll = this.GetComponent<Collider2D>();
    }
    void Through() //穿透
    {
        if (Input.GetKey(KeyCode.H))
        {
            Debug.Log("H key down");
            Physics2D.IgnoreCollision(player_coll, O_wall.GetComponent<BoxCollider2D>(), true);
        }
        else
        {
            //Physics2D.IgnoreCollision(player_coll, O_wall.GetComponent<BoxCollider2D>(), false);
        }
    }   
    // Update is called once per frame
    void Update () {
        // Through();
        if (a)
        {
            Debug.Log("insidea ");
      
            Through();
            timer +=Time.deltaTime;
            if (timer > 0.5)
            {
                a = false;
                timer = 0;
            }
                
        }
        else
        {
            timer = 0;
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject == O_wall) {
            a = true;
            Debug.Log("a");
               
        }
        //if (collision.gameObject == O_wall)
        //{
        //    if (Input.GetKey(KeyCode.H))
        //    {
        //        Debug.Log("H key down");
        //        Physics2D.IgnoreCollision(player_coll, O_wall.GetComponent<BoxCollider2D>(), true);
        //    }   

        //}


    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject == O_wall)
        {
            a = true;
            Debug.Log("a");

        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (timer == 0&&!a)
        {
            Physics2D.IgnoreCollision(player_coll, O_wall.GetComponent<BoxCollider2D>(), false);

        }
    }
}
