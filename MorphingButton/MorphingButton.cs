using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace MorphingButton
{
    public class MorphingButton : Button
    {
        #region Fields

        private bool _animationInProgress;
        private int _color;
        private int _cornerRadius;
        private int _height;
        private Padding _padding;
        private int _strokeColor;
        private int _strokeWidth;
        private int _width;

        #endregion

        #region Constructors

        public MorphingButton(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public MorphingButton(Context context) : base(context)
        {
            InitView();
        }

        public MorphingButton(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            InitView();
        }

        public MorphingButton(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
            InitView();
        }

        public MorphingButton(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes)
        {
        }

        #endregion

        #region Properties

        public StrokeGradientDrawable DrawableNormal { get; private set; }

        public StrokeGradientDrawable DrawablePressed { get; private set; }

        public bool AnimationInProgress => _animationInProgress;

        #endregion

        #region Methods

        public void BlockTouch()
        {
            this.SetOnTouchListener(new BlockOnTouchListener());
        }

        public void InitView()
        {
            _padding = new Padding
            {
                left = this.PaddingLeft,
                top = this.PaddingTop,
                right = this.PaddingRight,
                bottom = this.PaddingBottom
            };

            var resources = this.Resources;

            int radius = (int) resources.GetDimension(Resource.Dimension.mb_corner_radius_2);
            int blue = Resources.GetColor(Resource.Color.mb_blue);
            int blueDark = Resources.GetColor(Resource.Color.mb_blue_dark);

            var background = new StateListDrawable();
            DrawableNormal = CreateDrawable(blue, radius, 0);
            DrawablePressed = CreateDrawable(blueDark, radius, 0);

            _color = blue;
            _strokeColor = blue;
            _cornerRadius = radius;

            background.AddState(new int[] {Android.Resource.Attribute.StatePressed}, DrawablePressed.GradientDrawable);
            background.AddState(StateSet.WildCard.ToArray(), DrawableNormal.GradientDrawable);
        }

        public virtual void Morph(Params p)
        {
            if (!_animationInProgress)
            {
                DrawablePressed.Color = p.ColorPressed;
                DrawablePressed.Radius = p.CornerRadius;
                DrawablePressed.StrokeColor = p.StrokeColor;
                DrawablePressed.StrokeWidth = p.StrokeWidth;
            }

            if (p.Duration == 0)
            {
                MorphWithoutAnimation(p);
            }
            else
            {
                MorphWithAnimation(p);
            }

            _color = p.Color;
            _cornerRadius = p.CornerRadius;
            _strokeWidth = p.StrokeWidth;
            _strokeColor = p.StrokeColor;
        }

        public void SetIcon(int icon)
        {
            this.Post(() =>
            {
                var drawable = this.Resources.GetDrawable(icon);
                int padding = Width/2 - drawable.IntrinsicWidth/2;
                SetCompoundDrawablesWithIntrinsicBounds(icon, 0, 0, 0);
                SetPadding(padding, 0, 0, 0);
            });
        }

        public void SetIconLeft(int icon)
        {
            SetCompoundDrawablesWithIntrinsicBounds(icon, 0, 0, 0);
        }

        public void UnblockTouch()
        {
            this.SetOnTouchListener(new UnblockOnTouchListener());
        }

        protected override void OnSizeChanged(int w, int h, int oldw, int oldh)
        {
            base.OnSizeChanged(w, h, oldw, oldh);

            if (_height == 0 && _width == 0 && w != 0 && h != 0)
            {
                _height = this.Height;
                _width = this.Width;
            }
        }

        private StrokeGradientDrawable CreateDrawable(int color, int radius, int width)
        {
            var drawable = new StrokeGradientDrawable(new GradientDrawable())
            {
                Color = color,
                Radius = radius,
                StrokeColor = color,
                StrokeWidth = width
            };

            drawable.GradientDrawable.SetShape(ShapeType.Rectangle);

            return drawable;
        }

        private void FinalizeMorphing(Params @params)
        {
            _animationInProgress = false;

            if (@params.Icon != 0 && @params.Text != null)
            {
                SetIconLeft(@params.Icon);
                Text = @params.Text;
            }
            else if (@params.Icon != 0)
            {
                SetIcon(@params.Icon);
            }
            else if (@params.Text != null)
            {
                SetText(@params.Text, BufferType.Normal);
            }
        }

        private void MorphWithAnimation(Params @params)
        {
            _animationInProgress = true;

            Text = null;
            SetCompoundDrawablesWithIntrinsicBounds(0, 0, 0, 0);
            SetPadding(_padding.left, _padding.top, _padding.right, _padding.bottom);

            var animationParams = MorphingAnimation.Params.Create(this)
                .Color(_color, @params.Color)
                .CornerRadius(_cornerRadius, @params.CornerRadius)
                .StrokeWidth(_strokeWidth, @params.StrokeWidth)
                .StrokeColor(_strokeColor, @params.StrokeColor)
                .Height(_height, @params.Height)
                .Width(_width, @params.Width)
                .Duration(@params.Duration)
                .Listener(new MorphingAnimationListener(() => { FinalizeMorphing(@params); }));

            var animation = new MorphingAnimation(animationParams);
            animation.Start();
        }

        private void MorphWithoutAnimation(Params @params)
        {
            DrawableNormal.Color = @params.Color;
            DrawableNormal.Radius = @params.CornerRadius;
            DrawableNormal.StrokeColor = @params.StrokeColor;
            DrawableNormal.StrokeWidth = @params.StrokeWidth;

            if (@params.Width != 0 && @params.Height != 0)
            {
                var layoutParams = this.LayoutParameters;
                layoutParams.Width = @params.Width;
                layoutParams.Height = @params.Height;

                this.LayoutParameters = layoutParams;
            }

            FinalizeMorphing(@params);
        }

        #endregion

        #region Nested Types

        public class Params
        {
            #region Constructors

            private Params()
            {
            }

            #endregion

            #region Properties

            public MorphingAnimation.IListener AnimationListener { get; set; }
            public int Color { get; set; }
            public int ColorPressed { get; set; }
            public int CornerRadius { get; set; }
            public int Duration { get; set; }
            public int Height { get; set; }
            public int Icon { get; set; }
            public int StrokeColor { get; set; }
            public int StrokeWidth { get; set; }
            public string Text { get; set; }
            public int Width { get; set; }

            #endregion

            #region Methods

            public static Params Create()
            {
                return new Params();
            }

            public Params SetAnimationListener(MorphingAnimation.IListener listener)
            {
                AnimationListener = listener;
                return this;
            }

            public Params SetColor(int color)
            {
                Color = color;
                return this;
            }

            public Params SetColorPressed(int color)
            {
                ColorPressed = color;
                return this;
            }

            public Params SetCornerRadius(int radius)
            {
                CornerRadius = radius;
                return this;
            }

            public Params SetDuration(int duration)
            {
                Duration = duration;
                return this;
            }

            public Params SetHeight(int height)
            {
                Height = height;
                return this;
            }

            public Params SetIcon(int icon)
            {
                Icon = icon;
                return this;
            }

            public Params SetStrokeColor(int color)
            {
                StrokeColor = color;
                return this;
            }

            public Params SetStrokeWidth(int strokeWidth)
            {
                StrokeWidth = strokeWidth;
                return this;
            }

            public Params SetText(string text)
            {
                Text = text;
                return this;
            }

            public Params SetWidth(int width)
            {
                Width = width;
                return this;
            }

            #endregion
        }

        private class MorphingAnimationListener : Java.Lang.Object, MorphingAnimation.IListener
        {
            #region Fields

            private readonly Action run;

            #endregion

            #region Constructors

            public MorphingAnimationListener(Action action)
            {
                run = action;
            }

            #endregion

            #region Methods

            public void OnAnimationEnd()
            {
                run?.Invoke();
            }

            #endregion
        }


        private class Padding
        {
            #region Fields

            public int bottom;
            public int left;
            public int right;
            public int top;

            #endregion
        }

        private class BlockOnTouchListener : Java.Lang.Object, IOnTouchListener
        {
            #region Methods

            public bool OnTouch(View v, MotionEvent e)
            {
                return true;
            }

            #endregion
        }

        private class UnblockOnTouchListener : Java.Lang.Object, IOnTouchListener
        {
            #region Methods

            public bool OnTouch(View v, MotionEvent e)
            {
                return false;
            }

            #endregion
        }

        #endregion
    }
}