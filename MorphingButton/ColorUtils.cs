using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace MorphingButton
{
    public static class ColorUtils
    {
        public static Color FromUint(uint argb)
        {
            return Color.Argb((int)(byte)((argb & 4278190080U) >> 24), (int)(byte)((argb & 16711680U) >> 16), (int)(byte)((argb & 65280U) >> 8), (int)(byte)(argb & (uint)byte.MaxValue));
        }
    }
}