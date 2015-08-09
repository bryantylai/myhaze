using HazeMY.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace HazeMY.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            this.DataContext = ViewModel;
            
            PsiTextBlock.FontSize = LocationTextBlock.FontSize * 3;
        }

        private MainPageViewModel _viewModel;
        public MainPageViewModel ViewModel
        {
            get
            {
                return _viewModel ?? (_viewModel = new MainPageViewModel());
            }

            set
            {
                _viewModel = value;
            }
        }

        private void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            SplitView.IsPaneOpen = false;
        }

        private void SplitView_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (e.OriginalSource is Grid)
                SplitView.IsPaneOpen = true;
        }
    }
}
