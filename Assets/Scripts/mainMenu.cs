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
    public Slider slider;
    public TMP_Text sldTxt;
    public Toggle toggle;
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

    public void SliderValue()
    {
        sldTxt.text = "" + slider.value;
        PlayerPrefs.SetInt("colorNum", (int)slider.value);
    }

    public void OnColorChange()
    {
        PlayerPrefs.SetInt("changeColor" ,toggle.isOn == true ? 1 : 0);
    }

    void OnEnable()
    {
        PlayerPrefs.SetInt("changeColor" ,toggle.isOn == true ? 1 : 0);
        PlayerPrefs.SetInt("colorNum", (int)slider.value);

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
