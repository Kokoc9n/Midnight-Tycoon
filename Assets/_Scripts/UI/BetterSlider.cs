using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BetterSlider : Slider
{
    private GraphicRaycaster graphicRaycaster;

    protected override void Awake()
    {
        base.Awake();
        graphicRaycaster = FindObjectOfType<GraphicRaycaster>();
    }

    protected override void Update()
    {
        base.Update();
        if (Input.GetMouseButtonDown(0))
        {
            PointerEventData pointerData = new PointerEventData(EventSystem.current)
            {
                position = Input.mousePosition
            };

            var raycastResults = new System.Collections.Generic.List<RaycastResult>();
            graphicRaycaster.Raycast(pointerData, raycastResults);

            foreach (var result in raycastResults)
            {
                if (result.gameObject == this.fillRect.gameObject)
                {
                    RectTransform backgroundRect = this.GetComponentInChildren<RectTransform>();
                    Vector2 localPoint;
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(backgroundRect, pointerData.position, null, out localPoint);

                    float normalizedValue = Mathf.InverseLerp(backgroundRect.rect.xMin, backgroundRect.rect.xMax, localPoint.x);
                    this.value = normalizedValue;
                    break;
                }
            }
        }
    }
}
