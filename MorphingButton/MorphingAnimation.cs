using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace MorphingButton
{
    public class MorphingAnimation
    {
        public interface IListener
        {
            void OnAnimationEnd();
        }

        public class Params
        {
            private float _fromCornerRadius;
            private float _toCornerRadius;

            private int _fromHeight;
            private int _toHeight;

            private int _fromWidth;
            private int _toWidth;

            private int _fromColor;
            private int _toColor;

            private int _duration;

            private int _fromStrokeWidth;
            private int _toStrokeWidth;

            private int _fromStrokeColor;
            private int _toStrokeColor;

            private MorphingButton _button;
            private IListener _animationListener;

            private Params(MorphingButton button)
            {
                _button = button;
            }

            public static Params Create(MorphingButton button)
            {
                return new Params(button);
            }

            public Params Duration(int duration)
            {
                _duration = duration;
                return this;
            }

            public Params Listener(IListener animationListener)
            {
                _animationListener = animationListener;
                return this;
            }

            public Params Color(int fromColor, int toColor)
            {
                _fromColor = fromColor;
                _toColor = toColor;

                return this;
            }

            public Params CornerRadius(int fromRadius, int toRadius)
            {
                _fromCornerRadius = fromRadius;
                _toCornerRadius = toRadius;
                return this;
            }

            public Params Height(int fromHeight, int toHeight)
            {
                _fromHeight = fromHeight;
                _toHeight = toHeight;
                return this;
            }

            public Params Width(int fromWidth, int toWidth)
            {
                _fromWidth = fromWidth;
                _toWidth = toWidth;
                return this;
            }

            public Params StrokeWidth(int fromStrokeWidth, int toStrokeWidth)
            {
                _fromStrokeWidth = fromStrokeWidth;
                _toStrokeWidth = toStrokeWidth;
                return this;
            }

            public Params StrokeColor(int fromStrokeColor, int toStrokeColor)
            {
                _fromStrokeColor = fromStrokeColor;
                _toStrokeColor = toStrokeColor;
                return this;
            }
        }

        private Params _params;

        public MorphingAnimation(Params p)
        {
            _params = p;
        }

        public void Start()
        {
            
        }
    }
}