using Framesoft.Parallax.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Framesoft.Parallax
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ParallaxControl : ContentView
    {
        public ParallaxControl()
        {
            InitializeComponent();
            this.SizeChanged += ParallaxControl_SizeChanged;
        }

        private void ParallaxControl_SizeChanged(object sender, EventArgs e)
        {
            this.SizeChanged -= ParallaxControl_SizeChanged;
            CalculateHeader();
        }
        public int HeaderHeight { get; set; }
        public int HeaderFixedHeight { get; set; }

        public View HeaderView
        {
            get
            {
                return headerView.Content;
            }
            set
            {
                headerView.Content = value;
            }
        }

        public View HeaderControl
        {
            get
            {
                return headerView;
            }
        }

        public View ContentView
        {
            get
            {
                return contentView.Content;
            }
            set
            {
                contentView.Content = value;
            }
        }

        ListView _CurrentListView;
        public ListView CurrentListView
        {
            get
            {
                if (_CurrentListView == null && contentView.Content is ListView)
                    _CurrentListView = contentView.Content as ListView;
                return _CurrentListView;
            }
            set
            {
                ControlEventHelper.IsFirstScrollAfterListViewChanged = true;
                var old = _CurrentListView;
                _CurrentListView = value;
                CalculateHeader();
                RegenerateNewListScroll(old);
            }
        }

        public void CalculateHeader()
        {
            var listView = CurrentListView;
            //listView.VerticalOptions = LayoutOptions.FillAndExpand;
            listView.HeightRequest = 0;
            listView.PropertyChanged -= ListView_PropertyChanged;
            listView.PropertyChanged += ListView_PropertyChanged;
            ControlEventHelper.AddListViewScrollEvent(listView, ListViewScrolled);

            //create a fake header
            listView.Header = new StackLayout() { HeightRequest = HeaderHeight - HeaderFixedHeight };
            listView.Margin = new Thickness(0, HeaderFixedHeight, 0, 0);
        }

        public void RegenerateNewListScroll(ListView oldListView)
        {
            if (_currentDeviceScale != 0)
            {
                int maxTY = -(HeaderHeight - HeaderFixedHeight);
                if (HeaderControl.TranslationY + 1 != maxTY)
                {
                    ControlEventHelper.SetScrollAction?.Invoke(CurrentListView, 0, (int)(HeaderControl.TranslationY * _currentDeviceScale));
                }
                else
                {
                    var scroll = ControlEventHelper.GetScrollFunction?.Invoke(CurrentListView);
                    var pos = scroll.Item2 / _currentDeviceScale;
                    if (pos >= maxTY && scroll.Item3 == 0)
                    {
                        ControlEventHelper.SetScrollAction?.Invoke(CurrentListView, 0, (int)(HeaderControl.TranslationY * _currentDeviceScale));
                    }
                }
            }

            return;
            if (oldListView == null || !ListViewHeaderHeight.ContainsKey(oldListView))
                return;
            var oldTranslationY = ListViewHeaderHeight[oldListView];
            if (oldTranslationY != 0)
            {
                if (oldTranslationY < 0 && ListViewHeaderHeight.ContainsKey(CurrentListView))
                {
                    //var scroll = ControlEventHelper.GetScrollFunction?.Invoke(oldListView);
                    //var oldItemsCount = ((System.Collections.ICollection)oldListView.ItemsSource).Count;
                    //var currentItemsCount = ((System.Collections.ICollection)CurrentListView.ItemsSource).Count;
                    //if (oldItemsCount == 0 || oldListView.RowHeight == 0 || currentItemsCount == 0 || CurrentListView.RowHeight == 0)
                    //    return;
                    //var heightOfOldListScroll = oldListView.RowHeight * oldItemsCount;
                    //var nesbat = heightOfOldListScroll / scroll.Item2;

                    //var heightOfCurrentListScroll = CurrentListView.RowHeight * currentItemsCount;
                    //var newY = heightOfCurrentListScroll / nesbat;


                }
            }
        }

        private void ListView_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ItemsSource")
            {
                var listView = sender as ListView;
                listView.HeightRequest = -1;
            }
        }

        Dictionary<ListView, double> ListViewHeaderHeight = new Dictionary<ListView, double>();
        public float _currentDeviceScale = 0;
        public void ListViewScrolled(ListView listView, int firstItemPosition, float deviceScale, float x, float y)
        {
            if (listView != CurrentListView)
                return;
            _currentDeviceScale = deviceScale;
            if (firstItemPosition > 0 || Math.Abs(y / deviceScale) < 0)
            {
                HeaderControl.TranslationY = -(HeaderHeight - HeaderFixedHeight) - 1;
            }
            else
                HeaderControl.TranslationY = y / deviceScale;
            ListViewHeaderHeight[CurrentListView] = HeaderControl.TranslationY;
        }
    }
}