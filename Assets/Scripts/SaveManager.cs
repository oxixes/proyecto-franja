using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    [CanBeNull] private static SaveManager instance = null;

    // Start is called before the first frame update
    public SaveManager()
    {
        if (instance != null)
        {
            Debug.LogWarning("Replacing instance of SaveManager");
        }

        instance = this;
    }

    /// <summary>
    /// Get the instance of the SaveManager in the scene.
    /// 
    /// This is a singleton. You mustn't instantiate a SaveManager yourself.
    /// </summary>
    /// <returns>The instance of the current Save Manager.</returns>
    public static SaveManager GetInstance()
    {
        if (instance == null)
        {
            Debug.LogError("You called GetInstance of SaveManager before it was instantiated in the scene!");
        }

        return instance;
    }

    /// <summary>
    /// Gets the value of a key stored.
    /// </summary>
    /// <typeparam name="T">The type returned. Make sure the key is of this type.</typeparam>
    /// <param name="key">Key of the value to get.</param>
    /// <returns>The value linked to the key, or null, or the default value of the type.</returns>
    public T Get<T>(string key)
    {
        if (PlayerPrefs.HasKey(key))
        {
            if (typeof(T) == typeof(string))
            {
                return (T)(object)PlayerPrefs.GetString(key);
            }
            else if (typeof(T) == typeof(int))
            {
                return (T)(object)PlayerPrefs.GetInt(key);
            }
            else if (typeof(T) == typeof(float))
            {
                return (T)(object)PlayerPrefs.GetFloat(key);
            }
            else
            {
                return (T)(object)PlayerPrefs.GetString(key);
            }
        }
        else
        {
            return default(T);
        }
    }

    /// <summary>
    /// Set a value to a key. It is stored between sessions.
    /// </summary>
    /// <typeparam name="T">The type of the value to store.</typeparam>
    /// <param name="key">The key to access the value.</param>
    /// <param name="value">The value to store.</param>
    public void Set<T>(string key, T value)
    {
        if (typeof(T) == typeof(string))
        {
            PlayerPrefs.SetString(key, (string)(object)value);
        }
        else if (typeof(T) == typeof(int))
        {
            PlayerPrefs.SetInt(key, (int)(object)value);
        }
        else if (typeof(T) == typeof(float))
        {
            PlayerPrefs.SetFloat(key, (float)(object)value);
        }
        else
        {
            PlayerPrefs.SetString(key, value.ToString());
        }

        PlayerPrefs.Save();
    }

    public void DeleteAll()
    {
        Debug.LogWarning("All player preferences will be deleted. If this is not what you want, check the code!!");
        // Print the stack trace to know where this method was called.
        Debug.LogWarning(System.Environment.StackTrace);
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }

}
