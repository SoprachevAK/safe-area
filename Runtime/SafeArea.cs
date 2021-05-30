using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace AS.SafeArea
{
    [ExecuteAlways]
    public class SafeArea : MonoBehaviour
    {
        public Variant variant;
        public bool left = true;
        public bool right = true;
        public bool top = true;
        public bool bottom = true;
        public FloatRectOffset padding, minBorder;


        RectTransform rectTransform;
        LayoutGroup targetLayoutGroup;

        Rect safeArea = new Rect();
        Vector2 screenSize = Vector2.zero;
        ScreenOrientation screenOrientation = ScreenOrientation.AutoRotation;

        FloatRectOffset lastPadding, lastMinBorder;
        bool lastLeft, lastRight, lastTop, lastBottom;
        Variant lastVariant;

        public enum Variant
        {
            disable,
            anchorPosition,
            padding
        }

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            targetLayoutGroup = GetComponent<LayoutGroup>();
        }

        void Update()
        {
            Refresh();
        }

        void Refresh()
        {
            Rect currentSafeArea = Screen.safeArea;

            if (currentSafeArea != safeArea ||
                Screen.width != screenSize.x ||
                Screen.height != screenSize.y ||
                Screen.orientation != screenOrientation || Changed())
            {
                screenSize.x = Screen.width;
                screenSize.y = Screen.height;
                lastPadding = new FloatRectOffset(padding);
                lastMinBorder = new FloatRectOffset(minBorder);

                lastLeft = left;
                lastRight = right;
                lastTop = top;
                lastBottom = bottom;

                lastVariant = variant;

                screenOrientation = Screen.orientation;

                safeArea = currentSafeArea;

                Rect r = ModifySafeArea(safeArea);
                ApplySafeArea(r);

            }
        }

        bool Changed()
        {
            if (lastBottom != bottom || lastLeft != left || lastRight != right || lastTop != top) return true;
            if (!lastPadding.Equals(padding) || !lastMinBorder.Equals(minBorder)) return true;
            if (lastVariant != variant) return true;

            return false;
        }

        Rect ModifySafeArea(Rect r)
        {

            Vector2 size = GetComponentInParent<Canvas>().GetComponent<RectTransform>().sizeDelta;


            float xScale = size.x / (1.0f * Screen.width);
            float yScale = size.y / (1.0f * Screen.height);

            r.yMax = Mathf.Min(r.yMax, Screen.height - minBorder.top / yScale) - padding.top / yScale;
            r.yMin = Mathf.Max(r.yMin, minBorder.bottom / yScale) + padding.bottom / yScale;
            r.xMax = Mathf.Min(r.xMax, Screen.width - minBorder.right / yScale) - padding.right / yScale;
            r.xMin = Mathf.Max(r.xMin, minBorder.left / yScale) + padding.left / yScale;

            return r;
        }

        void ApplySafeArea(Rect r)
        {
            if (!top)
                r.yMax = Screen.height;

            if (!bottom)
                r.yMin = 0;

            if (!right)
                r.xMax = Screen.width;

            if (!left)
                r.xMin = 0;

            if (variant == Variant.anchorPosition)
            {
                SetRectAnchored(r);
            }
            else if (variant == Variant.padding)
            {
                SetRectPadding(r);
            }
        }

        void SetRectAnchored(Rect r)
        {
            if (!rectTransform) rectTransform = GetComponent<RectTransform>();

            Vector2 anchorMin = r.position;
            Vector2 anchorMax = r.position + r.size;

            anchorMin.x /= Screen.width;
            anchorMin.y /= Screen.height;
            anchorMax.x /= Screen.width;
            anchorMax.y /= Screen.height;

            rectTransform.anchorMin = anchorMin;
            rectTransform.anchorMax = anchorMax;
        }

        void SetRectPadding(Rect r)
        {
            if (!targetLayoutGroup) targetLayoutGroup = GetComponent<LayoutGroup>();
            if (!targetLayoutGroup)
            {
                this.enabled = false;
                throw new System.Exception("LayoutGroup not found");
            }


            Vector2 size = GetComponentInParent<Canvas>().GetComponent<RectTransform>().sizeDelta;


            float xScale = size.x / (1.0f * Screen.width);
            float yScale = size.y / (1.0f * Screen.height);

            targetLayoutGroup.padding.top = Mathf.RoundToInt((Screen.height - r.yMax) * xScale);
            targetLayoutGroup.padding.bottom = Mathf.RoundToInt(r.yMin * yScale);
            targetLayoutGroup.padding.left = Mathf.RoundToInt(r.xMin * xScale);
            targetLayoutGroup.padding.right = Mathf.RoundToInt((Screen.width - r.xMax) * xScale);

            LayoutRebuilder.MarkLayoutForRebuild(rectTransform);

        }
    }
}