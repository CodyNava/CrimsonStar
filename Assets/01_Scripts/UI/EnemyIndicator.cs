using UnityEngine;

public class EnemyIndicator : MonoBehaviour
{

    public Camera mainCamera;
    public GameObject indicatorPrefab;
    private RectTransform indicatorUI;
    private Transform player;
    private Canvas canvas;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        mainCamera = Camera.main;

        canvas = FindFirstObjectByType<Canvas>();
        GameObject indicatorInstance = Instantiate(indicatorPrefab, canvas.transform);
        indicatorUI = indicatorInstance.GetComponent<RectTransform>();
    }

    void Update()
    {
        Vector3 screenPos = mainCamera.WorldToViewportPoint(transform.position);

        bool isOffScreen = screenPos.x < 0 || screenPos.x > 1 || screenPos.y < 0 || screenPos.y > 1;

        indicatorUI.gameObject.SetActive(isOffScreen);

        if (isOffScreen)
        {
            Vector3 screenCenter = new Vector3(0.5f, 0.5f, screenPos.z);
            Vector3 fromCenterToEnemy = screenPos - screenCenter;

            fromCenterToEnemy.z = 0;
            fromCenterToEnemy.Normalize();

            float angle = Mathf.Atan2(fromCenterToEnemy.y, fromCenterToEnemy.x) * Mathf.Rad2Deg;
            indicatorUI.rotation = Quaternion.Euler(0, 0, angle + 90f);

            Vector3 cappedScreenPos = screenCenter + fromCenterToEnemy * 0.45f;
            Vector3 worldPos = mainCamera.ViewportToScreenPoint(cappedScreenPos);
            indicatorUI.position = worldPos;
        }
    }

    private void OnDestroy()
    {
        if (indicatorUI != null)
            Destroy(indicatorUI.gameObject);
    }

}
