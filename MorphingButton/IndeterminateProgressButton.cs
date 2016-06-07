using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.View;
using Android.Util;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;

namespace MorphingButton
{
    public class IndeterminateProgressButton : MorphingButton
    {
        private int _color1;
        private int _color2;
        private int _color3;
        private int _color4;
        private int _progressCornerRadius;
        private ProgressBar _progressBar;

        private bool _isRunning;

        public IndeterminateProgressButton(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public IndeterminateProgressButton(Context context) : base(context)
        {
            Init(context);
        }

        public IndeterminateProgressButton(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            Init(context);
        }

        public IndeterminateProgressButton(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
            Init(context);
        }

        public IndeterminateProgressButton(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes)
        {
        }

        private void Init(Context context)
        {
            var res = context.Resources;

            _color1 = res.GetColor(Resource.Color.mb_gray);
            _color2 = res.GetColor(Resource.Color.mb_blue);
            _color3 = res.GetColor(Resource.Color.mb_gray);
            _color4 = res.GetColor(Resource.Color.mb_blue);

            if (Build.VERSION.SdkInt <= BuildVersionCodes.JellyBeanMr2)
            {
                SetLayerType(LayerType.Software, null);
            }
        }

        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);

            if (!AnimationInProgress && _isRunning)
            {
                if (_progressBar == null)
                {
                    _progressBar = new ProgressBar(this);
                    SetupProgressBarBounds();
                    _progressBar.SetColorScheme(_color1, _color2, _color3, _color4);
                    _progressBar.Start();
                }

                _progressBar.Draw(canvas);
            }
        }

        private void SetupProgressBarBounds()
        {
            double indicatorHeight = this.Height;
            int bottom = (int)(MeasuredHeight - indicatorHeight);
            _progressBar.SetBounds(0, bottom, MeasuredWidth, MeasuredHeight, _progressCornerRadius);
        }

        public override void Morph(Params p)
        {
            _isRunning = false;

            base.Morph(p);
        }

        public void MorphToProgress(int backgroundColor, int progressCornerRadius, int width, int height, int duration,
                                int progressColor1)
        {
            MorphToProgress(backgroundColor, progressCornerRadius, width, height, duration, backgroundColor, progressColor1,
                    backgroundColor, progressColor1);
        }

        public void MorphToProgress(int backgroundColor, int progressCornerRadius, int width, int height, int duration,
                                    int progressColor1, int progressColor2)
        {
            MorphToProgress(backgroundColor, progressCornerRadius, width, height, duration, progressColor1, progressColor2,
                    progressColor1, progressColor2);
        }

        public void MorphToProgress(int backgroundColor, int progressCornerRadius, int width, int height, int duration,
                                    int progressColor1, int progressColor2, int progressColor3, int progressColor4)
        {
            this._progressCornerRadius = progressCornerRadius;
            _color1 = progressColor1;
            _color2 = progressColor2;
            _color3 = progressColor3;
            _color4 = progressColor4;

            Params longRoundedSquare = Params.Create()
                .SetDuration(duration)
                .SetCornerRadius(progressCornerRadius)
                .SetWidth(width)
                .SetHeight(height)
                .SetColor(backgroundColor)
                .SetColorPressed(backgroundColor)
                .SetAnimationListener(new MorphingAnimationListener(() =>
                {
                    _isRunning = true;
                    Invalidate();
                }));
        Morph(longRoundedSquare);
}

        class MorphingAnimationListener : Java.Lang.Object, MorphingAnimation.IListener
        {
            private readonly Action _run;

            public MorphingAnimationListener(Action action)
            {
                _run = action;
            }

            public void OnAnimationEnd()
            {
                _run?.Invoke();
            }
        }

        public class ProgressBar
        {
            private const uint COLOR1 = 0xB3000000;
            private const uint COLOR2 = 0x80000000;
            private const uint COLOR3 = 0x4d000000;
            private const uint COLOR4 = 0x1a000000;

            private const int ANIMATION_DURATION_MS = 2000;
            private const int FINISH_ANIMATION_DURATION_MS = 1000;

            private readonly IInterpolator _interpolator = new AccelerateDecelerateInterpolator();

            private readonly Paint _paint = new Paint();
            private readonly RectF _rectF = new RectF();
            private float _triggerPercentage;
            private long _startTime;
            private long _finishTime;
            private bool _running;

            private uint _color1;
            private uint _color2;
            private uint _color3;
            private uint _color4;
            private int _cornerRadius;
            private readonly View _parent;

            private readonly RectF _bounds = new RectF();

            public ProgressBar(View parent)
            {
                _parent = parent;

                _color1 = COLOR1;
                _color2 = COLOR2;
                _color3 = COLOR3;
                _color4 = COLOR4;
            }

            public void SetColorScheme(uint color1, uint color2, uint color3, uint color4)
            {
                _color1 = color1;
                _color2 = color2;
                _color3 = color3;
                _color4 = color4;
            }

            public void SetColorScheme(int color1, int color2, int color3, int color4)
            {
                _color1 = (uint) color1;
                _color2 = (uint) color2;
                _color3 = (uint) color3;
                _color4 = (uint) color4;
            }

            public void Start()
            {
                if (!_running)
                {
                    _triggerPercentage = 0;
                    _startTime = AnimationUtils.CurrentAnimationTimeMillis();
                    _running = true;
                    _parent.PostInvalidate();
                }
            }

            public void Draw(Canvas canvas)
            {
                var clipPath = new Path();
                clipPath.AddRoundRect(_bounds, _cornerRadius, _cornerRadius, Path.Direction.Cw);

                int width = (int) _bounds.Width();
                int height = (int) _bounds.Height();
                int cx = width/2;
                int cy = height/2;

                bool drawTriggerWhileFinishing = false;
                int restoreCount = canvas.Save();
                canvas.ClipPath(clipPath);

                if (_running || _finishTime > 0)
                {
                    long now = AnimationUtils.CurrentAnimationTimeMillis();
                    long elapsed = (now - _startTime)%ANIMATION_DURATION_MS;
                    long iterations = (now - _startTime) / ANIMATION_DURATION_MS;
                    float rawProgress = (elapsed/(ANIMATION_DURATION_MS/100f));

                    if (!_running)
                    {
                        if ((now - _finishTime) >= FINISH_ANIMATION_DURATION_MS)
                        {
                            _finishTime = 0;
                            return;
                        }

                        // Otherwise, use a 0 opacity alpha layer to clear the animation
                        // from the inside out. This layer will prevent the circles from
                        // drawing within its bounds.
                        long finishEplapsed = (now - _finishTime) % FINISH_ANIMATION_DURATION_MS;
                        float finishProgress = (finishEplapsed / (FINISH_ANIMATION_DURATION_MS / 100f));
                        float pct = finishProgress / 100f;
                        float clearRadius = width / 2 * _interpolator.GetInterpolation(pct);
                        _rectF.Set(cx - clearRadius, 0, cx + clearRadius, height);
                        canvas.SaveLayerAlpha(_rectF, 0, 0);

                        // Only draw the trigger if there is a space in the center of
                        // this refreshing view that needs to be filled in by the
                        // trigger. If the progress view is just still animating, let it
                        // continue animating.
                        drawTriggerWhileFinishing = true;
                    }

                    // First fill in with the last color that would have finished drawing.
                    if (iterations == 0)
                    {
                        canvas.DrawColor(ColorUtils.FromUint(_color1));
                    }
                    else
                    {
                        if (rawProgress >= 0 && rawProgress < 25)
                        {
                            canvas.DrawColor(ColorUtils.FromUint(_color4));
                        }
                        else if (rawProgress >= 25 && rawProgress < 50)
                        {
                            canvas.DrawColor(ColorUtils.FromUint(_color1));
                        }
                        else if (rawProgress >= 50 && rawProgress < 75)
                        {
                            canvas.DrawColor(ColorUtils.FromUint(_color2));
                        }
                        else
                        {
                            canvas.DrawColor(ColorUtils.FromUint(_color3));
                        }
                    }

                    // Then draw up to 4 overlapping concentric circles of varying radii, based on how far
                    // along we are in the cycle.
                    // progress 0-50 draw mColor2
                    // progress 25-75 draw mColor3
                    // progress 50-100 draw mColor4
                    // progress 75 (wrap to 25) draw mColor1
                    if (rawProgress >= 0 && rawProgress <= 25)
                    {
                        float pct = (rawProgress + 25)*2/100f;
                        DrawCircle(canvas, cx, cy, _color1, pct);
                    }

                    if (rawProgress >= 0 && rawProgress <= 50)
                    {
                        float pct = ((rawProgress * 2) / 100f);
                        DrawCircle(canvas, cx, cy, _color2, pct);
                    }
                    if (rawProgress >= 25 && rawProgress <= 75)
                    {
                        float pct = (((rawProgress - 25) * 2) / 100f);
                        DrawCircle(canvas, cx, cy, _color3, pct);
                    }
                    if (rawProgress >= 50 && rawProgress <= 100)
                    {
                        float pct = (((rawProgress - 50) * 2) / 100f);
                        DrawCircle(canvas, cx, cy, _color4, pct);
                    }
                    if ((rawProgress >= 75 && rawProgress <= 100))
                    {
                        float pct = (((rawProgress - 75) * 2) / 100f);
                        DrawCircle(canvas, cx, cy, _color1, pct);
                    }

                    if (_triggerPercentage > 0 && drawTriggerWhileFinishing)
                    {
                        canvas.RestoreToCount(restoreCount);
                        restoreCount = canvas.Save();
                        canvas.ClipPath(clipPath);
                        DrawTrigger(canvas, cx, cy);
                    }

                    ViewCompat.PostInvalidateOnAnimation(_parent);
                }
                else if (_triggerPercentage > 0 && _triggerPercentage < 1.0)
                {
                    DrawTrigger(canvas, cx, cy);
                }

                canvas.RestoreToCount(restoreCount);
            }

            private void DrawTrigger(Canvas canvas, int cx, int cy)
            {
                _paint.Color = ColorUtils.FromUint(_color1);
                canvas.DrawCircle(cx, cy, cx * _triggerPercentage, _paint);
            }

            /**
            * Draws a circle centered in the view.
            *
            * @param canvas the canvas to draw on
            * @param cx the center x coordinate
            * @param cy the center y coordinate
            * @param color the color to draw
            * @param pct the percentage of the view that the circle should cover
            */
            private void DrawCircle(Canvas canvas, float cx, float cy, int color, float pct)
            {
                DrawCircle(canvas, cx, cy, new Color(color), pct);
            }

            private void DrawCircle(Canvas canvas, float cx, float cy, uint color, float pct)
            {
                DrawCircle(canvas, cx, cy, ColorUtils.FromUint(color), pct);
            }

            private void DrawCircle(Canvas canvas, float cx, float cy, Color color, float pct)
            {
                _paint.Color = color;
                canvas.Save();
                canvas.Translate(cx, cy);
                float radiusScale = this._interpolator.GetInterpolation(pct);
                canvas.Scale(radiusScale, radiusScale);
                canvas.DrawCircle(0, 0, cx, _paint);
                canvas.Restore();
            }



            public void SetBounds(int left, int top, int right, int bottom, int cornerRadius)
            {
                _bounds.Left = left;
                _bounds.Top = top;
                _bounds.Right = right;
                _bounds.Bottom = bottom;
                _cornerRadius = cornerRadius;
            }
        }
    }
}