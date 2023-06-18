using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Interop;

namespace CraftEditor.Pages
{
    /// <summary>
    /// Логика взаимодействия для MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        public static string? SelectedModName { get; set; }
        private static string? defaultModsDerectory;
        public MainPage()
        {
            InitializeComponent();
        }

        private void RefreshModsListBox()
        {
            foreach (string recipe in Directory.GetFiles($"{App.AppDataPath}/{SelectedModName}/", "*.json"))
            {
                string image = string.Empty;
                if (File.Exists(recipe.Replace(".json", ".png")))
                    image = recipe.Replace(".json", ".png");
                ModRecipeListBox.Items.Add(new UserControls.ModUserControl(image, recipe.Split("/").Last()));
            }
        }

        private async Task GetRecipesImages(UserControls.ModUserControl selectedRecipe)
        {
            List<ZipArchiveEntry> images = ZipFile.OpenRead($"{defaultModsDerectory}/{SelectedModName}.jar").Entries.Where(x => x.FullName.EndsWith(".png")).ToList();

            selectedRecipe.LoadProgressBar.Visibility = Visibility.Visible;
            selectedRecipe.LoadProgressBar.Opacity = 1;
            selectedRecipe.LoadProgressBar.Maximum = images.Count;

            for (int i = 0; i < images.Count; i++)
            {
                await Task.Run(() =>
                {
                    try
                    {
                        if (!File.Exists($"{App.AppDataPath}/{SelectedModName}/{images[i].Name.Replace(".json", ".png")}"))
                        {
                            ZipArchiveEntry imageRecipes = ZipFile.OpenRead($"{defaultModsDerectory}/{SelectedModName}.jar").Entries.FirstOrDefault(x => x.FullName.Contains(images[i].Name.Replace(".json", ".png")));
                            imageRecipes?.ExtractToFile($"{App.AppDataPath}/{SelectedModName}/{images[i].Name.Replace(".json", ".png")}", true);
                        }
                    }
                    catch { }
                });
                if ((UserControls.ModUserControl)ModsListBox.SelectedItem == selectedRecipe)
                    selectedRecipe.LoadProgressBar.Value = i;
            }

            if ((UserControls.ModUserControl)ModsListBox.SelectedItem == selectedRecipe)
            {
                selectedRecipe.LoadProgressBar.Value = images.Count;
                selectedRecipe.LoadProgressBar.Opacity = 0.50;
            }
        }

        private async Task GetRecipes(UserControls.ModUserControl selectedRecipe)
        {
            ModRecipeListBox.Items.Clear();

            List<ZipArchiveEntry> recipes = ZipFile.OpenRead($"{defaultModsDerectory}/{SelectedModName}.jar").Entries.Where(x => x.FullName.Contains("recipes")).Where(x => x.FullName.EndsWith(".json")).ToList();
            Directory.CreateDirectory($"{App.AppDataPath}/{SelectedModName}");

            selectedRecipe.LoadProgressBar.Visibility = Visibility.Visible;
            selectedRecipe.LoadProgressBar.Opacity = 1;
            selectedRecipe.LoadProgressBar.Maximum = recipes.Count;

            for (int i = 0; i < recipes.Count; i++)
            {
                await Task.Run(() =>
                {
                    try
                    {
                        if (!File.Exists($"{App.AppDataPath}/{SelectedModName}/{recipes[i].Name}"))
                        {
                            recipes[i].ExtractToFile($"{App.AppDataPath}/{SelectedModName}/{recipes[i].Name}", true);
                        }
                    }
                    catch { }
                });
                if ((UserControls.ModUserControl)ModsListBox.SelectedItem == selectedRecipe)
                    selectedRecipe.LoadProgressBar.Value = i;
                else
                    return;
            }

            if ((UserControls.ModUserControl)ModsListBox.SelectedItem == selectedRecipe)
            {
                selectedRecipe.LoadProgressBar.Value = recipes.Count;
                selectedRecipe.LoadProgressBar.Opacity = 0.50;
            }
        }

        private async void Refresh()
        {
            if (defaultModsDerectory != $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}/.minecraft/mods" && Directory.GetDirectories($"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}/.minecraft/versions").Length > 0)
            {
                foreach (string direcotory in Directory.GetDirectories($"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}/.minecraft/versions"))
                {
                    VersionsComboBox.Items.Add(new ComboBoxItem().Content = direcotory.Replace("\\", "/").Split("/").Last());
                }
                VersionsComboBox.SelectedIndex = 0;
            }
            else
                VersionsComboBox.Visibility = Visibility.Collapsed;

            if (Directory.GetFiles(defaultModsDerectory).Length > 0)
            {
                ModsListBox.Items.Clear();
                foreach (string directory in Directory.GetFiles(defaultModsDerectory))
                {
                    string modIcon = string.Empty;

                    if (!File.Exists(App.AppDataPath + $"/{directory.Replace("\\", "/").Split("/").Last().Replace(".jar", "")}.png"))
                    {
                        if (ZipFile.Open(directory, ZipArchiveMode.Read).Entries.FirstOrDefault(x => x.Name.Contains("icon.png") || x.Name.Contains("logo.png")) != null)
                        {
                            modIcon = App.AppDataPath + $"/{directory.Replace("\\", "/").Split("/").Last().Replace(".jar", "")}.png";
                            await Task.Run(() =>
                            {
                                ZipFile.Open(directory, ZipArchiveMode.Read).Entries.FirstOrDefault(x => x.Name.Contains("icon.png") || x.Name.Contains("logo.png")).ExtractToFile(App.AppDataPath + $"/{directory.Replace("\\", "/").Split("/").Last().Replace(".jar", "")}.png", true);
                            });
                        }
                    }
                    else
                        modIcon = App.AppDataPath + $"/{directory.Replace("\\", "/").Split("/").Last().Replace(".jar", "")}.png";

                    ModsListBox.Items.Add(new UserControls.ModUserControl(modIcon, directory.Replace("\\", "/").Split("/").Last().Replace(".jar", "")));
                };
            }
            return;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (Directory.Exists($"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}/.minecraft/mods"))
                defaultModsDerectory = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}/.minecraft/mods";

            Refresh();
        }

        private void VersionsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string directory = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}/.minecraft/versions/{VersionsComboBox.SelectedItem}/mods";

            if (Directory.Exists(directory))
                defaultModsDerectory = directory;
            else
                Windows.MessageBox.Show("Модпак не найден. Если ошибка повторится - перезапустите приложение", "Ошибка");

            if (VersionsComboBox.SelectedIndex != 0)
                Refresh();
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            Helpers.PageManager.Navigate(new SettingsPage(), "Настройки");
        }

        private async void ModsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedModName = ((UserControls.ModUserControl)ModsListBox.SelectedItem).ModNameTextBlock.Text;

            await GetRecipes((UserControls.ModUserControl)ModsListBox.SelectedItem);

            RefreshModsListBox();

            await GetRecipesImages((UserControls.ModUserControl)ModsListBox.SelectedItem);

            RefreshModsListBox();
        }
    }
}
