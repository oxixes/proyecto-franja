using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuTextHighlightController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public bool isEnabled = true;

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
        buttonText.text = buttonText.text.Substring(0, buttonText.text.Length - 2) + " <";
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isEnabled)
        {
            return;
        }
        buttonText.text = buttonText.text.Substring(0, buttonText.text.Length - 2) + "  ";
    }
}
