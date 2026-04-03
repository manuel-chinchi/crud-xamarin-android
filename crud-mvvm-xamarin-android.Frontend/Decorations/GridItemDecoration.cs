using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace crud_xamarin_android.Frontend.Decorations
{
    public class GridItemDecoration : RecyclerView.ItemDecoration
    {
        private readonly Paint _paint;
        private readonly int _dividerHeight;

        public GridItemDecoration(int color, int dividerHeight)
        {
            _paint = new Paint
            {
                Color = new Color(color),
                StrokeWidth = dividerHeight
            };
            this._dividerHeight = dividerHeight;
        }

        public override void OnDraw(Canvas c, RecyclerView parent, RecyclerView.State state)
        {
            base.OnDraw(c, parent, state);

            int childCount = parent.ChildCount;
            //int width = parent.Width;

            for (int i = 0; i < childCount; i++)
            {
                View child = parent.GetChildAt(i);

                if (i < childCount - 1)
                {
                    c.DrawLine(child.Left, child.Bottom, child.Right, child.Bottom, _paint);
                }

                c.DrawLine(child.Right, child.Top, child.Right, child.Bottom, _paint);
            }
        }

        public override void GetItemOffsets(Rect outRect, View view, RecyclerView parent, RecyclerView.State state)
        {
            base.GetItemOffsets(outRect, view, parent, state);

            outRect.Bottom = _dividerHeight;
            outRect.Right = _dividerHeight;
        }
    }
}