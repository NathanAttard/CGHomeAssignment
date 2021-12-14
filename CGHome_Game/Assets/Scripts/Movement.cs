using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    int p1Counter;
    int p2Counter;

    int p1Score;
    int p2Score;

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
                p1Counter = Move(KeyCode.A, KeyCode.D, KeyCode.W, KeyCode.S, p1Counter);
                
                if (p1Counter == 10)
                {
                    p1Counter = 0;
                    p1Score = Score(p1Score);
                }

                break;
            case "Player2":
                p2Counter = Move(KeyCode.LeftArrow, KeyCode.RightArrow, KeyCode.UpArrow, KeyCode.DownArrow, p2Counter);

                if (p2Counter == 10)
                {
                    p2Counter = 0;
                    p2Score = Score(p2Score);
                }

                break;
            default:
                break;
        }
    }

    int Move(KeyCode left, KeyCode right, KeyCode up, KeyCode down, int counter)
    {
        if (Input.GetKeyDown(left))
        {
            Vector3 positon = this.transform.position;
            positon.x--;
            this.transform.position = positon;
            FirebaseController.UpdatePlayerPosition(this.gameObject);
        }

        if (Input.GetKeyDown(right))
        {
            Vector3 positon = this.transform.position;
            positon.x++;
            this.transform.position = positon;
            FirebaseController.UpdatePlayerPosition(this.gameObject);
        }

        if (Input.GetKeyDown(up))
        {
            Vector3 positon = this.transform.position;
            positon.y++;
            this.transform.position = positon;

            Debug.Log("Counter Before: " + counter);
            counter++;
            Debug.Log("Counter After: " + counter);
            Debug.Log("P1 Counter: " + p1Counter);
            Debug.Log("P2 Counter: " + p2Counter);

            FirebaseController.UpdatePlayerPosition(this.gameObject);
        }

        if (Input.GetKeyDown(down))
        {
            Vector3 positon = this.transform.position;
            positon.y--;
            this.transform.position = positon;

            Debug.Log("Counter Before: " + counter);
            counter++;
            Debug.Log("Counter After: " + counter);
            Debug.Log("P1 Counter: " + p1Counter);
            Debug.Log("P2 Counter: " + p2Counter);

            FirebaseController.UpdatePlayerPosition(this.gameObject);
        }

        return counter;
    }

    int Score(int scoreCount)
    {
        scoreCount++;
        FirebaseController.UpdatePlayerScore(this.gameObject, scoreCount);

        return scoreCount;
    }
}
