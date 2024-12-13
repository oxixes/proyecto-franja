using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FallingItem : MonoBehaviour
{
    public float fallAcceleration = 0.1f;
    public List<Sprite> possibleSprites;

    [HideInInspector] public ItemsFallingMinigame minigame;

    private float currentFallSpeed = 0.0f;
    private RectTransform rectTransform;

    // Start is called before the first frame update
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        rectTransform.position -= new Vector3(0, currentFallSpeed * Time.deltaTime, 0);
        currentFallSpeed += fallAcceleration * Time.deltaTime;
    }

    private void OnEnable()
    {
        currentFallSpeed = 0.0f;

        // Choose a random sprite for the item
        if (possibleSprites.Count > 0)
        {
            Image itemImage = GetComponent<Image>();
            itemImage.sprite = possibleSprites[Random.Range(0, possibleSprites.Count)];
        }
    }
}