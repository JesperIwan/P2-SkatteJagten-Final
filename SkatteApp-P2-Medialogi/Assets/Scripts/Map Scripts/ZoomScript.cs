using UnityEngine;
using UnityEngine.UI;

public class ZoomScript : MonoBehaviour
{
    public RectTransform content;
    public ScrollRect scrollRect;
    public float zoomSpeed = 0.1f;
    public float minScale = 0.5f;
    public float maxScale = 2.0f;

    private Vector2 originalContentSize;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (content != null)
        {
            originalContentSize = content.sizeDelta;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount == 2 && content != null)
        {
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            Vector2 prevTouchZero = touchZero.position - touchZero.deltaPosition;
            Vector2 prevTouchOne = touchOne.position - touchOne.deltaPosition;

            float prevMagnitude = (prevTouchZero - prevTouchOne).magnitude;
            float currentMagnitude = (touchZero.position - touchOne.position).magnitude;
            float difference = currentMagnitude - prevMagnitude;
            float currentScale = content.localScale.x;
            float newScale = Mathf.Clamp(currentScale + difference * zoomSpeed, minScale, maxScale);

            content.localScale = new Vector3(newScale, newScale, 1f);
            adjustScrollPosition(newScale);
        }
    }

    private void adjustScrollPosition(float newScale)
    {
        if (scrollRect != null)
        {
            Vector2 normalizedPosition = scrollRect.normalizedPosition;
            scrollRect.content.sizeDelta = originalContentSize * newScale;
            scrollRect.normalizedPosition = normalizedPosition;
        }
    }
}
