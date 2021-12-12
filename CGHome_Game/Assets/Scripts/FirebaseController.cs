using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;

public class FirebaseController : MonoBehaviour
{
    public static string key;
    public static Player player1;
    public static Player player2;

    public static bool isKeyCorrect;

    private static DatabaseReference databaseRef;

    // Start is called before the first frame update
    void Start()
    {
        // This object will not be destroyed while the game is being played
        DontDestroyOnLoad(this.gameObject);

        databaseRef = FirebaseDatabase.DefaultInstance.RootReference;

        player1 = new Player();
        player2 = new Player();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static IEnumerator CreateGameFB(string p1Name)
    {
        // Create a random key which represents a game
        key = databaseRef.Child("Objects").Push().Key;

        // Set player 1 name to the inputted value and convert it to Json
        player1.Name = p1Name;
        string p1Json = JsonUtility.ToJson(player1);

        // Upload the data to the database
        yield return databaseRef.Child("Objects").Child(key).Child("Player 1").SetRawJsonValueAsync(p1Json);

        Debug.Log("Player 1 was successfully added to Firebase");

        // Redirect user to the Lobby scene
        GameManager.LoadScene("Lobby");
    }

    public static IEnumerator CheckKey(string key)
    {
        yield return databaseRef.Child("Objects").Child(key).GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                Debug.Log(snapshot.Value);

                if (snapshot.Value != null)
                {
                    Debug.Log("Correct Key");
                    isKeyCorrect = true;
                }
                else
                {
                    Debug.Log("Incorrect Key");
                    isKeyCorrect = false;
                }
            }
        });
    }

    public static IEnumerator AddSecPlayerToFB()
    {
        // Set player 2 name to the inputted value and convert it to Json
        string p2Json = JsonUtility.ToJson(player2);

        // Upload the data to the database
        yield return databaseRef.Child("Objects").Child(key).Child("Player 2").SetRawJsonValueAsync(p2Json);

        Debug.Log("Player 2 was successfully added to Firebase");

        GameManager.LoadScene("Lobby");
    }
}
