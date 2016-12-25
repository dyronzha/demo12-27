using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_Teleport : MonoBehaviour {

    //public GameObject O_virtualplayer = null; //虛像
    C_Player player;
    bool tele_skill = false;
    GameObject camera;

    void Start () {
        player = this.GetComponent<C_Player>();
        Debug.Log(player.direction);
        camera = GameObject.Find("Main Camera");
    }
    void Teleport()
    {
        if (player.direction)
        {
            if (Input.GetKeyDown(KeyCode.J))
            {
                tele_skill = true;
                player.b_use_skill = true;
                //Instantiate(O_virtualplayer, transform.position + new Vector3(5f, -0.5f, 0), Quaternion.identity);
            }
            else if (Input.GetKeyUp(KeyCode.J))
            {
                tele_skill = false;
                transform.position = transform.position + new Vector3(5f, -0.5f, 0);
                player.b_use_skill = false;
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.J))
            {
                tele_skill = true;
                player.b_use_skill = true;
                //Instantiate(O_virtualplayer, transform.position + new Vector3(-5f, -0.5f, 0), Quaternion.identity);
            }
            else if (Input.GetKeyUp(KeyCode.J))
            {
                tele_skill = false;
                transform.position = transform.position + new Vector3(-5f, -0.5f, 0);
                player.b_use_skill = false;
            }
        }
    }
    void Update () {
        Teleport();
        if (tele_skill) camera.SendMessage("TeleMove");
    }
}
