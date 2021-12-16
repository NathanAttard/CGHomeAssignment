using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;
using System;

public class FirebaseController : MonoBehaviour
{
    public static string key;
    public static Player player1 = new Player();
    public static Player player2 = new Player();

    public static string winner = "";

    public static bool isPlayer1 = false;
    public static bool isPlayer2 = false;

    public static bool isKeyCorrect;

    private static DatabaseReference databaseRef;

    // Start is called before the first frame update
    void Start()
    {
        // This object will not be destroyed while the game is being played
        DontDestroyOnLoad(this.gameObject);

        databaseRef = FirebaseDatabase.DefaultInstance.RootReference;
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
        player1.ID = "1";
        player1.Name = p1Name;
        string p1Json = JsonUtility.ToJson(player1);

        // Upload the data to the database
        yield return databaseRef.Child("Objects").Child(key).Child("Player_1").SetRawJsonValueAsync(p1Json);

        databaseRef.Child("Objects").Child(key).ValueChanged += HandlePlayerChanged;

        Debug.Log("Player 1 was successfully added to Firebase");

        // Redirect user to the Lobby scene
        GameManager.LoadScene("Lobby");

        isPlayer1 = true;

        Debug.Log("This is P1: " + isPlayer1);
        Debug.Log("This is P2: " + isPlayer2);
    }

    public static void HandlePlayerChanged(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        else
        {
            Debug.Log("Player Joining");

            foreach (var player in args.Snapshot.Children)
            {
                if (player.Key == "Player_1")
                {
                    foreach(var child in player.Children)
                    {
                        if (child.Key == "Name")
                        {
                            player1.Name = child.Value.ToString();
                        }
                    }
                    
                }
                else if (player.Key == "Player_2")
                {
                    foreach (var child in player.Children)
                    {
                        if (child.Key == "Name")
                        {
                            player2.Name = child.Value.ToString();
                        }
                    }
                }
            }

            Debug.Log("Player Joined");

            Debug.Log("P1 Name: " + player1.Name);
            Debug.Log("P2 Name: " + player2.Name);

            Debug.Log("This is P1: " + isPlayer1);
            Debug.Log("This is P2: " + isPlayer2);
        }
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

    public static IEnumerator AddSecPlayerToFB(string key)
    {
        // Set player 2 name to the inputted value and convert it to Json
        player2.ID = "2";
        string p2Json = JsonUtility.ToJson(player2);

        // Upload the data to the database
        yield return databaseRef.Child("Objects").Child(key).Child("Player_2").SetRawJsonValueAsync(p2Json);

        databaseRef.Child("Objects").Child(key).ValueChanged += HandlePlayerChanged;

        Debug.Log("Player 2 was successfully added to Firebase");

        // Redirect user to the Lobby scene
        GameManager.LoadScene("Lobby");

        isPlayer2 = true;

        Debug.Log("This is P1: " + isPlayer1);
        Debug.Log("This is P2: " + isPlayer2);
    }

    // Update the fields for a particular player
    public static IEnumerator UpdatePlayerDetails(GameObject player)
    {
        Debug.Log("Updating Details");

        Dictionary<string, System.Object> result = new Dictionary<string, System.Object>();

        result["Objects/" + key + "/" + player.name + "/CreatedDate"] = DateTime.Now.ToString();
        result["Objects/" + key + "/" + player.name + "/Position"] = player.transform.position.ToString();
        result["Objects/" + key + "/" + player.name + "/Score"] = 0;
        result["Objects/" + key + "/" + player.name + "/UniqueID"] = key;

        string shapeName = player.GetComponent<SpriteRenderer>().sprite.name;
        result["Objects/" + key + "/" + player.name + "/Shape"] = shapeName.Remove(shapeName.Length - 4);

        databaseRef.UpdateChildrenAsync(result);

        Debug.Log("Details Updated");

        yield return new WaitForSeconds(2f);
    }

    // Update the position of the player
    public static void UpdatePlayerPosition(GameObject player)
    {
        Debug.Log("Updating Player Position");

        Dictionary<string, System.Object> result = new Dictionary<string, System.Object>();

        result["Objects/" + key + "/" + player.name + "/Position"] = player.transform.position.ToString();

        databaseRef.UpdateChildrenAsync(result);

        Debug.Log("Player Position Updated");
    }

    public static void CheckOpponentPosition()
    {
        if (isPlayer1 == true)
        {
            databaseRef.Child("Objects").Child(key).Child("Player_2").ValueChanged += HandleOpponentPosChanged;
        }
        else if (isPlayer2 == true)
        {
            databaseRef.Child("Objects").Child(key).Child("Player_1").ValueChanged += HandleOpponentPosChanged;
        }
    }

    public static void HandleOpponentPosChanged(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        else
        {
            Debug.Log("Opponent Position Changed");

            string playerID = "";
            string position = "";

            foreach (var player in args.Snapshot.Children)
            {
                if (player.Key == "ID")
                {
                    playerID = player.Value.ToString();
                    Debug.Log("ID: " + player.Value);
                }
                else if (player.Key == "Position")
                {
                    position = player.Value.ToString();
                    Debug.Log("Pos: " + position);
                }
            }

            UpdateOpponentPos(playerID, position);
            Debug.Log("Opponent Position Updated");
        }
    }

    public static void UpdateOpponentPos(string opponentID, string opponentPos)
    {
        if (opponentPos != "")
        {
            Vector3 newOppPos = ConvertStringToVector3(opponentPos);

            GameObject opponent = GameObject.Find("Player_" + opponentID).gameObject;

            Vector3 currentOppPos = opponent.transform.position;

            if (currentOppPos != newOppPos)
            {
                opponent.transform.position = newOppPos;
            }
        }
    }

    public static Vector3 ConvertStringToVector3(string posToConvert)
    {
        posToConvert = posToConvert.Substring(1, posToConvert.Length - 2);

        string[] posAxes = posToConvert.Split(',');

        Vector3 newPos = new Vector3(float.Parse(posAxes[0]), float.Parse(posAxes[1]), float.Parse(posAxes[2]));

        return newPos;
    }

    // Update the score of the player
    public static void UpdatePlayerScoreToFB(GameObject player, int playerScore)
    {
        Debug.Log("Updating Player Score");

        Dictionary<string, System.Object> result = new Dictionary<string, System.Object>();

        result["Objects/" + key + "/" + player.name + "/Score"] = playerScore;

        databaseRef.UpdateChildrenAsync(result);
        databaseRef.Child("Objects").Child(key).ValueChanged += HandleScoreChanged;

        Debug.Log("Player Score Updated");

        CheckWinner();
    }

    // Update the Score UI
    public static void HandleScoreChanged(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        else
        {
            Debug.Log("Score Changed");
            Dictionary<string, System.Object> result = new Dictionary<string, System.Object>();

            foreach (var player in args.Snapshot.Children)
            {
                switch (player.Key)
                {
                    case "Player_1":
                        player1.Score = Int32.Parse(player.Child("Score").Value.ToString());
                        break;
                    case "Player_2":
                        player2.Score = Int32.Parse(player.Child("Score").Value.ToString());
                        break;
                    default:
                        break;
                }
            }

            Debug.Log("Score Updated");
        }
    }

    public static void CheckWinner()
    {
        if (player1.Score == 10)
        {
            winner = player1.Name;
        }
        else if(player2.Score == 10)
        {
            winner = player2.Name;
        }
    }
}
