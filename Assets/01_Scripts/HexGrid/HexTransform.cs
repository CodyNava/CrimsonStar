using UnityEngine;

public class HexTransform : MonoBehaviour
{
    [SerializeField] private float scale = 1f;
    [SerializeField, Min(1)] private int gizmosRange = 3;
    [SerializeField] private float gizmosScale = 0.9f;
    [SerializeField] private Color gizmosColor = Color.green;

    public HexLayout Layout => new(HexOrientation.Flat, Vector2.one * scale, transform.position.xy());

    private void OnDrawGizmos()
    {
        var layout = Layout;
        HexCoordinate origin = HexCoordinate.Zero;
        foreach (HexCoordinate cell in origin.CoordinatesInRange(gizmosRange))
        {
            layout.DrawGizmos(cell, gizmosColor, gizmosScale);
        }
    }
}
