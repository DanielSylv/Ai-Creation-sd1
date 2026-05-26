using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UpgradeItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Upgrade upgrade;
    private Transform originalParent;
    private Vector3 originalPosition;
    private Transform dragParent; // Parent while dragging (e.g., Canvas)

    void Start()
    {
        originalParent = transform.parent;
        dragParent = GameObject.Find("Canvas").transform;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalPosition = transform.position;
        transform.SetParent(dragParent);
        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        GetComponent<CanvasGroup>().blocksRaycasts = true;

        // Check if dropped in the CartPanel
        if (RectTransformUtility.RectangleContainsScreenPoint(
            ShopManager.instance.cartPanel.GetComponent<RectTransform>(),
            eventData.position))
        {
            transform.SetParent(ShopManager.instance.cartPanel.transform);
            ShopManager.instance.AddToCart(this);
        }
        else
        {
            transform.SetParent(originalParent);
            transform.position = originalPosition;
        }
    }
}