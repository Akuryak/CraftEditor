using Microsoft.Win32;
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
        private static string? defaultModsDirectory;
        public MainPage()
        {
            InitializeComponent();
        }

        private void RefreshItemsAndBlocks()
        {
            ItemsAndBlocksTreeView.Items.Clear();

            foreach (string directory in Directory.GetDirectories(App.AppDataPath))
            {
                string modName = directory.Replace("\\", "/").Split("/").Last();
                TreeViewItem modTreeViewItem = new TreeViewItem() { Header = modName };

                TreeViewItem blocksTreeViewItem = new TreeViewItem() { Header = "Блоки" };
                modTreeViewItem.Items.Add(blocksTreeViewItem);
                TreeViewItem itemsTreeViewItem = new TreeViewItem() { Header = "Предметы" };
                modTreeViewItem.Items.Add(itemsTreeViewItem);

                if (Directory.Exists($"{App.AppDataPath}/{modName}/Blocks"))
                {
                    foreach (string blocks in Directory.GetFiles($"{App.AppDataPath}/{modName}/Blocks"))
                    {
                        blocksTreeViewItem.Items.Add(new TreeViewItem() { Header = blocks.Replace(".json", "").Replace("\\", "/").Split("/").Last() });
                    }
                }

                if (Directory.Exists($"{App.AppDataPath}/{modName}/Items"))
                {
                    foreach (string items in Directory.GetFiles($"{App.AppDataPath}/{modName}/Items"))
                    {
                        itemsTreeViewItem.Items.Add(new TreeViewItem() { Header = items.Replace(".json", "").Replace("\\", "/").Split("/").Last() });
                    }
                }

                ItemsAndBlocksTreeView.Items.Add(modTreeViewItem);
            }
        }

        private void RefreshItemsListBox()
        {

        }

        private void RefreshBlocksListBox()
        {

        }

        private async Task GetAllItems()
        {
            foreach (string directory in Directory.GetFiles(defaultModsDirectory))
            {
                string modItems = $"{App.AppDataPath}/{directory.Replace("\\", "/").Split("/").Last().Replace(".jar", "")}/Items"; //defaultDir/ModName/Items

                List<ZipArchiveEntry> items = ZipFile.OpenRead(directory).Entries.Where(x => x.FullName.Contains("/models/item") && x.Name.EndsWith(".json")).ToList();

                if (!Directory.Exists(modItems) && items.Count > 0)
                    Directory.CreateDirectory(modItems);

                foreach (ZipArchiveEntry item in items)
                {
                    await Task.Run(() =>
                    {
                        try
                        {
                            if (!File.Exists($"{modItems}/{item.Name}"))
                            {
                                item.ExtractToFile($"{modItems}/{item.Name}");
                            }
                        }
                        catch { }
                    });
                }
            }
        }

       private async Task GetAllBlocks()
       {
            foreach (string directory in Directory.GetFiles(defaultModsDirectory))
            {
                string modBlocks = $"{App.AppDataPath}/{directory.Replace("\\", "/").Split("/").Last().Replace(".jar", "")}/Blocks"; //defaultDir/ModName/Blocks

                List<ZipArchiveEntry> blocks = ZipFile.OpenRead(directory).Entries.Where(x=>x.FullName.Contains("/models/block") && x.Name.EndsWith(".json")).ToList();

                if (!Directory.Exists(modBlocks) && blocks.Count > 0)
                        Directory.CreateDirectory(modBlocks);

                foreach (ZipArchiveEntry block in blocks)
                {
                    await Task.Run(() =>
                    {
                        try
                        {
                            if (!File.Exists($"{modBlocks}/{block.Name}"))
                            {
                                block.ExtractToFile($"{modBlocks}/{block.Name}");
                            }
                        }
                        catch { }
                    });
                }
            }
       }

        private void RefreshModsRecipesListBox()
        {
            ModRecipeListBox.Items.Clear();

            if (Directory.GetFiles($"{App.AppDataPath}/{SelectedModName}/", "*.json").Length > 1)
            {
                ModsListBox.Height = 75;
                ModsListBox.ScrollIntoView(ModsListBox.SelectedItem);

                ModRecipeListBox.Visibility = Visibility.Visible;
                BackButton.Visibility = Visibility.Visible;

                foreach (string recipe in Directory.GetFiles($"{App.AppDataPath}/{SelectedModName}/", "*.json"))
                {
                    string image = string.Empty;
                    if (File.Exists(recipe.Replace(".json", ".png")))
                        image = recipe.Replace(".json", ".png");
                    ModRecipeListBox.Items.Add(new UserControls.ModUserControl(image, recipe.Replace(".json", "").Split("/").Last()));
                }
            }
            else
            {
                ModsListBox.Height = Height;

                ModRecipeListBox.Visibility = Visibility.Collapsed;
                BackButton.Visibility = Visibility.Collapsed;
            }
                
        }

        private async Task GetRecipesImages(UserControls.ModUserControl selectedRecipe)
        {
            List<ZipArchiveEntry> images = ZipFile.OpenRead($"{defaultModsDirectory}/{SelectedModName}.jar").Entries.Where(x => x.FullName.EndsWith(".png")).ToList();

            selectedRecipe.LoadProgressBar.Visibility = Visibility.Visible;
            selectedRecipe.LoadProgressTextBlock.Visibility = Visibility.Visible;
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
                            ZipArchiveEntry? imageRecipes = ZipFile.OpenRead($"{defaultModsDirectory}/{SelectedModName}.jar").Entries.FirstOrDefault(x => x.FullName.Contains(images[i].Name.Replace(".json", ".png")));
                            imageRecipes?.ExtractToFile($"{App.AppDataPath}/{SelectedModName}/{images[i].Name.Replace(".json", ".png")}", true);
                        }
                    }
                    catch { }
                });
                if ((UserControls.ModUserControl)ModsListBox.SelectedItem == selectedRecipe)
                {
                    selectedRecipe.LoadProgressBar.Value = i;
                    selectedRecipe.LoadProgressTextBlock.Text = $"Загружено {i} из {images.Count} изображений";
                }
                else
                    return;
            }

            if ((UserControls.ModUserControl)ModsListBox.SelectedItem == selectedRecipe)
            {
                selectedRecipe.LoadProgressTextBlock.Text = $"Завершено";
                selectedRecipe.LoadProgressBar.Value = images.Count;
                selectedRecipe.LoadProgressBar.Opacity = 0.50;
            }
        }

        private async Task GetRecipes(UserControls.ModUserControl selectedRecipe)
        {
            List<ZipArchiveEntry> recipes = ZipFile.OpenRead($"{defaultModsDirectory}/{SelectedModName}.jar").Entries.Where(x => x.FullName.Contains("recipes")).Where(x => x.FullName.EndsWith(".json")).ToList();
            Directory.CreateDirectory($"{App.AppDataPath}/{SelectedModName}");

            selectedRecipe.LoadProgressBar.Visibility = Visibility.Visible;
            selectedRecipe.LoadProgressTextBlock.Visibility = Visibility.Visible;
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
                {
                    selectedRecipe.LoadProgressBar.Value = i;
                    selectedRecipe.LoadProgressTextBlock.Text = $"Загружено {i} из {recipes.Count} модов";
                }
                else
                    return;
            }

            if ((UserControls.ModUserControl)ModsListBox.SelectedItem == selectedRecipe)
            {
                selectedRecipe.LoadProgressTextBlock.Text = $"Завершена загрузка модов";
                selectedRecipe.LoadProgressBar.Value = recipes.Count;
                selectedRecipe.LoadProgressBar.Opacity = 0.50;
            }
        }

        private void SearchModPacks()
        {
            if (defaultModsDirectory != $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}/.minecraft/mods" && Directory.GetDirectories($"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}/.minecraft/versions").Length > 0)
            {
                if (defaultModsDirectory == $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}/.minecraft/mods")
                {
                    VersionsComboBox.Items.Add(new ComboBoxItem().Content = "mods");
                }

                foreach (string directory in Directory.GetDirectories($"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}/.minecraft/versions"))
                {
                    VersionsComboBox.Items.Add(new ComboBoxItem().Content = "versions/" + directory.Replace("\\", "/").Split("/").Last());
                }
                VersionsComboBox.SelectedIndex = 0;
                defaultModsDirectory = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}/.minecraft/{VersionsComboBox.SelectedItem}/mods";
            }
            else
            {
                Windows.MessageBox.Show("Сборки не найдены, укажите вручную", "Уведомление");

                OpenFileDialog openFileDialog = new OpenFileDialog()
                {
                    CheckFileExists = true,
                    CheckPathExists = true,
                    InitialDirectory = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}/.minecraft",
                    Multiselect = false,
                    Title = "Выберите сборку модов"
                };
                if (openFileDialog.ShowDialog() == true)
                {
                    defaultModsDirectory = openFileDialog.FileName;
                }
            }
        }

        private void RefreshModsListBox()
        {
            ModsListBox.Items.Clear();
            foreach (string directory in Directory.GetFiles(defaultModsDirectory))
            {
                string modIcon = string.Empty;

                if (File.Exists(App.AppDataPath + $"/{directory.Replace("\\", "/").Split("/").Last().Replace(".jar", "")}.png"))
                    modIcon = App.AppDataPath + $"/{directory.Replace("\\", "/").Split("/").Last().Replace(".jar", "")}.png";

                ModsListBox.Items.Add(new UserControls.ModUserControl(modIcon, directory.Replace("\\", "/").Split("/").Last().Replace(".jar", "")));
            }
        }

            private async void SearchMods()
        {
            if (Directory.GetFiles(defaultModsDirectory).Length > 0)
            {
                foreach (string directory in Directory.GetFiles(defaultModsDirectory))
                {
                    if (!File.Exists(App.AppDataPath + $"/{directory.Replace("\\", "/").Split("/").Last().Replace(".jar", "")}.png"))
                    {
                        if (ZipFile.Open(directory, ZipArchiveMode.Read).Entries.FirstOrDefault(x => x.Name.Contains("icon.png") || x.Name.Contains("logo.png")) != null)
                        {
                            await Task.Run(() =>
                            {
                                ZipFile.Open(directory, ZipArchiveMode.Read).Entries.FirstOrDefault(x => x.Name.Contains("icon.png") || x.Name.Contains("logo.png")).ExtractToFile(App.AppDataPath + $"/{directory.Replace("\\", "/").Split("/").Last().Replace(".jar", "")}.png", true);
                            });
                        }
                    }
                }
                RefreshModsListBox();
            }
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (Directory.Exists($"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}/.minecraft/mods"))
                defaultModsDirectory = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}/.minecraft/mods";

            SearchModPacks();
            SearchMods();

            await GetAllBlocks();
            await GetAllItems();

            RefreshItemsAndBlocks();
        }

        private void VersionsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (VersionsComboBox.SelectedIndex != 0)
            {
                SearchModPacks();
                SearchMods();
            }
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            Helpers.PageManager.Navigate(new SettingsPage(), "Настройки");
        }

        private async void ModsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((UserControls.ModUserControl)ModsListBox.SelectedItem != null)
            {
                SelectedModName = ((UserControls.ModUserControl)ModsListBox.SelectedItem).ModNameTextBlock.Text;

                await GetRecipes((UserControls.ModUserControl)ModsListBox.SelectedItem);

                RefreshModsRecipesListBox();

                await GetRecipesImages((UserControls.ModUserControl)ModsListBox.SelectedItem);

                RefreshModsRecipesListBox();
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            ModsListBox.Height = Height;

            ModRecipeListBox.Visibility = Visibility.Collapsed;
            BackButton.Visibility = Visibility.Collapsed;

            RefreshModsListBox();
        }
    }
}
