using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Editor
{
    [CreateAssetMenu(fileName = "FavoritesWindowSettings", menuName = "Data/Favorites WindowSettings", order = 0)]
    public class FavoritesWindSettings : ScriptableObject
    {
        public int topPadding = 12;
        public int EntryHeight = 28;
        public int EntryWidth = 185;
        public int EntrySpacing = 3;
        public float RemoveBtnWidthPercent = 0.15f;
        public Color RemoveButtonColor = Color.red;
    }
}