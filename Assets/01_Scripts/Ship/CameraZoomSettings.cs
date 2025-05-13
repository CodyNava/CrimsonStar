using _01_Scripts.Lib;
using UnityEngine;

namespace _01_Scripts.Ship
{
    [CreateAssetMenu(fileName = "CameraZoomSettings", menuName = "Settings/CameraZoom")]
    public class CameraZoomSettings : ScriptableObject
    {
        [Tooltip("Sets the closes distances zoomed in in Unity Units ")]
        [AbsoluteValue] [field:SerializeField] public float MinDistance { get; private set; }
        [Tooltip("Sets the farthest distance zoomed out in Unity Units")]
        [AbsoluteValue] [field:SerializeField] public float MaxDistance { get; private set; }
        [Tooltip("Sets the general zoom speed")]
        [AbsoluteValue] [field:SerializeField] public float ZoomSpeedFactor { get; private set; }
        [Tooltip("This enables exponential zoom speed. For example the zoom speed is faster if zoomed far out as if it is very zoomed in")]
        [field:SerializeField] public bool ExpZoomSpeed { get; private set; }
        [Tooltip("Sets the minimum zoom speed factor even if zoomed very closely")]
        [AbsoluteValue] [field:SerializeField] public float ExpZoomMinSpeed { get; private set; }
        
        
    }
}