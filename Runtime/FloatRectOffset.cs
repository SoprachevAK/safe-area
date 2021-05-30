

namespace AS.SafeArea
{
    [System.Serializable]
    public class FloatRectOffset
    {

        public float left;
        public float right;
        public float top;
        public float bottom;

        public float horizontal
        {
            get
            {
                return left + right;
            }
        }
        public float vertical
        {
            get
            {
                return top + bottom;
            }
        }

        public FloatRectOffset(float left, float right, float top, float bottom)
        {
            this.left = left;
            this.right = right;
            this.top = top;
            this.bottom = bottom;
        }

        public FloatRectOffset(FloatRectOffset floatRectOffset)
        {
            left = floatRectOffset.left;
            right = floatRectOffset.right;
            top = floatRectOffset.top;
            bottom = floatRectOffset.bottom;
        }

        public override bool Equals(object obj)
        {
            return obj is FloatRectOffset offset &&
                   left == offset.left &&
                   right == offset.right &&
                   top == offset.top &&
                   bottom == offset.bottom;
        }
    }
}