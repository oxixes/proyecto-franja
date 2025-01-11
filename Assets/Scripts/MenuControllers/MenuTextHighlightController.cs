using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuTextHighlightController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public bool isEnabled = true;
    public bool colorChange = false;

    public AudioSource audioSource;
    public AudioClip hoverSound;

    private TextMeshProUGUI buttonText;

    public void Start()
    {
        buttonText = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isEnabled)
        {
            return;
        }

        if (audioSource != null && hoverSound != null)
        {
            audioSource.PlayOneShot(hoverSound);
        }

        if (colorChange)
        {
            buttonText.text = "<color=yellow>" + buttonText.text + "</color>";
        }
        else
        {
            buttonText.text = buttonText.text.Substring(0, buttonText.text.Length - 2) + " <";
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isEnabled)
        {
            return;
        }

        if (colorChange)
        {
            buttonText.text = buttonText.text.Substring(14, buttonText.text.Length - 22);
        }
        else
        {
            buttonText.text = buttonText.text.Substring(0, buttonText.text.Length - 2) + "  ";
        }
    }
}
