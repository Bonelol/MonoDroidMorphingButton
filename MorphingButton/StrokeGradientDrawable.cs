using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Graphics;
using Android.Graphics.Drawables;

namespace MorphingButton
{
    public class StrokeGradientDrawable
    {
        private int _strokeWidth;
        private int _strokeColor;
        private float _radius;
        private int _color;

        public int StrokeWidth
        {
            get
            {
                return _strokeWidth;
            }
            set
            {
                _strokeWidth = value;
                GradientDrawable.SetStroke(value, new Color(_strokeColor));
            }
        }

        public int StrokeColor
        {
            get
            {
                return _strokeColor;
            }
            set
            {
                _strokeColor = value;
                GradientDrawable.SetStroke(_strokeWidth, new Color(value));
            }
        }

        public float Radius
        {
            get
            {
                return _radius;
            }
            set
            {
                _radius = value;
                GradientDrawable.SetCornerRadius(value);
            }
        }

        public GradientDrawable GradientDrawable { get; }

        public int Color
        {
            get
            {
                return _color;
            }
            set
            {
                _color = value;
                GradientDrawable.SetColor(value);
            }
        }

        public StrokeGradientDrawable(GradientDrawable drawable)
        {
            this.GradientDrawable = drawable;
        }
    }
}
