using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyFramework.UI
{
    using PRect = RectUtil.PRect;
    public class UIBase : MyMonoBehaviour
    {
        public enum SafeAreaActionType : int
        {
            Anchor,
            PushRect
        }
        [Header("SafeArea")]
        public SafeAreaActionType SafeAreaActionTypeEnum;
        public bool ConformX = true;
        public bool ConformY = true;
        public RectTransform SafeAreaTransform;

        public void ApplySafeArea()
        {
            Rect safeArea = Game.UIManager.SafeArea;

            switch(SafeAreaActionTypeEnum)
            {
                case SafeAreaActionType.Anchor:
                    ApplySafeAreaByAnchor(safeArea);
                    break;
                case SafeAreaActionType.PushRect:
                    ApplySafeAreaByPushRect(safeArea);
                    break;
            }
        }

        void ApplySafeAreaByAnchor(Rect safeArea)
        {
            var rectTm = SafeAreaTransform;
            if (rectTm == null)
                rectTm = this.transform as RectTransform;
            // Ignore x-axis?
            if (!ConformX)
            {
                safeArea.x = 0;
                safeArea.width = Screen.width;
            }

            // Ignore y-axis?
            if (!ConformY)
            {
                safeArea.y = 0;
                safeArea.height = Screen.height;
            }

            // Check for invalid screen startup state on some Samsung devices (see below)
            if (Screen.width > 0 && Screen.height > 0)
            {
                // Convert safe area rectangle from absolute pixels to normalised anchor coordinates
                Vector2 anchorMin = safeArea.position;
                Vector2 anchorMax = safeArea.position + safeArea.size;
                anchorMin.x /= Screen.width;
                anchorMin.y /= Screen.height;
                anchorMax.x /= Screen.width;
                anchorMax.y /= Screen.height;

                if (anchorMin.x >= 0 && anchorMin.y >= 0 && anchorMax.x >= 0 && anchorMax.y >= 0)
                {
                    rectTm.anchorMin = anchorMin;
                    rectTm.anchorMax = anchorMax;
                }
            }
        }

        void ApplySafeAreaByPushRect(Rect safeArea)
        {
            PRect myRect = new PRect(SafeAreaTransform);
            PRect safeAreaRect = new PRect(safeArea);

            if(myRect.Width > safeAreaRect.Width || 
                myRect.Height > safeAreaRect.Height)
            {
                Debug.Log("Can't apply safeArea. Cause rect is more huge then safeArea");
                return;
            }

            SafeAreaTransform.ReviseTransformInRect(safeArea);
        }
    }
}
