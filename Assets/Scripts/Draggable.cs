using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Playables;
using UnityEngine.UI;

public class Draggable : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    Vector2 mousePosition;
    Vector2 difference;
    public Transform parentToReturnTo; // Store the initial position for resetting
    public float smooth = 100f; // You can adjust the smoothness

    private GameObject placeholder = null;
    

    void Start()
    {
        // No need to set smooth here, as it's better to define it directly
        parentToReturnTo = this.transform.parent; // Store the initial position
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("OnBeginDrag");

        CreatePlaceholder(this.transform);

        mousePosition = eventData.position;
        difference = (Vector2)transform.position - mousePosition;

        this.transform.SetParent(this.parentToReturnTo.parent);
        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    private void CreatePlaceholder(Transform cardTransform)
    {
        placeholder = new GameObject();
        placeholder.transform.SetParent(cardTransform.parent);
 
        RectTransform placeholderRectTransform = placeholder.AddComponent<RectTransform>();
        RectTransform cardRectTransform = cardTransform.GetComponent<RectTransform>();

        placeholderRectTransform.localPosition = cardRectTransform.localPosition;
        placeholderRectTransform.localScale = cardRectTransform.localScale;
        placeholderRectTransform.sizeDelta = cardRectTransform.sizeDelta;
        placeholderRectTransform.anchorMin = cardRectTransform.anchorMin;
        placeholderRectTransform.anchorMax = cardRectTransform.anchorMax;
        placeholderRectTransform.anchoredPosition = cardRectTransform.anchoredPosition;
        placeholderRectTransform.pivot = cardRectTransform.pivot;

        placeholder.transform.SetSiblingIndex(cardTransform.GetSiblingIndex());
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 newPosition = eventData.position + difference;
        this.transform.position = Vector2.Lerp(transform.position, newPosition, smooth * Time.deltaTime);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("OnEndDrag");
        // If you want to reset the position when the drag ends, you can do it like this:
        // transform.position = initialPosition;
        this.transform.SetParent(this.parentToReturnTo);
        GetComponent<CanvasGroup>().blocksRaycasts = true;
        this.transform.SetSiblingIndex(placeholder.transform.GetSiblingIndex());
        
        Destroy(placeholder);
    }
}