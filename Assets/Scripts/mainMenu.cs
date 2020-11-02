using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class mainMenu : MonoBehaviour
{

    int width;
    int height;
    public TMP_InputField w;
    public TMP_InputField h;
    public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void SetWidth(string s)
    {
        width = Int32.Parse(w.text);
        PlayerPrefs.SetInt("width", width);
    }
    
    public void SetHeight(string s)
    {
        height = Int32.Parse(h.text);
        PlayerPrefs.SetInt("height", height);
    }


    void OnEnable()
    {
        width = PlayerPrefs.GetInt("width");
        height = PlayerPrefs.GetInt("height");
        if(width == 0 || height == 0)
        {
            w.text = "" + 8;
            h.text = "" + 9;
        }
        else
        {
            w.text = ""+width;
            h.text = ""+height;
        }
    }
}
