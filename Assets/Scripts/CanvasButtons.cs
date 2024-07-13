using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CanvasButtons : MonoBehaviour
{
    public Sprite musicOn, musicOff;

    private void Start()
    {
        if (PlayerPrefs.GetString("music") == "No" && gameObject.name == "Music")
            GetComponent<Image>().sprite = musicOff;
    }

    public void RestartGame()
    {
        if (PlayerPrefs.GetString("music") != "No")
            GetComponent<AudioSource>().Play();

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadInstagram()
    {
        if (PlayerPrefs.GetString("music") != "No")
            GetComponent<AudioSource>().Play();

        Application.OpenURL("https://www.youtube.com/channel/UCHQU99yKRd_BQVSci2MZ08w");
    }

    public void MusicWork()
    {
        //если музыка сейчас выключена,то ее надо включить
        if (PlayerPrefs.GetString("music") == "No")
        {
            GetComponent<AudioSource>().Play();
            PlayerPrefs.SetString("music", "Yes");
            GetComponent<Image>().sprite = musicOn;
        }
        else
        {
            PlayerPrefs.SetString("music", "No");
            GetComponent<Image>().sprite = musicOff;
        }
    }
}
