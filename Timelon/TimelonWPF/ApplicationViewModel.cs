﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows;
using TimelonCl;
using TimelonCl.Data;
using TimelonWPF.Core;

namespace TimelonWPF
{
    public class ExtendedCard : Card
    {
        private int _parentListId;
        private string _parentListName;

        public int ParentId
        {
            get { return _parentListId; }
            set { _parentListId = value; }
        }
        public string ParentName
        {
            get { return _parentListName; }
            set { _parentListName = value; }
        }
        public ExtendedCard(int listId,string listName,int cardId,string cardName,DateTimeContainer date) : base(cardId, cardName, date)
        {
            _parentListId = listId;
            _parentListName = listName;
        }
    }


    public class ApplicationViewModel : INotifyPropertyChanged
    {
        #region Fields
        /// <summary>
        /// Выбранная карта
        /// </summary>
        private Card _selectedCard;

        /// <summary>
        /// Выбранный список карт
        /// </summary>
        private CardList _selectedList;

        /// <summary>
        /// Текущий менеджер списков
        /// </summary>
        private Manager _listManager;


        /// <summary>
        /// Выбранная расширенная карта
        /// </summary>
        private ExtendedCard _selectedExtendedCard;

        /// <summary>
        /// Список расширенных карт
        /// </summary>
        private List<ExtendedCard> _extendedCardList;

        /// <summary>
        /// Коллекция карт - результата работы поиска
        /// </summary>
        private ObservableCollection<ExtendedCard> _extendedCards = new ObservableCollection<ExtendedCard>();

        /// <summary>
        /// Коллекция списков
        /// </summary>
        private ObservableCollection<CardList> _lists = new ObservableCollection<CardList>();


        /// <summary>
        /// Коллекция важных карт, отсортированных по дате
        /// </summary>
        private ObservableCollection<Card> _importantCards = new ObservableCollection<Card>();

        /// <summary>
        /// Коллекция невыполненных карт, отсортированных по дате
        /// </summary>
        private ObservableCollection<Card> _defaultCards = new ObservableCollection<Card>();

        /// <summary>
        /// Коллекция выполненных карт, отсортированных по дате
        /// </summary>
        private ObservableCollection<Card> _doneCards = new ObservableCollection<Card>();

        //Для save
        public bool Need_Save=false;

        public void NeedSave()
        {
            Need_Save = true;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Доступ к коллекции списков
        /// </summary>
        public ObservableCollection<CardList> Lists { get { return _lists; } set { _lists = value; } }

        /// <summary>
        /// Доступ к менеджеру списков
        /// </summary>
        public Manager ListManager { get { return _listManager; } set { _listManager = value; } }

        /// <summary>
        /// Доступ к важным задачам
        /// </summary>
        public ObservableCollection<Card> ImportantCards
        {
            get
            {
                return _importantCards;
            }
            set
            {
                _importantCards = value;
                OnPropertyChanged("ImportantCards");
            }
        }

        /// <summary>
        /// Доступ к невыполненным задачам
        /// </summary>
        public ObservableCollection<Card> DefaultCards
        {
            get
            {
                return _defaultCards;
            }
            set
            {
                _defaultCards = value;
                OnPropertyChanged("DefaultCards");
            }
        }
        /// <summary>
        /// Доступ к выполненным задачам
        /// </summary>
        public ObservableCollection<Card> DoneCards
        {
            get
            {
                return _doneCards;
            }
            set
            {
                _doneCards = value;
                OnPropertyChanged("DoneCards");
            }
        }
        /// <summary>
        /// Доступ к выбранной карте
        /// </summary>
        public Card SelectedCard
        {
            get { return _selectedCard; }
            set
            {
                _selectedCard = value;
                OnPropertyChanged("SelectedCard");
            }
        }

        /// <summary>
        /// Доступ к выбранному списку
        /// </summary>
        public CardList SelectedList
        {
            get { return _selectedList; }
            set
            {
                _selectedList = value;

                //Перезаполняем коллекции карт выбранного списка
                ImportantCards = new ObservableCollection<Card>(_selectedList.GetListImportant());
                DefaultCards = new ObservableCollection<Card>(_selectedList.GetListDefault());
                DoneCards = new ObservableCollection<Card>(_selectedList.GetListCompleted());


                OnPropertyChanged("SelectedList");
            }
        }

        /// <summary>
        /// Доступ к выбранной карте
        /// </summary>
        public ExtendedCard SelectedExtendedCard
        {
            get { return _selectedExtendedCard; }
            set
            {
                _selectedExtendedCard = value;
                OnPropertyChanged("SelectedExtendedCard");
            }
        }

        /// <summary>
        /// Доступ к невыполненным задачам
        /// </summary>
        public ObservableCollection<ExtendedCard> ExtendedCards
        {
            get
            {
                return _extendedCards;
            }
            set
            {
                _extendedCards = value;
                OnPropertyChanged("ExtendedCards");
            }
        }

        #endregion

        #region Commands
        /// <summary>
        /// Команда отметки задачи(карточки) как важной
        /// </summary>
        private RelayCommand cardImportantCommand;
        /// <summary>
        /// Отметить задачу как важную
        /// </summary>
        public RelayCommand CardImportantCommand
        {
            get
            {
                return cardImportantCommand ??
                    (cardImportantCommand = new RelayCommand(obj =>
                    {
                        Card iCard = obj as Card;
                        if (iCard != null)
                        {
                            if (!iCard.IsImportant)
                            {
                                iCard.IsImportant = true;
                                SelectedList.Set(iCard);

                                ImportantCards = new ObservableCollection<Card>(_selectedList.GetListImportant());
                                DefaultCards = new ObservableCollection<Card>(SelectedList.GetListDefault());
                                DoneCards = new ObservableCollection<Card>(SelectedList.GetListCompleted());
                            }
                        }
                    }));
            }
        }
        /// <summary>
        /// Команда отметки задачи(карточки) как не важной
        /// </summary>
        private RelayCommand cardUndoImportantCommand;
        /// <summary>
        /// Отменить отметку о важности
        /// </summary>
        public RelayCommand CardUndoImportantCommand
        {
            get
            {
                return cardUndoImportantCommand ??
                    (cardUndoImportantCommand = new RelayCommand(obj =>
                    {
                        Card iCard = obj as Card;
                        if (iCard != null)
                        {
                            if (iCard.IsImportant)
                            {
                                iCard.IsImportant = false;
                                SelectedList.Set(iCard);

                                ImportantCards = new ObservableCollection<Card>(_selectedList.GetListImportant());
                                DefaultCards = new ObservableCollection<Card>(SelectedList.GetListDefault());
                                DoneCards = new ObservableCollection<Card>(SelectedList.GetListCompleted());
                            }
                        }
                    }));
            }
        }

        /// <summary>
        /// Команда завершения задачи(карточки)
        /// </summary>
        private RelayCommand cardDoneCommand;
        /// <summary>
        /// Выполнить задачу
        /// </summary>
        public RelayCommand CardDoneCommand
        {
            get
            {
                return cardDoneCommand ??
                    (cardDoneCommand = new RelayCommand(obj =>
                    {
                        Card completed = obj as Card;
                        if (completed!=null)
                        if (!completed.IsCompleted)
                        {
                            completed.IsCompleted = true;
                            SelectedList.Set(completed);    //Обновляем карту в списке

                            ImportantCards = new ObservableCollection<Card>(_selectedList.GetListImportant());
                            DefaultCards = new ObservableCollection<Card>(SelectedList.GetListDefault());
                            DoneCards = new ObservableCollection<Card>(SelectedList.GetListCompleted());
                        }

                    }));
            }
        }
        /// <summary>
        /// Команда восстановления задачи(карточки) из выполненных в невыполненные
        /// </summary>
        private RelayCommand cardRecoverCommand;
        /// <summary>
        /// Восстановить задачу(карту) в невыполненные
        /// </summary>
        public RelayCommand CardRecoverCommand
        {
            get
            {
                return cardRecoverCommand ??
                    (cardRecoverCommand = new RelayCommand(obj =>
                    {
                        Card iCard = obj as Card;
                        if (iCard != null)
                        {
                            if (iCard.IsCompleted)
                            {
                                iCard.IsCompleted = false;
                                SelectedList.Set(iCard);

                                ImportantCards = new ObservableCollection<Card>(_selectedList.GetListImportant());
                                DefaultCards = new ObservableCollection<Card>(SelectedList.GetListDefault());
                                DoneCards = new ObservableCollection<Card>(SelectedList.GetListCompleted());
                            }
                        }
                    }));
            }
        }

        /// <summary>
        /// Команда удаления карты из списка
        /// </summary>
        private RelayCommand removeCommand;
        
        /// <summary>
        /// Удаление карты из списка
        /// </summary>
        public RelayCommand RemoveCommand
        {
            get
            {
                return removeCommand ??
                    (removeCommand = new RelayCommand(obj =>
                    {
                        Card rCard = obj as Card;
                        if (rCard != null)
                        {
                            if (rCard.IsCompleted)
                            {
                                DoneCards.Remove(rCard);
                                SelectedList.Remove(rCard.Id);
                            }
                            else if(rCard.IsImportant)
                            {
                                ImportantCards.Remove(rCard);
                                SelectedList.Remove(rCard.Id);
                            }
                            else
                            {
                                DefaultCards.Remove(rCard);
                                SelectedList.Remove(rCard.Id);
                            }
                        }
                    },
                    (obj) => DefaultCards.Count > 0 || DoneCards.Count > 0 || ImportantCards.Count > 0));  //Удаляем карты, только если они есть в списке
            }
        }


        /// <summary>
        /// Команда добавления нового списка
        /// </summary>
        private RelayCommand addListCommand;
        /// <summary>
        /// Добавить новый список
        /// </summary>
        public RelayCommand AddListCommand
        {
            get
            {
                return addListCommand ??
                    (addListCommand = new RelayCommand(obj =>
                    {
                        TextBox tmp = obj as TextBox;
                        CardList newList = new CardList(tmp.Text);
                        ListManager.SetList(newList);
                        Lists.Add(newList);
                        SelectedList = newList;
                    }));
            }
        }

        /// <summary>
        /// Команда добавления новой карты
        /// </summary>
        private RelayCommand addCardCommand;
        /// <summary>
        /// Добавить новую карту
        /// </summary>
        public RelayCommand AddCardCommand
        {
            get
            {
                return addCardCommand ??
                    (addCardCommand = new RelayCommand(obj =>
                    {
                        TextBox tmp = obj as TextBox;
                        Card newCard = new Card(tmp.Text);
                        SelectedList.Set(newCard);
                        DefaultCards.Add(newCard);
                        SelectedCard = newCard;
                    }));
            }
        }

        /// <summary>
        /// Команда поиска карты по содержимому
        /// </summary>
        private RelayCommand searchCardCommand;
        /// <summary>
        /// Поиск карты по содержимому
        /// </summary>
        public RelayCommand SearchCardCommand
        {
            get
            {
                return searchCardCommand ??
                    (searchCardCommand = new RelayCommand(obj =>
                    {
                        TextBox tmp = obj as TextBox;
                        _extendedCardList = new List<ExtendedCard>();
                        foreach (KeyValuePair<int, CardList> item in _listManager.All)
                        {
                            List<Card> searchResult = item.Value.SearchByContent(tmp.Text);
                            foreach (Card card in searchResult)
                                _extendedCardList.Add(new ExtendedCard(item.Value.Id, item.Value.Name, card.Id, card.Name, card.Date));
                        }

                        ExtendedCards = new ObservableCollection<ExtendedCard>(_extendedCardList);
                    }));
            }
        }
        #endregion


        /// <summary>
        /// Конструктор по умолчанию
        /// </summary>
        public ApplicationViewModel()
        {
            //Загрузка данных из файла .xml
            ListManager = Manager.Instance;

            //Изначально выбран первый список - по умолчанию "Задачи"
            SelectedList = ListManager.All[0];

            //Загрузка списков в коллекцию
            Lists = new ObservableCollection<CardList>(ListManager.All.Values);

            //Загрузка выполненных и невыполненных задач в соответствующие коллекции
            ImportantCards = new ObservableCollection<Card>(_selectedList.GetListImportant());
            DefaultCards = new ObservableCollection<Card>(SelectedList.GetListDefault());
            DoneCards = new ObservableCollection<Card>(SelectedList.GetListCompleted());


        }


        /// <summary>
        /// Определение интерфейса событий
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
