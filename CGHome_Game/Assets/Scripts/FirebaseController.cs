using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;
using System;

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

    //// Call this method before UpdatePlayerDetails to check which player needs to be updated
    //public static void CheckPlayerToUpdate(GameObject player)
    //{
    //    switch (player.name)
    //    {
    //        case "Player 1":
    //        // UpdatePlayerDetails(player, player1)
    //        case "Player 2":
    //        // UpdatePlayerDetails(player, player1)
    //        default:
    //            break;

    //    }
    //}

    public static IEnumerator UpdatePlayerDetails(GameObject player)
    {
        if (player.name == "Player 1")
        {
            // Update player's details
            player1.CreatedDate = DateTime.Now.ToString();
            player1.Position = player.transform.position;

            string shapeName = player.GetComponent<SpriteRenderer>().sprite.name;
            player1.Shape = shapeName.Remove(shapeName.Length - 4);

            // Convert to Json
            string p1Json = JsonUtility.ToJson(player1);

            // Upload the data to the database
            yield return databaseRef.Child("Objects").Child(key).Child("Player 1").SetRawJsonValueAsync(p1Json);
        }
        else
        {
            // Update player's details
            player2.CreatedDate = DateTime.Now.ToString();
            player2.Position = player.transform.position;

            string shapeName = player.GetComponent<SpriteRenderer>().sprite.name;
            player2.Shape = shapeName.Remove(shapeName.Length - 4);

            // Convert to Json
            string p2Json = JsonUtility.ToJson(player2);

            // Upload the data to the database
            yield return databaseRef.Child("Objects").Child(key).Child("Player 2").SetRawJsonValueAsync(p2Json);
        }
    }

    //public static IEnumerator UpdatePlayerDetails(GameObject playerGObj, Player player)
    //{
    //    // Update player's details
    //    player.CreatedDate = DateTime.Now.ToString();
    //    player.Position = playerGObj.transform.position;

    //    string shapeName = playerGObj.GetComponent<SpriteRenderer>().sprite.name;
    //    player.Shape = shapeName.Remove(shapeName.Length - 4);

    //    // Convert to Json
    //    string pJson = JsonUtility.ToJson(player);

    //    // Upload the data to the database
    //    yield return databaseRef.Child("Objects").Child(key).Child(playerGObj.name).SetRawJsonValueAsync(pJson);
    //}
}
