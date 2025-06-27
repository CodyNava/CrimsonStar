using System;
using UnityEngine;

public class EnergySystemReactorRange : MonoBehaviour
{
    [SerializeField] private float _radius = 7f;
    [SerializeField] LineRenderer _lr;
    [SerializeField] NetEditorModule module;
    public int segments = 64;

    public void Start()
    {
        _lr = GetComponent<LineRenderer>();
    }
    public void Update()
    {
        if (module.EnergyViewEnable())
            InvokeDraw();
        else if (_lr.enabled)
        {
            _lr.enabled = false;
        }
    }
    public void InvokeDraw()
    {
        _lr.enabled = true;
        _radius = module.ModuleData.EffectRange * 3.5f;
        DrawCircle(_radius, _lr);
    }

    private void DrawCircle(float radius, LineRenderer lr)
    {
        float angleStep = 360f / segments;
        for (int i = 0; i < segments; i++)
        {
            float angle = Mathf.Deg2Rad * (angleStep * i);
            float x = Mathf.Cos(angle) * radius;
            float y = Mathf.Sin(angle) * radius;
            lr.SetPosition(i, new Vector3(x, y, -3));
        }
    }
}