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
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms;
using Framesoft.Parallax;
using Framesoft.Parallax.Droid.Renderers;
using Framesoft.Parallax.Helpers;

[assembly: ExportRenderer(typeof(ParallaxControl), typeof(ParallaxControlRenderer))]
namespace Framesoft.Parallax.Droid.Renderers
{
    public class ParallaxControlRenderer : ViewRenderer
    {
        protected override void OnFinishInflate()
        {
            base.OnFinishInflate();

        }
        
        ParallaxControl parallaxControl;
        public ParallaxControlRenderer(Context context) : base(context)
        {

        }

        public override void OnViewAdded(Android.Views.View child)
        {
            base.OnViewAdded(child);
            try
            {
                parallaxControl = (ParallaxControl)this.Element;
               
                var header = (Android.Views.View)parallaxControl.HeaderControl.GetRenderer();

                GenerateHeader(header);
            }
            catch (Exception ex)
            {
                //System.Diagnostics.Debug.WriteLine(@"          ERROR: ", ex.Message);
            }
        }

        private void Header_Click(object sender, EventArgs e)
        {

        }

        List<Android.Views.View> GeneratedViews = new List<Android.Views.View>();
        void GenerateHeader(Android.Views.View header)
        {
            if (GeneratedViews.Contains(header))
                return;
            else
                GeneratedViews.Add(header);
            var childs = GetAllChildrenBFS(header);
            foreach (object item in childs)
            {
                bool canTouch = true;
                if (item is IVisualElementRenderer)
                {
                    IVisualElementRenderer renderer = (IVisualElementRenderer)(object)item;
                    Xamarin.Forms.View xView = (Xamarin.Forms.View)renderer.Element;
                    //if (xView.GestureRecognizers.Count > 0)
                    //    canTouch = false;
                    if (ParallaxPropertyHelper.GetIsClickable(xView as BindableObject))
                        canTouch = false;
                }
                else
                    canTouch = false;

                Android.Views.View view = (Android.Views.View)item;
                if (!view.Clickable && canTouch)
                    view.Touch += OnHeaderTouch;
            }
        }

        private List<Android.Views.View> GetAllChildrenBFS(Android.Views.View v)
        {
            List<Android.Views.View> visited = new List<Android.Views.View>();
            List<Android.Views.View> unvisited = new List<Android.Views.View>();
            unvisited.Add(v);

            while (unvisited.Count > 0)
            {
                Android.Views.View child = unvisited[0];
                unvisited.Remove(child);
                visited.Add(child);
                if (!(child is ViewGroup)) continue;
                ViewGroup group = (ViewGroup)child;
                int childCount = group.ChildCount;
                for (int i = 0; i < childCount; i++) unvisited.Add(group.GetChildAt(i));
            }

            return visited;
        }
        
        
        protected virtual void OnHeaderTouch(object sender, Android.Views.View.TouchEventArgs e)
        {
            Android.Widget.ListView listView = null;
            Android.Views.View header = null;

            try
            {
                var listViewRenderer = (Xamarin.Forms.Platform.Android.ViewRenderer<Xamarin.Forms.ListView, Android.Widget.ListView>)parallaxControl.CurrentListView.GetRenderer();
                listView = listViewRenderer.Control;
                header = (Android.Views.View)parallaxControl.HeaderControl.GetRenderer();
                GenerateHeader(header);
            }
            catch (Exception ex)
            {
                return;
            }

            var evt = e.Event;
            //var eventConsumed = false;

            if (listView != null && header != null)
            {
                MotionEvent ev = MotionEvent.Obtain(
                                evt.DownTime,
                                evt.EventTime,
                                evt.Action,
                                evt.GetX(),
                                evt.GetY() + header.TranslationY,
                                evt.Pressure, evt.Size, evt.MetaState, evt.XPrecision, evt.YPrecision, evt.DeviceId, evt.EdgeFlags);
                var d = listView.DispatchTouchEvent(ev);
                //return;
                //switch (evt.Action)
                //{
                //    case MotionEventActions.Move:
                //        if (!downEventDispatched)
                //        {
                //            // if moving, create a fake down event for the scrollingView to start the scroll. 
                //            // the y of the touch in the scrolling view is the y coordinate of the touch in the 
                //            // header + the translation of the header
                //            MotionEvent downEvent = MotionEvent.Obtain(
                //                evt.DownTime - 1,
                //                evt.EventTime - 1,
                //                MotionEventActions.Down,
                //                evt.GetX(),
                //                evt.GetY() + header.TranslationY,
                //                evt.Pressure, evt.Size, evt.MetaState, evt.XPrecision, evt.YPrecision, evt.DeviceId, evt.EdgeFlags);
                //            //evt.Action = MotionEventActions.Down;
                //            listView.DispatchTouchEvent(downEvent);
                //            //  listView.DispatchTouchEvent(downEvent);
                //            downEventDispatched = true;
                //        }

                //        // dispatching the move event. we need to create a fake motionEvent using a different y 
                //        // coordinate related to the scrolling view
                //        //MotionEvent moveEvent = MotionEvent.Obtain(
                //        //    evt.DownTime,
                //        //    evt.EventTime,
                //        //    MotionEventActions.Move,
                //        //    evt.GetX(),
                //        //    evt.GetY() + header.TranslationY,
                //        //    0);
                //        MotionEvent moveEvent = MotionEvent.Obtain(
                //               evt.DownTime,
                //               evt.EventTime,
                //               MotionEventActions.Move,
                //               evt.GetX(),
                //               evt.GetY() + header.TranslationY,
                //               evt.Pressure, evt.Size, evt.MetaState, evt.XPrecision, evt.YPrecision, evt.DeviceId, evt.EdgeFlags);
                //        //evt.Action = MotionEventActions.Down;
                //        listView.DispatchTouchEvent(moveEvent);
                //        //evt.Action = MotionEventActions.Move;
                //        //listView.DispatchTouchEvent(evt);
                //        //listView.DispatchTouchEvent(moveEvent);
                //        break;
                //    case MotionEventActions.Up:

                //        // when action up, dispatch an action cancel to avoid a possible click
                //        //MotionEvent cancelEvent = MotionEvent.Obtain(
                //        //    evt.DownTime,
                //        //    evt.EventTime,
                //        //    MotionEventActions.Cancel,
                //        //    evt.GetX(),
                //        //    evt.GetY(),
                //        //    0);
                //        evt.Action = MotionEventActions.Cancel;
                //        listView.DispatchTouchEvent(evt);
                //        //listView.DispatchTouchEvent(cancelEvent);
                //        downEventDispatched = false;
                //        break;
                //    case MotionEventActions.Cancel:
                //        listView.DispatchTouchEvent(evt);
                //        downEventDispatched = false;
                //        break;
                //}

                //eventConsumed = true;
            }

            // e.Handled = eventConsumed;
        }
    }
}