using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Champion.ViewModels;

namespace Champion.Views;

public partial class BracketEditorView : UserControl
{
    public BracketEditorView()
    {
        InitializeComponent();
        
        AddHandler(DragDrop.DragOverEvent, DragOver);
        AddHandler(DragDrop.DropEvent, Drop);
    }
    
    public void ShowBracket(IEnumerable<Competitor> bracket)
    {
        BracketGrid.Children.Clear();

        var splitList = Utils.SplitCompetitors(bracket.Count(), App.AppConfig.MaxCompetitorsPerGroup);
        var splitTypeIsRound = (bool)radioButtonLimitRound.IsChecked!;
        if (splitTypeIsRound) 
            splitList = Utils.SplitCompetitors(bracket.Count(), App.AppConfig.MaxCompetitorsPerRoundGroup);

        var index = 0;
        var rowIndex = 0;
        foreach (var competitor in bracket)
        {
            var textBlock = new TextBlock();
            textBlock.Text = $"{competitor.SortId}. " + competitor.GetFullName();
            textBlock.Tag = competitor;
            textBlock.PointerPressed += OnPointerPressed;
            textBlock.SetValue(DragDrop.AllowDropProperty, true);

            var contextMenu = new ContextMenu();

            var editMenuItem = new MenuItem();
            editMenuItem.Header = "Изменить";
            editMenuItem.Click += OpenEditCompetitorWindow;
            contextMenu.Items.Add(editMenuItem);

            var deleteMenuItem = new MenuItem();
            deleteMenuItem.Header = "Удалить";
            deleteMenuItem.Click += DeleteCompetitor;
            contextMenu.Items.Add(deleteMenuItem);

            textBlock.ContextMenu = contextMenu;

            textBlock.Padding = new Thickness(5);
            textBlock.Width = 350;
            // textBlock.VerticalAlignment = VerticalAlignment.Center;

            textBlock.DataContext = competitor;

            if (splitList[0] == index && index > 1)
            {
                splitList.RemoveAt(0);
                index = 0;
                var separator = new Separator();
                separator.Height = 5;
                separator.Width = 350;
                separator.Margin = new Thickness(10, 10, 10, 0);
                separator.Background = new SolidColorBrush(Colors.White);
                Grid.SetRow(separator, rowIndex);
                BracketGrid.Children.Add(separator);
                rowIndex++;
            }

            if (splitTypeIsRound)
            {
                textBlock.Margin = new Thickness(10, 10, 10, 0);
                textBlock.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0x57, 0x4E, 0xB9));
            }
            else
            {
                textBlock.Margin = new Thickness(10, 10, 10, 0);
                if (index % 2 == 1)
                {
                    textBlock.Background = new SolidColorBrush(Colors.DodgerBlue);
                }
                else
                {
                    textBlock.Background = new SolidColorBrush(Colors.DarkRed);
                    if (index > 1)
                        textBlock.Margin = new Thickness(10, 20, 10, 0);
                }
            }

            var row = new RowDefinition();
            row.Height = new GridLength(1, GridUnitType.Auto);
            BracketGrid.RowDefinitions.Add(row);

            Grid.SetRow(textBlock, rowIndex);
            BracketGrid.Children.Add(textBlock);
            rowIndex++;

            index++;
        }
    }

    private void CategoriesListBox_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        var data = CategoriesListBox.SelectedItem?.ToString();
        if (!String.IsNullOrEmpty(data))
            ShowBracket(App.CompetitorManager.GetBracket(data));
    }
    
    private async void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (!e.GetCurrentPoint(this).Properties.IsLeftButtonPressed) return;
        
        if (sender is not TextBlock textblock) return;
        if (textblock.DataContext is not Competitor competitor) return;

        if (DataContext is not BracketEditorViewModel vm) return;

        var dragData = new DataObject();
        dragData.Set("competitor", competitor);
        
        await DragDrop.DoDragDrop(e, dragData, DragDropEffects.Move);
    }

    private void DragOver(object? sender, DragEventArgs e)
    {
        e.DragEffects = DragDropEffects.Move;
        
        var data = e.Data.Get("competitor");
        if (data is not Competitor srcCompetitor) return;
        
        if (e.Source is not TextBlock textblock) return;
        if (textblock.DataContext is not Competitor dstCompetitor) return;
        
        if (DataContext is not BracketEditorViewModel vm) return;
        if (srcCompetitor.SortId == dstCompetitor.SortId) e.DragEffects = DragDropEffects.None;
    }

    private void Drop(object? sender, DragEventArgs e)
    {
        var data = e.Data.Get("competitor");
        if (data is not Competitor srcCompetitor) return;
        
        if (e.Source is not TextBlock textblock) return;
        if (textblock.DataContext is not Competitor dstCompetitor)
        {
            if (textblock.DataContext is not string category) return;
            if (!App.CompetitorManager.GetCategories().Contains(category)) return;
            App.CompetitorManager.ChangeCompetitorCategory(srcCompetitor, category);
            RefreshUi();
        }
        else
        {
            if (DataContext is not BracketEditorViewModel vm) return;
            
            if (srcCompetitor.SortId == dstCompetitor.SortId) return;

            (srcCompetitor.SortId, dstCompetitor.SortId) = (dstCompetitor.SortId, srcCompetitor.SortId);
            
            RefreshUi();
        }
    }

    private async void OpenEditCompetitorWindow(object? sender, RoutedEventArgs args)
    {
        if (args.Source is not MenuItem item) return;
        if (item.DataContext is not Competitor dstCompetitor) return;
        
        // var dialog = new EditCompetitorWindow();
        // var result = await dialog.ShowDialog<BracketEditorViewModel?>(App.);
        
        RefreshUi();
    }
    
    private void DeleteCompetitor(object? sender, RoutedEventArgs args)
    {
        if (args.Source is not MenuItem item) return;
        if (item.DataContext is not Competitor dstCompetitor) return;
        App.CompetitorManager.RemoveCompetitor(dstCompetitor);
        RefreshUi();
    }

    private void RefreshUi()
    {
        CategoriesListBox.ItemsSource = App.CompetitorManager.GetCategories().OrderBy(s => s).ToList();
        
        var chosenCategory = CategoriesListBox.SelectedItem?.ToString();
        var chosenBracket = App.CompetitorManager.GetBracket(chosenCategory);
        if (chosenBracket.Count > 0)
            ShowBracket(chosenBracket);
        else
            BracketGrid.Children.Clear();
    }
}