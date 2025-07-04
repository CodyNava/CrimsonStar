using UnityEngine;

public class ButtonTransformBehaviour : MonoBehaviour
{
    [SerializeField] private float sizeIncrease;
    [SerializeField] private Vector2 rectVector;
    [SerializeField] private RectTransform rectTransform;
    private bool _hovered;
    
    private void Awake()
    {
        rectTransform = gameObject.GetComponent<RectTransform>();
        rectVector = new Vector2(rectTransform.sizeDelta.x, rectTransform.sizeDelta.y);
        UnHover();
    }

    public void Hover()
    {
        if (!_hovered)
        {
            rectVector.x *= sizeIncrease;
            rectTransform.sizeDelta = rectVector;
            _hovered = true;
        }
    }

    public void UnHover()
    {
        if (_hovered)
        {
            rectVector.x /= sizeIncrease;
            rectTransform.sizeDelta = rectVector;
            _hovered = false;
        }
    }
}