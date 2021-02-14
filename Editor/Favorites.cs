using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Editor
{
    [System.Serializable]
    public class Favorites 
    {
        public List<string> Paths;
        public List<Texture> Icons;
        public List<string> Names;
        public Favorites()
        {
            Paths = new List<string>();
            Icons = new List<Texture>();
            Names = new List<string>();
        }
    }
}