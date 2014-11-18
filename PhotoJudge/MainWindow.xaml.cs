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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PhotoJudge
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ImageLoader Loader { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            OutputDirectoryTextBox.Text = @"Y:\photos\judged";
            InputDirectoryTextBox.Text = @"Y:\photos\New iPhoto Library\P1070007.photolibrary\Masters";
        }

        private void OnOutputDirectoryBrowseButtonClicked(object sender, RoutedEventArgs e)
        {

        }

        private void OnInputDirectoryBrowseButtonClicked(object sender, RoutedEventArgs e)
        {

        }

        private void OnStartButtonClicked(object sender, RoutedEventArgs e)
        {
            Loader = new ImageLoader(new FileEnumerator(InputDirectoryTextBox.Text));
            ShowNextImage();
        }

        private void OnKeepButtonClicked(object sender, RoutedEventArgs e)
        {
            ShowNextImage();
        }

        private void OnNextButtonClicked(object sender, RoutedEventArgs e)
        {
            ShowNextImage();
        }

        private FileInfo ShowNextImage()
        {
            ImageInfo imageInfo = Loader.GetNextImage();
            if (imageInfo == null)
            {
                // Stop
                return null;
            }

            ImageViewer.Source = imageInfo.BitmapSource;
            ImagePathTextBlock.Text = imageInfo.FileInfo.FullName;

            return imageInfo.FileInfo;
        }

    }
}
