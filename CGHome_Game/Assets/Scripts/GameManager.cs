using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField playerName;
    [SerializeField] private TMP_InputField gameCode;
    [SerializeField] private TMP_Text p1Name;

    private void Awake()
    {
        if (SceneManager.GetActiveScene().name == "Lobby")
        {
            gameCode.text = FirebaseController.key;
            p1Name.text = FirebaseController.player1.Name;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void LoadScene(string Scene)
    {
        SceneManager.LoadScene(Scene);
    }

    // First player will create the game. After the user inputs his/her name, the user will be redirected to the 'Lobby' scene
    public void CreateGame()
    {
        if (playerName.text != "")
        {
            // Save to Firebase
            StartCoroutine(FirebaseController.CreateGameFB(playerName.text));
        }
    }
}
