using UnityEngine;

namespace MyFramework
{
    public class Cell
    {
        public Vector3 CenterPosition { get; set; }
        public bool IsMoveable { get; private set; }
    }
}