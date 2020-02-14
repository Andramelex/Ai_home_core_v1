using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Shapes;

namespace Ai_home_core_v1
{
    /// <summary>
    /// Логика взаимодействия для BaseForm.xaml
    /// </summary>
    public partial class BaseForm : Window
    {
        public BaseForm()
        {
            InitializeComponent();
            LoadImage();




        }
        public void LoadImage()
        {
            BitmapImage bitmap = new BitmapImage();
            var stream = File.OpenRead(@"D:\wamp2\www\3\images\cam\web1.jpg");
            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.OnLoad;

            bitmap.StreamSource = stream;
            bitmap.EndInit();
            stream.Close();

            Application.Current.Dispatcher.Invoke(() =>
            {
                myImage.Source = bitmap;
            });

        }

        private void But_click(object sender, RoutedEventArgs e)
        {
            LoadImage();

        }
    }
}
