using Busan.Logics;
using Busan.Models;
using MahApps.Metro.Controls;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI.Relational;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Busan
{
    /// <summary>
    /// DetailWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class DetailWindow : MetroWindow
    {
        public DetailWindow()
        {
            InitializeComponent();
        }

        public DetailWindow(string sectorName, string content, string posterPath) : this()
        {
            LblSectorName.Content = $"{sectorName} 을(를) 소개합니다!";
            TxtContent.Text = content ;
            ImgPoster.Source = new BitmapImage(new Uri($"{posterPath}", UriKind.RelativeOrAbsolute));
        }


        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            
            


        }
    }
}
