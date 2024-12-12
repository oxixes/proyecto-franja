using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelDisable : MonoBehaviour
{
    public string levelName;

    // Start is called before the first frame update
    void Start()
    {
        if (SaveManager.GetInstance().Get<int>(levelName + "Disabled") == 1)
        {
            gameObject.SetActive(false);
        }
    }

    // Before disabling the object, save the state
    public void Disable()
    {
        SaveManager.GetInstance().Set(levelName + "Disabled", 1);
        gameObject.SetActive(false);
    }

    // After enabling the object, save the state
    public void Enable()
    {
        SaveManager.GetInstance().Set(levelName + "Disabled", 0);
        gameObject.SetActive(true);
    }
}
