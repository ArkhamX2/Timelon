﻿using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Data;
using Timelon.Data;

namespace Timelon.App
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ApplicationViewModel viewModel = new ApplicationViewModel();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = viewModel;

            NoCard();

            Title.MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(LayoutRoot_MouseLeftButtonDown);
            Window_Menu.MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(LayoutRoot_MouseLeftButtonDown);
        }

        #region ButtonClick

        /// <summary>
        /// События скрытия или открытия информации о карте
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            Veil_Search.Visibility = Visibility.Hidden;
            CardInfoColumn.Width = new GridLength(0);
            ExCardInfoColumn.Width = new GridLength(0);
            Sleeper();
            FromSearch();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if ((viewModel.DefaultCards.Count + viewModel.ImportantCards.Count + viewModel.DoneCards.Count) == 1) NoVisible();
            viewModel.Need_Save = true;
            CardInfoColumn.Width = new GridLength(0);
        }

        private void CardButton_Click(object sender, RoutedEventArgs e)
        {
            if (CardDescriptionTextbox.Text == "") CardDescriptionTemplate.Visibility = Visibility.Visible;
            else CardDescriptionTemplate.Visibility = Visibility.Hidden;
            DoneCardsPanel.SelectedItem = null;
            CardsPanel.SelectedItem = null;
            CardInfoColumn.Width = new GridLength(240);
        }

        private void DoneCardButton_Click(object sender, RoutedEventArgs e)
        {
            viewModel.Need_Save = true;
        }

        private void ImportantCardButton_Click(object sender, RoutedEventArgs e)
        {
            viewModel.Need_Save = true;
        }

        private void RecoverCardButton_Click(object sender, RoutedEventArgs e)
        {
            viewModel.Need_Save = true;
        }

        private void UndoImportantCardButton_Click(object sender, RoutedEventArgs e)
        {
            viewModel.Need_Save = true;
        }

        private void AddListButton_Click(object sender, RoutedEventArgs e)
        {
            Veil_Search.Visibility = Visibility.Hidden;
            CardInfoColumn.Width = new GridLength(0);
            ExCardInfoColumn.Width = new GridLength(0);
            NoVisible();
            viewModel.Need_Save = true;
            AddList();
            FromSearch();
        }

        private void AddCardButton_Click(object sender, RoutedEventArgs e)
        {
            CardInfoColumn.Width = new GridLength(240);
            YesVisible();
            viewModel.Need_Save = true;
            AddCard();
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            DeleteListButton.Visibility = Visibility.Collapsed;
            AddBorder.Visibility = Visibility.Hidden;
            AddCardButton.Visibility = Visibility.Hidden;
            AddCardTextbox.Visibility = Visibility.Hidden;
            Veil.Visibility = Visibility.Hidden;
            ExtendedCardsMenu.Visibility = Visibility.Visible;
            MainCardsMenu.Visibility = Visibility.Hidden;
            CardListName.Visibility = Visibility.Collapsed;
            SearchResult.Visibility = Visibility.Visible;
            CardInfoColumn.Width = new GridLength(0);
            SearchCard();
        }

        private void GoToListButton_Click(object sender, RoutedEventArgs e)
        {
            SearchResult.Visibility= Visibility.Hidden;
            Veil_Search.Visibility = Visibility.Hidden;
            Sleeper();
            DeleteListButton.Visibility = Visibility.Visible;
            ExtendedCardsMenu.Visibility = Visibility.Hidden;
            MainCardsMenu.Visibility = Visibility.Visible;
            CardListName.Visibility = Visibility.Visible;
            SearchResult.Visibility = Visibility.Collapsed;
            CardInfoColumn.Width = new GridLength(240);
            ExCardInfoColumn.Width = new GridLength(0);
        }

        private void DoneCardsShow_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (DoneCardsPanel.Visibility == Visibility.Hidden)
            {
                CardsPanelArrowDown.Visibility = Visibility.Hidden;
                CardspanelArrowUp.Visibility = Visibility.Visible;
                DoneCardsPanel.Visibility = Visibility.Visible;
            }
            else
            {
                CardspanelArrowUp.Visibility = Visibility.Hidden;
                CardsPanelArrowDown.Visibility = Visibility.Visible;
                DoneCardsPanel.Visibility = Visibility.Hidden;
            }
        }

        private void AddCardTextbox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                YesVisible();
                viewModel.Need_Save = true;

                CardInfoColumn.Width = new GridLength(240);
                if (viewModel.AddCardCommand.CanExecute(AddCardTextbox))
                    viewModel.AddCardCommand.Execute(AddCardTextbox);
                AddCardTextbox.Text = "";
            }
        }

        private void SearchResultCard_GotFocus(object sender, RoutedEventArgs e)
        {
            ExCardInfoColumn.Width = new GridLength(240);
        }

        private void SearchTextbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SearchButton_Click(sender, e);
                if (viewModel.SearchCardCommand.CanExecute(SearchTextbox))
                    viewModel.SearchCardCommand.Execute(SearchTextbox);

            }
        }

        private void AddListTextbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                AddListButton_Click(sender, e);

                if (viewModel.AddListCommand.CanExecute(AddListTextbox))
                    viewModel.AddListCommand.Execute(AddListTextbox);
                AddListTextbox.Text = "";
            }
        }

        #endregion ButtonClick

        #region TextChangedEvents

        //Изменение видимости текстовых полей для полей с шаблонами
        private void SearchTextbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TemplateSearch.Visibility = SearchTextbox.Text == "" ? Visibility.Visible : Visibility.Hidden;
        }

        private void AddListTextbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TemplateList.Visibility = AddListTextbox.Text == "" ? Visibility.Visible : Visibility.Hidden;
        }

        private void AddCardTextbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TemplateCard.Visibility = AddCardTextbox.Text == "" ? Visibility.Visible : Visibility.Hidden;
        }

        private void CardDateTextbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            CardDateTemplate.Visibility = Visibility.Hidden;
        }

        private void CardDescriptionTextbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            viewModel.Need_Save = true;
            if (sender is Card sCard)
            {
                viewModel.SelectedList.Set(sCard);
            }
            if (CardDescriptionTextbox.Text=="") CardDescriptionTemplate.Visibility = Visibility.Visible;
            else CardDescriptionTemplate.Visibility = Visibility.Hidden;
        }

        #endregion TextChangedEvents

        #region Window Manager Events

        /// <summary>
        /// Закрыть окно
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseApp_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }
        private void SaveChanges_Click(object sender, RoutedEventArgs e)
        {
            MainCardsMenu.Focus();
            viewModel.Need_Save = false;
            viewModel.ListManager.SaveData();
        }

        /// <summary>
        /// Скрыть окно
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HideWindow_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
            Canvas.SetZIndex(CloseButton, 1);
        }

        /// <summary>
        /// Полный экран/оконный режим
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MinimizeWindow_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Normal)
                this.WindowState = WindowState.Maximized;
            else
                this.WindowState = WindowState.Normal;
        }

        private void TaskbarIcon_TrayLeftMouseDown(object sender, RoutedEventArgs e)
        {
            this.Show();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Confirmation();
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            viewModel.Need_Save = false;
            viewModel.ListManager.SaveData();
        }

        private void Confirmation()
        {
            Window ClearWindow = new Window()
            {
                Visibility = Visibility.Collapsed,
                AllowsTransparency = true,
                Background = System.Windows.Media.Brushes.Transparent,
                WindowStyle = WindowStyle.None,
                ShowInTaskbar = false
            };
            if (viewModel.Need_Save == true)
            {
                ClearWindow.Show();
                if (MessageBox.Show(ClearWindow, "Точно хотите выйти? Все несохраненные данные будут удалены.",
                    "Выход",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    ClearWindow.Close();
                    this.Close();
                }
                else ClearWindow.Close();
            }
            else
            {
                ClearWindow.Close();
                this.Close();
            }
        }

        private void DeleteListButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Точно хотите удалить весь список?",
            "Удаление",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                DeleteListButton.Command = viewModel.RemoveListCommand;
            }
            else
            {
                DeleteListButton.Command = null;
            }
        }

        /// <summary>
        /// Перетаскиваение окна по клику мыши
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LayoutRoot_MouseLeftButtonDown(object sender, EventArgs e)
        {
            DragMove();
        }

        #endregion Window Manager Events

        #region Methods

        /// <summary>
        /// Проверка на наличие карточек в списке
        /// </summary>
        private void NoCard()
        {
            if ((viewModel.DefaultCards.Count + viewModel.ImportantCards.Count + viewModel.DoneCards.Count) != 0)
            {
                YesVisible();
            }
            else
            {
                NoVisible();
            }
        }

        /// <summary>
        /// Показать MainCardsMenu и скрыть Veil
        /// </summary>
        private void YesVisible()
        {
            DeleteListButton.Visibility = Visibility.Visible;
            AddBorder.Visibility = Visibility.Visible;
            AddCardButton.Visibility = Visibility.Visible;
            AddCardTextbox.Visibility = Visibility.Visible;
            MainCardsMenu.Visibility = Visibility.Visible;
            EndTasks.Visibility = Visibility.Visible;
            Veil.Visibility = Visibility.Hidden;
            CardListName.Visibility = Visibility.Visible;
            SearchResult.Visibility = Visibility.Hidden;
            if (viewModel.SelectedList.IsEssential) DeleteListButton.Visibility = Visibility.Hidden;
            else DeleteListButton.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Показать Veil и скрыть MainCardsMenu
        /// </summary>
        private void NoVisible()
        {
            DeleteListButton.Visibility = Visibility.Visible;
            AddBorder.Visibility = Visibility.Visible;
            AddCardButton.Visibility = Visibility.Visible;
            AddCardTextbox.Visibility = Visibility.Visible;
            MainCardsMenu.Visibility = Visibility.Hidden;
            EndTasks.Visibility = Visibility.Hidden;
            Veil.Visibility = Visibility.Visible;
            CardListName.Visibility = Visibility.Visible;
            SearchResult.Visibility = Visibility.Hidden;
            if (viewModel.SelectedList.IsEssential) DeleteListButton.Visibility = Visibility.Hidden;
            else DeleteListButton.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Задержка для проверки нового выбранного списка на наличие карточек
        /// </summary>
        private async void Sleeper()
        {
            await Task.Delay(2);
            NoCard();
        }

        async void SearchCard()
        {
            await Task.Delay(2);
            SearchTextbox.Text = "";
            if (viewModel.ExtendedCards.Count == 0) Veil_Search.Visibility = Visibility.Visible;
            else Veil_Search.Visibility = Visibility.Hidden;
        }

        async void AddList()
        {
            await Task.Delay(2);
            AddListTextbox.Text = "";
        }
        async void AddCard()
        {
            await Task.Delay(2);
            AddCardTextbox.Text = "";
        }

        void FromSearch()
        {
            if (ExtendedCardsMenu.Visibility == Visibility.Visible)
            {
                ExtendedCardsMenu.Visibility = Visibility.Hidden;
                NoVisible();
            }
        }



        #endregion Methods

        
    }
    
}