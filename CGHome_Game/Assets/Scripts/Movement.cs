using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        switch (this.tag)
        {
            case "Player1":
                Keys(KeyCode.A, KeyCode.D, KeyCode.W, KeyCode.S);
                break;
            case "Player2":
                Keys(KeyCode.LeftArrow, KeyCode.RightArrow, KeyCode.UpArrow, KeyCode.DownArrow);
                break;
            default:
                break;
        }
    }

    void Keys(KeyCode left, KeyCode right, KeyCode up, KeyCode down)
    {
        if (Input.GetKeyDown(left))
        {
            Vector3 positon = this.transform.position;
            positon.x--;
            this.transform.position = positon;
        }

        if (Input.GetKeyDown(right))
        {
            Vector3 positon = this.transform.position;
            positon.x++;
            this.transform.position = positon;
        }

        if (Input.GetKeyDown(up))
        {
            Vector3 positon = this.transform.position;
            positon.y++;
            this.transform.position = positon;
        }

        if (Input.GetKeyDown(down))
        {
            Vector3 positon = this.transform.position;
            positon.y--;
            this.transform.position = positon;
        }

        FirebaseController.UpdatePlayerPosition(this.gameObject);
    }
}
