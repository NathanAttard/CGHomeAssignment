using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Storage;
using Firebase.Extensions;

public class FirebaseDLC : MonoBehaviour
{
    int playerNo = 0;

    // Start is called before the first frame update
    void Start()
    {
        // Create a reference to the firebase storage
        FirebaseStorage storage = FirebaseStorage.DefaultInstance;
        StorageReference storageReference = storage.GetReferenceFromUrl("gs://cghomeassignment-bc76b.appspot.com");

        // Create a reference to an image in the storage
        StorageReference allSprites = storageReference.Child("Sprites");

        // Create an array with all the sprite names
        string[] sprites = { "Circle.png", "Square.png" };

        // Foreach sprite in the array
        foreach (string sprite in sprites)
        {
            // Download the image
            DownloadSprite(allSprites.Child(sprite));
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void DownloadSprite(StorageReference image)
    {
        // Maximum size
        const long maxAllowedSize = 1 * 1024 * 1024;

        image.GetBytesAsync(maxAllowedSize).ContinueWithOnMainThread(task => {
            // Error occured
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogException(task.Exception);
            }
            else
            {
                byte[] fileContent = task.Result;
                Debug.Log("Download Complete");

                // Create a texture
                Texture2D texture = new Texture2D(10, 10);
                texture.LoadImage(fileContent);

                // Create the sprite
                Sprite newSprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);
                newSprite.name = image.Name;

                // Attach sprite to gameobjet
                CreatePlayer(newSprite);
            }
        });
    }

    // Create the player
    private void CreatePlayer(Sprite sprite)
    {
        Debug.Log("Sprite Name: " + sprite.name);
        Debug.Log("PLayerNo: " + playerNo);

        // Check the sprite name to set the player accordingly
        if (sprite.name == "Circle.png")
        {
            playerNo = 1;
        }
        else if (sprite.name == "Square.png")
        {
            playerNo = 2;
        }

        // Create an empty GameObject
        GameObject player = new GameObject();

        // Set details
        player.name = "Player_" + playerNo;
        player.tag = "Player" + playerNo;

        if (player.tag == "Player1")
        {
            player.transform.position = new Vector2(-2f, 0f);
        }
        else if (player.tag == "Player2")
        {
            player.transform.position = new Vector2(2f, 0f);
        }

        // Add components
        player.AddComponent<SpriteRenderer>();
        player.AddComponent<Rigidbody2D>();
        player.AddComponent<BoxCollider2D>();
        player.AddComponent<Movement>();

        // Update fields in components
        player.GetComponent<SpriteRenderer>().sprite = sprite;
        player.GetComponent<Rigidbody2D>().gravityScale = 0;
        player.GetComponent<Rigidbody2D>().freezeRotation = true;
        player.GetComponent<BoxCollider2D>().size = new Vector2(1.2f, 1.2f);

        // Update details in Firebase
        StartCoroutine(FirebaseController.UpdatePlayerDetails(player));
    }
}
