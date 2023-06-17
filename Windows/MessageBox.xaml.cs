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
using System.Windows.Shapes;

namespace CraftEditor.Windows
{
    /// <summary>
    /// Логика взаимодействия для MessageBox.xaml
    /// </summary>
    public partial class MessageBox : Window
    {
        public MessageBox(string messageBoxText, string caption)
        {
            InitializeComponent();

            Title = caption;
            MessageBoxTextBlock.Text = messageBoxText;
        }

        public static void Show(string messageBoxText, string caption)
        {
            MessageBox messageBox = new MessageBox(messageBoxText, caption);
            messageBox.ShowDialog();
        }
    }
}
