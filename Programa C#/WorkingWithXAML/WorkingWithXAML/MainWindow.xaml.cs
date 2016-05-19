using System;
using System.Collections.Generic;
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

namespace WorkingWithXAML
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        const int initialHeight = 720;
        const int initialWidth = 1280;
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //changeObjectsSize();
        }

        private void changeObjectsSize()
        {
            double hProportion = this.ActualHeight / initialHeight;
            double wProportion = this.ActualWidth / initialWidth;
            //MessageBox.Show(this.ActualHeight.ToString() + " " + this.ActualWidth.ToString());
            string[] names = {"pMain", "p0", "p1", "p2", "p3", "p4", "p5", "p6", "p7", "p8" };
            foreach (string name in names)
            {
                var obj = (Grid)FindName(name);
                //MessageBox.Show(name + "altura= " + obj.Height.ToString() + " largura= " + obj.Width.ToString() + " x=" + obj.Margin.Left.ToString() + " y=" + obj.Margin.Top.ToString());
                obj.Height *= hProportion;
                obj.Width *= wProportion;
                obj.Margin = new Thickness(obj.Margin.Left * wProportion, obj.Margin.Top * hProportion, obj.Margin.Right * wProportion, obj.Margin.Bottom * hProportion);
                //MessageBox.Show(name + "altura= " + obj.Height.ToString() + " largura= " + obj.Width.ToString() + " x=" + obj.Margin.Left.ToString() + " y=" + obj.Margin.Top.ToString());
            }
            txtInput.Height *= hProportion;
            txtInput.Width *= wProportion;
            txtInput.FontSize *= Math.Sqrt(Math.Pow(hProportion, 2) + Math.Pow(wProportion, 2));
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.WindowState = System.Windows.WindowState.Maximized;
            //this.Height = 480;
            //this.Width = 640;
            //MessageBox.Show(this.ActualHeight.ToString() + " " + this.ActualWidth.ToString());
            changeObjectsSize();
        }
    }
}
