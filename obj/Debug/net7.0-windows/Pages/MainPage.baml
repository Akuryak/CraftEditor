   M S B A M L   `   `   `  ����F  BCraftEditor, Version=0.0.1.0, Culture=neutral, PublicKeyToken=null5clr-namespace:CraftEditor.PagesCraftEditor.Pages  5           CraftEditor.Pages.MainPage   5      W SPresentationCore, Version=7.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=7.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=7.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=7.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=7.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      1localclr-namespace:CraftEditor.Pages     5      -   5          N� Title$  MainPage��5	       N� 
Background
 �����.��5      �  � ColumnDefinitions 5   
   �� 5      $��0.01* �6   �� 5      �� 5      $��0.01* �6    � RowDefinitions 5   
   �� 5      $��0.01* �6   �� 5      �� 5      $��0.01* �6   .+�5   
   � $��1��6   $��1��6    5      �� 5      $
��auto �6    �� 5      .+�5      � -   5         �Name$ ToolBarGrid��5      $��0��6(    5      �� 5      $
��auto �6'   �� 5      $
��auto �6'   �� 5      �� 5       $
��auto �6'   .+�5"      �� $%��Название сборки: ��6   $��Center=�65   �� 5#      -   5         ��Name$ VersionsComboBox��5#      $��0��65   $��1��6B   6R   �� 5%      -   5         ��Name$ SettingsButton��5%      $��Настройки��61   $��3��6E   6U   5&      � 5(      -   5        $ ContentGrid��5(      $��1��6(    5)      �� 5*      $
��auto �6'   �� 5+      $
��auto �6'   �� 5,      $
��auto �6'   �� 5-      �� 5.      $
��auto �6'   .+�50      �� -   5        $ 
BackButton��50      
������ $y�	Collapsed=�6F   $	��Top=�6]   $��15q�6u   $��1��6�   .��51      �� $#��../Assets/Icons/back-icon.png��6   $��50��6C   $��50��6N   52      �� 53      -   5         ��Name$ ModsListBox��53      $	��Top=�6/   $��Stretch=�6G   $��0��6l   6|   �� 55      -   5        $ ModRecipeListBox��55      $��0,75,0,0q�64   $��2��6F   $��0��6Z   $��Stretch=�6g   $y�	Collapsed=�6�   R� 56      -	   5         R�Name$ ItemsAndBlocksTreeView��56      $��4��6;   .@�57      Q� $��Загрузка...��6#   $��Center=�68   58      59      5:      5;      5<      