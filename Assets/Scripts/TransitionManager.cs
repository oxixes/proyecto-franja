using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionManager : MonoBehaviour
{
    public enum Coordinate
    {
        X,
        Y,
        BOTH
    }

    public GameObject newLevel;
    private GameObject currentLevel;
    [HideInInspector] public GameObject player;
    public Coordinate coordToChange;
    public Vector2 newPlayerPosition;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        currentLevel = transform.parent.gameObject;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Transition()
    {
        Debug.Log("Transitioning");
        currentLevel.GetComponent<LevelDisable>().Disable();
        newLevel.GetComponent<LevelDisable>().Enable();

        if (coordToChange == Coordinate.X)
        {
            player.transform.position = new Vector2(newPlayerPosition.x, player.transform.position.y);
        }
        else if (coordToChange == Coordinate.Y)
        {
            player.transform.position = new Vector2(player.transform.position.x, newPlayerPosition.y);
        }
        else
        {
            player.transform.position = newPlayerPosition;
        }
    }
}
