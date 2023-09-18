using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DevLoader : MonoBehaviour
{
    public static bool loaded;

    private void Awake()
    {
        if (SceneManager.GetActiveScene().name == "GamePlay")
        {
            loaded = true;
        }

        if (!loaded)
        {
            SceneManager.LoadScene("GamePlay");
        }
    }
}
