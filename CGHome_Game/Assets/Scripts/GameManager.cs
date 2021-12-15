using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField playerName;
    [SerializeField] private TMP_InputField gameCode;
    [SerializeField] private TMP_InputField enterGameCode;
    [SerializeField] private TMP_Text p1Name;
    [SerializeField] private TMP_Text p2Name;
    [SerializeField] private TMP_Text p1Score;
    [SerializeField] private TMP_Text p2Score;
    [SerializeField] private TMP_Text winner;

    private void Awake()
    {
        switch (SceneManager.GetActiveScene().name)
        {
            case "Lobby":
                gameCode.text = FirebaseController.key;
                p1Name.text = FirebaseController.player1.Name;
            
                if (FirebaseController.player2.Name != "")
                {
                    p2Name.text = FirebaseController.player2.Name;
                }
                break;
            case "Game":
                p1Score.text = FirebaseController.player1.Score.ToString();
                p2Score.text = FirebaseController.player2.Score.ToString();
                break;
            case "Winner":
                //Debug.Log("Winner: " + FirebaseController.winner);
                winner.text = FirebaseController.winner;
                break;
            default:
                break;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (SceneManager.GetActiveScene().name == "Game")
        {
            UpdateScoreUI();
            Winner();
        }        
    }

    public static void LoadScene(string Scene)
    {
        SceneManager.LoadScene(Scene);
    }

    // First player will create the game. After the user inputs his/her name, the user will be redirected to the Lobby scene
    public void CreateGame()
    {
        if (playerName.text != "")
        { 
            StartCoroutine(FirebaseController.CreateGameFB(playerName.text));
        }
    }

    // Second player will join the game. Afther the user inputs his/her name, the user will be redirected to the Join scene
    public void EnterCode()
    {
        if (playerName.text !=  "")
        {
            // Store player 2's name
            StartCoroutine(FirebaseController.AddSecPlayerToFB(playerName.text));

            // Redirect user to Join scene
            LoadScene("Join");
        }
    }

    // When the second player inputs the code for the game and validates the code, the user will be redirected to the Lobby scene
    public void JoinGame()
    {
        if (enterGameCode.text != "")
        {
            // Validates the key and if key is correct
            StartCoroutine(FirebaseController.CheckKey(enterGameCode.text));

            // If the key is valid, save data to firebase and redirect user to Lobby
            if (FirebaseController.isKeyCorrect == true)
            {
                LoadScene("Lobby");
            }
        }
    }

    // Update the score in the Game scene
    public void UpdateScoreUI()
    {
        p1Score.text = FirebaseController.player1.Score.ToString();
        p2Score.text = FirebaseController.player2.Score.ToString();
    }

    // Check for winner
    public void Winner()
    {
        if (FirebaseController.winner != "")
        {
            LoadScene("Winner");
        }
    }

    // Quit
    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
