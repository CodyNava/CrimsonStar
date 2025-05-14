using UnityEngine;

public class HexTransform : MonoBehaviour
{
    [SerializeField] private float scale = 1f;

    public HexLayout Layout => new(HexOrientation.Flat, Vector2.one * scale, transform.position.xy());

    private void OnDrawGizmos()
    {
        var layout = Layout;
        for (int q = -3; q <= 3; q++)
        {
            for (int r = -3; r <= 3; r++)
            {
                HexCoordinate coord = new(q, r);
                layout.DrawGizmos(coord, Color.green);
            }
        }
    }
}
