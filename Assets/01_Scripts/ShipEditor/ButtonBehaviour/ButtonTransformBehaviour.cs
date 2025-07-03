using UnityEngine;

public class ButtonTransformBehaviour : MonoBehaviour
{
    [SerializeField] private float sizeIncrease;
    [SerializeField] private Vector2 rectVector;
    [SerializeField] private RectTransform rectTransform;
    
    private void Awake()
    {
        rectTransform = gameObject.GetComponent<RectTransform>();
        rectVector = new Vector2(rectTransform.sizeDelta.x, rectTransform.sizeDelta.y);
        UnHover();
    }

    public void Hover()
    {
        rectVector.x *= sizeIncrease;
        rectTransform.sizeDelta = rectVector;
    }

    public void UnHover()
    {
        rectVector.x /= sizeIncrease;
        rectTransform.sizeDelta = rectVector;
    }
}