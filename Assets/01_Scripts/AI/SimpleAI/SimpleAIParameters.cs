using System;
using _01_Scripts.Lib;
using UnityEngine;

namespace _01_Scripts.AI.SimpleAI
{
    [CreateAssetMenu(fileName = "SimpleAIObject", menuName = "SimpleAI/SimpleAIObject", order = 1)]
    public class SimpleAIParameters : ScriptableObject
    {
        [AbsoluteValue]
        public float wanderDistance;
        [AbsoluteValue]
        public float wanderDistanceVariance;
        [AbsoluteValue]
        public float wanderReachDistance;
        [AbsoluteValue]
        public float wanderReachDistanceVariance;
        [AbsoluteValue]
        public float perceptionDistance;
        [AbsoluteValue]
        public float attackDistance;
        [AbsoluteValue]
        public float safeDistance;

        public float wanderDistanceMax => wanderDistance + wanderDistanceVariance / 2f;
        public float wanderDistanceMin => Mathf.Max(wanderDistance - wanderDistanceVariance / 2f, 0f);
        
        public float wanderReachDistanceMax => wanderReachDistance + wanderReachDistanceVariance / 2f;
        public float wanderReachDistanceMin => Mathf.Max(wanderReachDistance - wanderReachDistanceVariance / 2f, 0f);
    }

    
}