using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CanvasButtons : MonoBehaviour
{
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadInstagram()
    {
        Application.OpenURL("https://www.youtube.com/channel/UCHQU99yKRd_BQVSci2MZ08w");
    }
}
