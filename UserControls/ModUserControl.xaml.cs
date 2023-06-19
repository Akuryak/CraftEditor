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
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CraftEditor.UserControls
{
    /// <summary>
    /// Логика взаимодействия для ModUserControl.xaml
    /// </summary>
    public partial class ModUserControl : UserControl
    {
        public ModUserControl(string modImage, string modName)
        {
            InitializeComponent();

            ModNameTextBlock.Text = modName;

            if (!string.IsNullOrWhiteSpace(modImage))
                Refresh(modImage);
        }

        private async void Refresh(string modImage)
        {
            await LoadImageAsync(modImage);
        }

        private async Task LoadImageAsync(string modImage)
        {
            try
            {
                var bitmap = await Task.Run(() =>
                {
                    try
                    {
                        BitmapImage bitmapImage = new BitmapImage();
                        using (var stream = new FileStream(modImage, FileMode.Open, FileAccess.Read))
                        {
                            bitmapImage.BeginInit();
                            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                            bitmapImage.StreamSource = stream;
                            bitmapImage.EndInit();

                            bitmapImage.Freeze();
                        }
                        return bitmapImage;
                    }
                    catch { return null; }
                });
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    if (bitmap != null)
                        ModImage.Source = bitmap;
                });
            }
            catch { }
        }
    }
}
