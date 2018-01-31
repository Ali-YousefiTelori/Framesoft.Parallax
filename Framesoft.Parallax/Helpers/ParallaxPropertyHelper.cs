using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Framesoft.Parallax.Helpers
{
    public static class ParallaxPropertyHelper
    {
        public static readonly BindableProperty IsClickableProperty =
                BindableProperty.CreateAttached("IsClickable", typeof(bool), typeof(ParallaxPropertyHelper), false, propertyChanged: OnHasShadowChanged);

        public static bool GetIsClickable(BindableObject view)
        {
            return (bool)view.GetValue(IsClickableProperty);
        }

        public static void SetIsClickable(BindableObject view, bool value)
        {
            view.SetValue(IsClickableProperty, value);
        }

        static void OnHasShadowChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var view = bindable as View;
            if (view == null)
            {
                return;
            }
            if (view is Layout)
            {
                ((Layout)view).ChildAdded += ParallaxPropertyHelperHasShadow_ChildAdded;
                foreach (var item in ((Layout)view).Children)
                {
                    SetIsClickable(item, (bool)newValue);
                }
            }
        }

        static void AddToClickable(View view)
        {
            SetIsClickable(view, true);
        }

        private static void ParallaxPropertyHelperHasShadow_ChildAdded(object sender, ElementEventArgs e)
        {
            var view = sender as View;
            AddToClickable(view);
            if (view is Layout)
            {
                ((Layout)view).ChildAdded += ParallaxPropertyHelperHasShadow_ChildAdded;
                foreach (var item in ((Layout)view).Children)
                {
                    SetIsClickable(item, true);
                }
            }
        }



        //public static readonly BindableProperty IsGestureRecognizersForChildrenProperty =
        //       BindableProperty.CreateAttached("IsGestureRecognizersForChildren", typeof(bool), typeof(ParallaxPropertyHelper), false, propertyChanged: OnGestureRecognizersForChildrenChanged);

        //public static bool GetIsGestureRecognizersForChildren(BindableObject view)
        //{
        //    return (bool)view.GetValue(IsGestureRecognizersForChildrenProperty);
        //}

        //public static void SetIsGestureRecognizersForChildren(BindableObject view, bool value)
        //{
        //    view.SetValue(IsGestureRecognizersForChildrenProperty, value);
        //}

        //static void OnGestureRecognizersForChildrenChanged(BindableObject bindable, object oldValue, object newValue)
        //{
        //    var view = bindable as View;
        //    if (view == null)
        //    {
        //        return;
        //    }
        //    if (view is Layout)
        //    {
        //        ((Layout)view).ChildAdded += ParallaxPropertyHelperGestureRecognizersForChildren_ChildAdded;
        //        foreach (View item in ((Layout)view).Children)
        //        {
        //            foreach (var g in view.GestureRecognizers)
        //            {
        //                item.GestureRecognizers.Add(g);
        //            }
        //            SetIsGestureRecognizersForChildren(item, (bool)newValue);
        //        }
        //    }
        //}

        //private static void ParallaxPropertyHelperGestureRecognizersForChildren_ChildAdded(object sender, ElementEventArgs e)
        //{
        //    var view = sender as View;
        //    AddGestureRecognizersForChildren(view);
        //    if (view is Layout)
        //    {
        //        ((Layout)view).ChildAdded += ParallaxPropertyHelperHasShadow_ChildAdded;
        //        foreach (View item in ((Layout)view).Children)
        //        {
        //            foreach (var g in view.GestureRecognizers)
        //            {
        //                item.GestureRecognizers.Add(g);
        //            }
        //            SetIsGestureRecognizersForChildren(item, true);
        //        }
        //    }
        //}

        //private static void AddGestureRecognizersForChildren(View view)
        //{
        //    SetIsGestureRecognizersForChildren(view, true);
        //}
    }
}
