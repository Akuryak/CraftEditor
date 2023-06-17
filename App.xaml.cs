using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace CraftEditor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public const string Version = "0.0.1";
        public static string AppDataPath = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}/CraftEditor";
    }
}
