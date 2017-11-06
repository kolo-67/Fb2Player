using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Fb2PlayerViewModel;
using Fb2PlayerCommon.Contracts;

namespace Fb2Player
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class Fb2PlayerView : Window
    {
        private bool isStart = true;
        private bool isTimelineFromPosition = false;
        private int numberOfTimerRun = 0;
        public Fb2PlayerView()
        {
            InitializeComponent();
        }
        //----------------------------------------------------------------------------------------------------------------------    
        private void UIElement_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                System.Diagnostics.Process.Start(txtPlaingFileDirectory.Text);
                DataObject data = new DataObject(DataFormats.StringFormat, "cd " + txtPlaingFileDirectory.Text);

                Clipboard.SetDataObject(data);
            }
        }
        //----------------------------------------------------------------------------------------------------------------------    
        private void SlTimeline_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //if (Math.Abs((workPlayer.Position.TotalMilliseconds - slTimeline.Value)) > 1000 && !isStart)
            //    workPlayer.Position = new TimeSpan(0, 0, 0, 0, (int)e.NewValue);
            //if (!isTimelineFromPosition && !isStart)
            //    workPlayer.Position = new TimeSpan(0, 0, 0, 0, (int)e.NewValue);
        }
        //----------------------------------------------------------------------------------------------------------------------    
        private void MediaElementMouseDown(Object sender, MouseButtonEventArgs e)
        {
        }
        //----------------------------------------------------------------------------------------------------------------------    
        private void WorkPlayer_OnMediaEnded(object sender, RoutedEventArgs e)
        {
        }
        //----------------------------------------------------------------------------------------------------------------------    
        private void WorkPlayer_OnMediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
        }
        //----------------------------------------------------------------------------------------------------------------------    
        private void Row_DoubleClick(object sender, MouseButtonEventArgs e)
        {
        }
        //----------------------------------------------------------------------------------------------------------------------    
        private void TbFileName_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
        }
        //----------------------------------------------------------------------------------------------------------------------    
        private void DataGrid_OnSelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            DataGrid dg = sender as DataGrid;
            if (dg != null && dg.SelectedItem != null)
            {
                dg.ScrollIntoView(dg.SelectedItem);
            }
        }
        //----------------------------------------------------------------------------------------------------------------------    
        private void TbFileName_OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            DataGridRow row = e.Source as DataGridRow;
            object arg = null;
            if (row != null)
                arg = row.DataContext;

            if (e.ClickCount == 2)
                CurrentTrackDoubleClickCommand.Execute(arg);
        }
        //-------------------------------------------------------------------------------------------------------------------
        public ICommand CurrentTrackDoubleClickCommand
        {
            get { return (ICommand)GetValue(CurrentTrackDoubleClickCommandProperty); }
            set { SetValue(CurrentTrackDoubleClickCommandProperty, value); }
        }
        //-------------------------------------------------------------------------------------------------------------------
        public static DependencyProperty CurrentTrackDoubleClickCommandProperty = DependencyProperty.RegisterAttached(
                                                                        "CurrentTrackDoubleClickCommand",
                                                                        typeof(ICommand),
                                                                        typeof(Fb2PlayerView));

        //-------------------------------------------------------------------------------------------------------------------
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var viewModel = new Fb2PlayerViewModel.Fb2PlayerViewModel();
            DataContext = viewModel;

            ISavable saveModel = DataContext as ISavable;
            if (saveModel != null)
            {
                this.Closing += (s, ev) => saveModel.Save(); ;

            }
            IScrollIntoViewAction scrollIntoViewAction = (IScrollIntoViewAction)DataContext;
            if (scrollIntoViewAction != null)
            {
                scrollIntoViewAction.SpeechPhraseScrollIntoView += () =>
                {
                    SpeechPhraseScrollIntoView();
                };
            }
        }
        //-------------------------------------------------------------------------------------------------------------------
        private void SpeechPhraseScrollIntoView()
        {
            try
            {
                lbParagraphs.ScrollIntoView(lbParagraphs.SelectedItem);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }
        //-------------------------------------------------------------------------------------------------------------------
    }
    //----------------------------------------------------------------------------------------------------------------------    
    public class TimelineMultiValueConvertor : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            TimeSpan ts = (TimeSpan)values[0];
            double dValue = ts.TotalMilliseconds;

            return dValue;
        }
        //----------------------------------------------------------------------------------------------------------------------    
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            //if (value != null)
            //{
            //    double lSliderValue = (double)value;

            //    TimeSpan ts = new TimeSpan(0, 0, 0, 0, (int)lSliderValue);
            //    return  new object[] { ts, lSliderValue };
            //}
            return new object[] { null, null };
        }
    }
    //----------------------------------------------------------------------------------------------------------------------    
    [ValueConversion(typeof(DateTime), typeof(String))]
    public class TimeSpanAsStringConverter : IValueConverter
    {
        //----------------------------------------------------------------------------------------------------------------------    
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double position = (double)value;
            var ts = new TimeSpan(0, 0, 0, 0, (int)position);
            return (new TimeSpan(0, 0, 0, (int)ts.TotalSeconds, 0)).ToString();
        }
        //----------------------------------------------------------------------------------------------------------------------    
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
        //----------------------------------------------------------------------------------------------------------------------    
    }
    //----------------------------------------------------------------------------------------------------------------------    

}
