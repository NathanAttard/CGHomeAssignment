using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Storage;
using Firebase.Extensions;

public class FirebaseDLC : MonoBehaviour
{
    int playerCount;

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
        // Increase the number of players
        playerCount++;

        // Create an empty GameObject
        GameObject player = new GameObject();

        // Set details
        player.name = "Player_" + playerCount;
        player.tag = "Player" + playerCount;
        player.transform.position = new Vector2(Random.Range(-2f, 2f), Random.Range(-2f, 2f));

        // Add components
        player.AddComponent<SpriteRenderer>();
        player.AddComponent<Movement>();

        // Update fields in components
        player.GetComponent<SpriteRenderer>().sprite = sprite;

        // Update details in Firebase
        StartCoroutine(FirebaseController.UpdatePlayerDetails(player));
    }
}
