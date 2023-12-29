using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Transform parentToReturnTo = null;
    public Transform placeholderParent = null;

    GameObject placeholder = null;

    public void OnBeginDrag(PointerEventData eventData)
    {
        placeholder = new GameObject();
        placeholder.transform.SetParent(this.transform.parent);

        RectTransform placeholderRectTransform = placeholder.AddComponent<RectTransform>();
        RectTransform cardRectTransform = this.transform.GetComponent<RectTransform>();

        placeholderRectTransform.localPosition = cardRectTransform.localPosition;
        placeholderRectTransform.localScale = cardRectTransform.localScale;
        placeholderRectTransform.sizeDelta = cardRectTransform.sizeDelta;
        placeholderRectTransform.anchorMin = cardRectTransform.anchorMin;
        placeholderRectTransform.anchorMax = cardRectTransform.anchorMax;
        placeholderRectTransform.anchoredPosition = cardRectTransform.anchoredPosition;
        placeholderRectTransform.pivot = cardRectTransform.pivot;

        placeholder.transform.SetSiblingIndex(this.transform.GetSiblingIndex());

        parentToReturnTo = this.transform.parent;
        placeholderParent = parentToReturnTo;
        this.transform.SetParent(this.transform.parent.parent);

        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Update the position of the card to follow the mouse cursor.
        this.transform.position = eventData.position;

        // If the placeholder object is null, exit the function.
        if (placeholder == null)
        {
            return;
        }

        // Check if the placeholderParent is not null.
        if (placeholderParent != null)
        {
            // If the placeholder's parent is not the same as the placeholderParent, update it.
            if (placeholder.transform.parent != placeholderParent)
            {
                placeholder.transform.SetParent(placeholderParent);
            }

            // Start with the assumption that the new position for the placeholder is at the end.
            int newSiblingIndex = placeholderParent.childCount;

            // Loop through all children of the placeholderParent.
            for (int i = 0; i < placeholderParent.childCount; i++)
            {
                // Check if the card's position is less than the current child's position.
                if (this.transform.position.x < placeholderParent.GetChild(i).position.x)
                {
                    // Update the newSiblingIndex to the current index.
                    newSiblingIndex = i;

                    // If the placeholder is already in a lower position, adjust the index to insert before the current child.
                    if (placeholder.transform.GetSiblingIndex() < newSiblingIndex)
                    {
                        newSiblingIndex--;
                    }

                    // Break the loop as we found the new position for the placeholder.
                    break;
                }
            }

            // Set the sibling index of the placeholder to the new calculated position.
            placeholder.transform.SetSiblingIndex(newSiblingIndex);
        }
    }



    public void OnEndDrag(PointerEventData eventData)
    {
        this.transform.SetParent(parentToReturnTo);
        this.transform.SetSiblingIndex(placeholder.transform.GetSiblingIndex());
        GetComponent<CanvasGroup>().blocksRaycasts = true;

        Destroy(placeholder);
    }
}
