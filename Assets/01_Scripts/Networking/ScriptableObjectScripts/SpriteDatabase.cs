using System.Collections.Generic;
using UnityEngine;

namespace _01_Scripts.Networking.ScriptableObjectScripts
{
    [CreateAssetMenu(fileName = "SpriteDatabase", menuName = "ScriptableObjects/Sprite Database")]
    public class SpriteDatabase : ScriptableObject
    {
        public List<SpriteEntry> Entries;


        public Sprite GetSpriteById(string id)
        {
            foreach (var entry in Entries )
            {
                if (entry.id == id)
                    return entry.sprite;
            }

            return null;
        }
    }
}