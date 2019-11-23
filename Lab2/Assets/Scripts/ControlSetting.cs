using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ControlSetting : MonoBehaviour
{

    private Dictionary<string, KeyCode> controlKeys = new Dictionary<string, KeyCode>();

    public Text Up1, Down1, Left1, Right1, Fire1;
    
    private Color32 defaultColor = new Color32(255,255,255,255);
    private Color32 selectedColor = new Color32(255,127,0,255);
    private GameObject currentKeyToSetup;
    // Start is called before the first frame update
    void Start()
    {
        controlKeys.Add("Up1", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Up1","W")));
        controlKeys.Add("Down1", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Down1","S")));
        controlKeys.Add("Left1", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Left1","A")));
        controlKeys.Add("Right1", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Right1","D")));
        controlKeys.Add("Fire1", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Fire1","Space")));

        Up1.text = controlKeys["Up1"].ToString();
        Down1.text = controlKeys["Down1"].ToString();
        Left1.text = controlKeys["Left1"].ToString();
        Right1.text = controlKeys["Right1"].ToString();
        Fire1.text = controlKeys["Fire1"].ToString();
    }

    void OnGUI()
    {
        if (currentKeyToSetup != null)
        {
            Event e = Event.current;
            if (e.isKey)
            {
                controlKeys[currentKeyToSetup.name] = e.keyCode;
                currentKeyToSetup.transform.GetChild(0).GetComponent<Text>().text = e.keyCode.ToString();
                currentKeyToSetup.GetComponent<Image>().color = defaultColor;
                currentKeyToSetup = null;
            }
        }
    }

    public void ChangeKeyControl(GameObject clickedKey)
    {
        if (currentKeyToSetup != null)
        {
            currentKeyToSetup.GetComponent<Image>().color = defaultColor;
        }
        
        currentKeyToSetup = clickedKey;
        currentKeyToSetup.GetComponent<Image>().color = selectedColor;
    }

    public void saveKeySetting()
    {
        foreach (var key in controlKeys)
        {
            PlayerPrefs.SetString(key.Key, key.Value.ToString());
        }
        
        PlayerPrefs.Save();
        SceneManager.LoadScene("Launcher");
    }
}
