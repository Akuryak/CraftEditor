using System;
using System.Collections.Generic;
using System.DirectoryServices;
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

namespace CraftEditor.Pages
{
    /// <summary>
    /// Логика взаимодействия для SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            InitializeComponent();
        }

        private async Task GetUsageMemory()
        {
            long size = 0;
            DirectoryInfo directoryInfo = new DirectoryInfo(App.AppDataPath);
            foreach (FileInfo file in directoryInfo.GetFiles("*", SearchOption.AllDirectories))
            {
                size += file.Length;
            }
            MemoryUsageTextBlock.Text = $"Используется {Math.Round((double)size / 1024 / 1024, 1)} мб";
        }

        private async void ClearCacheButton_Click(object sender, RoutedEventArgs e)
        {
            foreach(string file in Directory.GetFiles(App.AppDataPath, "*", SearchOption.AllDirectories))
            {
                File.Delete(file);
            }

            await GetUsageMemory();
        }

        private void ReturnButton_Click(object sender, RoutedEventArgs e)
        {
            Helpers.PageManager.Navigate(new Pages.MainPage(), $"CraftEditor {App.Version}");
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            await GetUsageMemory();
        }
    }
}
