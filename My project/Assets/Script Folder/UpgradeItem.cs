using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UpgradeItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Upgrade upgrade;
    private Transform originalParent;
    private Vector3 originalPosition;
    private Transform dragParent;

    void Start()
    {
        originalParent = transform.parent;
        dragParent = GameObject.Find("Canvas").transform;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalPosition = transform.position;
        transform.SetParent(dragParent);
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup != null)
        {
            canvasGroup.blocksRaycasts = false;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup != null)
        {
            canvasGroup.blocksRaycasts = true;
        }

        // Check if dropped in the CartPanel
        if (RectTransformUtility.RectangleContainsScreenPoint(
            ShopManager.instance.cartPanel.GetComponent<RectTransform>(),
            eventData.position))
        {
            // Pass the upgrade data to ShopManager
            ShopManager.instance.AddToCart(this.upgrade);
        }

        // Always return to original position
        transform.SetParent(originalParent);
        transform.position = originalPosition;
    }
}