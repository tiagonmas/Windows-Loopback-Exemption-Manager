using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Shapes;

namespace Loopback
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private LoopUtil _loop;
        private List<LoopUtil.AppContainer> appFiltered=new List<LoopUtil.AppContainer>();
        private bool isDirty=false;

        public MainWindow()
        {
            InitializeComponent();
            _loop = new LoopUtil();
             dgLoopback.ItemsSource =appFiltered;
             Filter(String.Empty,false);
            ICollectionView cvApps = CollectionViewSource.GetDefaultView(dgLoopback.ItemsSource);

        }


        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!isDirty) 
            {
                Log("nothing to save");
                return; 
            }

            isDirty = false;
            if (_loop.SaveLoopbackState())
            { 
                Log(" saved loopback excemptions");
            }
            else
            { Log(" ERROR SAVING"); }
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            _loop.LoadApps();
            Filter(String.Empty, false);
            txtFilter.Text = "";
            cbLoopback.IsChecked = false;
            isDirty = false;
            Log("refreshed");
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (isDirty)
            {
                MessageBoxResult resp=System.Windows.MessageBox.Show("You have not saved your changes. Are you sure you want to exit ?","Loopback Manager", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (resp==MessageBoxResult.No)
                {
                    e.Cancel = true;
                    return;
                }

            }
            //To Do
            _loop.FreeResources();
        }

        private void txtFilter_KeyUp(object sender, KeyEventArgs e)
        {
            Filter(txtFilter.Text, (bool)cbLoopback.IsChecked);
        }


        private void cbLoopback_Click(object sender, RoutedEventArgs e)
        {
            Filter(txtFilter.Text, (bool)cbLoopback.IsChecked);
        }

        private void Filter(string filter, bool enabled)
        {
            string right = filter.ToUpper();
            appFiltered.Clear();

            foreach (LoopUtil.AppContainer app in _loop.Apps)
            {
                string left = app.displayName.ToUpper();

                if (filter == String.Empty || left.Contains(right))
                {
                    if (enabled == false || app.LoopUtil == true)
                    {
                        appFiltered.Add(app);
                    }
                }
            }
            dgLoopback.Items.Refresh();
        }

        private void dgcbLoop_Click(object sender, RoutedEventArgs e)
        {
            isDirty=true;
        }

        private void Log(String logtxt) 
        {
                txtStatus.Text = DateTime.Now.ToString("hh:mm:ss.fff ") + logtxt;
        }

    }
}
