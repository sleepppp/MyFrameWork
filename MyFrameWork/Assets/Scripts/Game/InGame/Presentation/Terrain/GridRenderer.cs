using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyFramework
{
    public class GridRenderer : MonoBehaviour
    {
        [SerializeField] MeshRenderer _meshRenderer;
        [SerializeField] MeshFilter _meshFilter;
        [SerializeField] Material _material;
        public void ActiveRender(bool isRender)
        {
            gameObject.SetActive(isRender);
        }
    }
}
