using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyFramework
{
    public class GridRenderer : MonoBehaviour
    {
        public MeshRenderer MeshRenderer;
        public MeshFilter MeshFilter;
        public Material Material;
        public void ActiveRender(bool isRender)
        {
            gameObject.SetActive(isRender);
        }

        public void SetScale(float scale)
        {
            Material.SetFloat("_Scale", scale);
        }

        public void SetGraduationScaleX(float scale)
        {
            Material.SetFloat("_GraduationScaleX", scale);
        }

        public void SetGraduationScaleY(float scale)
        {
            Material.SetFloat("_GraduationScaleY", scale);
        }

        public void SetThickness(float thickness)
        {
            Material.SetFloat("_Thickness", thickness);
        }

        public void SetMainColor(Color color)
        {
            Material.SetColor("_MainColor", color);
        }

        public void SetSecondaryColor(Color color)
        {
            Material.SetColor("_SecondaryColor", color);
        }

    }
}
