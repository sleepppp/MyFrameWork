using UnityEngine;
using UnityEngine.UI;

namespace MyFramework.UI
{
    public static class RectUtil
    {
        public struct PRect
        {
            public float Left;
            public float Top;
            public float Right;
            public float Bottom;

            public float Width { get { return Right - Left; } }
            public float Height { get { return Top - Bottom; } }

            public PRect(float left, float top, float right, float bottom)
            {
                Left = left;
                Top = top;
                Right = right;
                Bottom = bottom;
            }

            public PRect(Rect rect)
            {
                Left = rect.x;
                Right = rect.x + rect.width;
                Top = rect.y + rect.height;
                Bottom = rect.y;
            }

            public PRect(RectTransform rectTransform)
            {
                //rectTramsform.GetCorners랑 같은 결과값이나 gc발생 안하게 하려고 이렇게 처리합니다
                float scaleFactor = Game.UIManager.MainCanvas.scaleFactor;
                
                var size = new Vector2(rectTransform.rect.width, rectTransform.rect.height) * scaleFactor;
                Rect rect = new Rect
                    (
                    rectTransform.position.x - size.x * rectTransform.pivot.x,
                    rectTransform.position.y - size.y * rectTransform.pivot.y,
                    size.x,
                    size.y
                    );
                PRect PRect = new PRect(rect);
                Left = PRect.Left;
                Right = PRect.Right;
                Top = PRect.Top;
                Bottom = PRect.Bottom;
            }
        }

        public static bool IsCollision(Rect aRect, Rect bRect)
        {
            PRect aPRect = new PRect(aRect);
            PRect bPRect = new PRect(bRect);

            return IsCollision(aPRect, bPRect);
        }

        public static bool IsCollision(PRect aRect, PRect bRect)
        {
            if (aRect.Right < bRect.Left)
            {
                return false;
            }
            if (aRect.Left > bRect.Right)
            {
                return false;
            }
            if (aRect.Top < bRect.Bottom)
            {
                return false;
            }
            if (aRect.Bottom > bRect.Top)
            {
                return false;
            }

            return true;
        }

        public static PRect GetPRect(this RectTransform rectTransform)
        {
            return new PRect(rectTransform);
        }

        public static bool IsCollision(this RectTransform rectTransform, Rect rect)
        {
            return IsCollision(rectTransform.GetPRect(), new PRect(rect));
        }

        public static bool IsCollision(this RectTransform rectTransform, PRect rect)
        {
            return IsCollision(rectTransform.GetPRect(), rect);
        }

        public static bool IsCollision(this RectTransform rectTransform, RectTransform other)
        {
            return IsCollision(rectTransform.GetPRect(), other.GetPRect());
        }

        //rect안으로 rectTransform을 밀어 보정합니다. SafeArea밖에 나가면 안되는 UI의 경우 해당 매서드 사용
        public static bool ReviseTransformInRect(this RectTransform rectTransform, Rect rect)
        {
            PRect aRect = rectTransform.GetPRect();
            PRect bRect = new PRect(rect);

            if (aRect.Left < bRect.Left)
            {
                rectTransform.Translate(new Vector2(bRect.Left - aRect.Left, 0f));
            }
            if (aRect.Right > bRect.Right)
            {
                rectTransform.Translate(-new Vector2(aRect.Right - bRect.Right, 0f));
            }
            if (aRect.Bottom < bRect.Bottom)
            {
                rectTransform.Translate(new Vector2(0f, bRect.Bottom - aRect.Bottom));
            }
            if (aRect.Top > bRect.Top)
            {
                rectTransform.Translate(-new Vector2(0f, aRect.Top - bRect.Top));
            }

            return IsCollision(aRect, bRect);
        }
    }
}
