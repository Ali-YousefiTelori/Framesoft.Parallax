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
using Framesoft.Parallax.Droid.Renderers;
using Framesoft.Parallax.Helpers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(Xamarin.Forms.ListView), typeof(CustomListViewRenderer))]
namespace Framesoft.Parallax.Droid.Renderers
{
    public class CustomListViewRenderer : ListViewRenderer
    {
        static CustomListViewRenderer()
        {
            ControlEventHelper.SetScrollAction = (lst, x, y) =>
            {
                try
                {
                    var listViewRenderer = (Xamarin.Forms.Platform.Android.ViewRenderer<Xamarin.Forms.ListView, Android.Widget.ListView>)lst.GetRenderer();
                    var listView = listViewRenderer.Control;
                    //int item = (int)Math.Floor((double)y / lst.RowHeight);
                    //int scroll = (int)((item * lst.RowHeight) - (double)y);
                    listView.SetSelectionFromTop(0, y);// Important
                    //listView.ScrollTo(x,y);
                }
                catch (Exception ex)
                {

                }
            };

            ControlEventHelper.GetScrollFunction = (lst) =>
            {
                try
                {
                    var listViewRenderer = (Xamarin.Forms.Platform.Android.ViewRenderer<Xamarin.Forms.ListView, Android.Widget.ListView>)lst.GetRenderer();
                    var listView = listViewRenderer.Control;
                    if (listView.ChildCount == 0)
                        return new Tuple<int, int, int>(0, 0, 0);

                    return new Tuple<int, int, int>(0, listView.GetChildAt(0).Top, listView.FirstVisiblePosition);
                }
                catch (Exception ex)
                {
                    return new Tuple<int, int, int>(0, 0, 0);
                }
            };
        }

        public CustomListViewRenderer(Context context) : base(context)
        {
        }

        public static Action<bool> ChangeScrollEnabledAction { get; set; }
        public static Action<float, float, GestureStatus, float> DispatchTouchEventAction { get; set; }
        public bool ScrollEnabled { get; set; } = true;
        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.ListView> e)
        {
            base.OnElementChanged(e);
            if (e.OldElement != null || Element == null)
            {
                return;
            }
            Control.Scroll += Control_Scroll;
            ChangeScrollEnabledAction += (value) =>
            {
                ScrollEnabled = value;
            };
        }

        float firstY = 0.0f;


        public override bool DispatchTouchEvent(MotionEvent e)
        {
            if (e.Action == MotionEventActions.Down)
                firstY = e.GetY();
            else if (!ScrollEnabled && e.Action == MotionEventActions.Move && e.GetY() != firstY)
            {
                //GestureStatus status = GestureStatus.Canceled;
                //if (e.Action == MotionEventActions.Down)
                //    status = GestureStatus.Started;
                //else if (e.Action == MotionEventActions.Up)
                //    status = GestureStatus.Completed;
                //else if (e.Action == MotionEventActions.Move)
                //    status = GestureStatus.Running;
                DispatchTouchEventAction?.Invoke(e.GetX(), e.GetY(), GestureStatus.Running, Resources.DisplayMetrics.Density);
                return false;
            }
            else if (DispatchTouchEventAction != null && (e.Action == MotionEventActions.Cancel || e.Action == MotionEventActions.Up))
                DispatchTouchEventAction?.Invoke(e.GetX(), e.GetY(), GestureStatus.Canceled, Resources.DisplayMetrics.Density);
            var result = base.DispatchTouchEvent(e);
            return result;
        }
        private void Control_Scroll(object sender, AbsListView.ScrollEventArgs e)
        {
            if (ControlEventHelper.IsFirstScrollAfterListViewChanged)
            {
                ControlEventHelper.IsFirstScrollAfterListViewChanged = false;
                return;
            }
            var action = Parallax.Helpers.ControlEventHelper.GetListViewScrollAction(this.Element);
            if (action == null)
                return;
            else if (Control.ChildCount == 0)
                return;
            var c = Control.GetChildAt(0);

            action(Element, Control.FirstVisiblePosition, Resources.DisplayMetrics.Density, 0, c.Top);
        }
    }
}