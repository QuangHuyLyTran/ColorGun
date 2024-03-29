using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AutoLoader : MonoBehaviour
{
    public static bool initialSceneLoaded;

    private void Update()
    {
        if (initialSceneLoaded)
        {
            Debug.LogWarning("The Initial scene has already been loaded. This is not allowed. Cancelling main menu auto-load.");
            return;
        }
        initialSceneLoaded = true;

        SceneManager.LoadScene("Lobby");
        gameObject.SetActive(false);
    }
}
