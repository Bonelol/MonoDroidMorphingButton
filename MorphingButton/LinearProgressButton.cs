using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace MorphingButton
{
    public class LinearProgressButton : MorphingButton, IProgress
    {
        public const int MAX_PROGRESS = 100;
        public const int MIN_PROGRESS = 0;

        public int Progress { get; set; }
        public int ProgressColor { get; set; }
        public int ProgressCornerRadius { get; set; }

        private Paint paint;
        private RectF rectF;


        public LinearProgressButton(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public LinearProgressButton(Context context) : base(context)
        {
        }

        public LinearProgressButton(Context context, IAttributeSet attrs) : base(context, attrs)
        {
        }

        public LinearProgressButton(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
        }

        public LinearProgressButton(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes)
        {
        }

        public void SetProgress(int progress)
        {
            Progress = progress;
            Invalidate();
        }

        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);

            if (AnimationInProgress && Progress > MIN_PROGRESS && Progress < MAX_PROGRESS)
            {
                if (paint == null)
                {
                    paint = new Paint
                    {
                        AntiAlias = true,
                        Color = new Color(ProgressColor)
                    };
                    paint.SetStyle(Android.Graphics.Paint.Style.Fill);
                }

                if (rectF == null)
                {
                    rectF = new RectF();
                }

                rectF.Right = Width/MAX_PROGRESS*Progress;
                rectF.Bottom = Height;
                canvas.DrawRoundRect(rectF, ProgressCornerRadius, ProgressCornerRadius, paint);
            }
        }

        public override void Morph(Params p)
        {
            base.Morph(p);

            Progress = MIN_PROGRESS;
            paint = null;
            rectF = null;
        }

        public void ToProgress(int color, int progressColor, int radius, int width, int height, int duration)
        {
            ProgressCornerRadius = radius;
            ProgressColor = progressColor;

            var longRoundedSquare = MorphingButton.Params.Create()
                .SetDuration(duration)
                .SetCornerRadius(radius)
                .SetWidth(width)
                .SetHeight(height)
                .SetColor(color)
                .SetColorPressed(color);

            Morph(longRoundedSquare);
        }
    }
}