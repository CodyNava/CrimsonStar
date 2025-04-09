using UnityEngine;

namespace _01_Scripts.AI.SimpleAI
{
    [CreateAssetMenu(fileName = "SimpleAIObject", menuName = "SimpleAI/SimpleAIObject", order = 1)]
    public class SimpleAIParameters : ScriptableObject
    {
        public float wanderDistanceMax;
        public float wanderDistanceMin;
        public float wanderReachDistanceMax;
        public float wanderReachDistanceMin;
        public float perceptionDistance;
        public float attackDistance;
        public float safeDistance;
    }
}