using System;
using System.Collections.Generic;
using System.Windows;
using System.IO;
using Newtonsoft.Json;
using System.Linq;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Xml;


namespace BudgetApp
{
    public class RecordAllNotes
    {
        public string NoteName { get; set; }
        public string NoteTypeName { get; set; }
        public int NoteMoney { get; set; }
        public bool NoteIsIncome { get; set; }

        public RecordAllNotes(string notename, string notetypename, int notemoney, bool noteisincome)
        {
            NoteName = notename;
            NoteTypeName = notetypename;
            NoteMoney = notemoney;
            NoteIsIncome = noteisincome;
        }
    }

    public partial class MainWindow : Window
    {
        public string PathToAllNotes = "D:\\С#\\Budget\\AllNotes.json";
        public string PathToTypes = "D:\\С#\\Budget\\Types.json";

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }
        private void AddRecord(object sender, RoutedEventArgs e)
        {
            if (!IsValidRecord())
            {
                return;
            }

            bool isIncome = int.Parse(sum.Text) > 0;
            int amount = Math.Abs(int.Parse(sum.Text));

            var data = new { Name = RecordName.Text, TypeName = Types.SelectedValue.ToString(), Money = amount, IsIncome = isIncome };
            datagrid.Items.Add(data);

            RecordName.Text = "";
            Types.SelectedValue = "";
            sum.Text = "";

            total.Content = $"Итог: {Recalculate()}";
        }

        private bool IsValidRecord()
        {
            if (string.IsNullOrWhiteSpace(RecordName.Text) || Types.SelectedValue == null)
            {
                return false;
            }

            int amount;
            if (!int.TryParse(sum.Text, out amount))
            {
                return false;
            }

            return true;
        }




        private int Recalculate()
        {
            int totalIncome = 0;
            int totalExpense = 0;
            foreach (dynamic item in datagrid.Items)
            {
                int money = (int)item.Money;
                bool isIncome = item.IsIncome;

                if (isIncome)
                {
                    totalIncome += money;
                }
                else
                {
                    totalExpense += money;
                }
            }

            int total = totalIncome - totalExpense;
            return total;
        }



        private void SaveDataToJson()
        {
            Dictionary<string, List<RecordAllNotes>> recordsByDate;
            string json = File.ReadAllText(PathToAllNotes);
            if (json.Length == 0)
            {
                recordsByDate = new Dictionary<string, List<RecordAllNotes>>();
            }
            else
            {
                recordsByDate = JsonConvert;
            }


        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SaveDataToJson();
        }



        private void calendar_SelectedDateChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0)
            {
                return;
            }

            string selectedDate = ((DateTime)e.AddedItems[0]).ToString("dd.MM.yyyy");
            string json = File.ReadAllText(PathToAllNotes);

            Dictionary<string, List<RecordAllNotes>> recordsByDate = JsonConvert.DeserializeObject<Dictionary<string, List<RecordAllNotes>>>(json);

            if (recordsByDate.TryGetValue(selectedDate, out List<RecordAllNotes> recordsForDate))
            {
                datagrid.Items.Clear();

                foreach (RecordAllNotes record in recordsForDate)
                {
                    dynamic data = new { Name = record.NoteName, TypeName = record.NoteTypeName, Money = record.NoteMoney, IsIncome = record.NoteIsIncome };
                    datagrid.Items.Add(data);
                }

                total.Content = $"Итог: {Recalculate()}";
            }
            else
            {
                datagrid.Items.Clear();
                total.Content = "Итог: 0";
            }
        }
        private void datagrid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            dynamic selectedItem = datagrid.SelectedItem;
            if (selectedItem != null)
            {
                RecordName.Text = selectedItem.Name;
                Types.SelectedValue = selectedItem.TypeName;
                sum.Text = selectedItem.Money.ToString();
            }
        }

        private void UpdateRecord(object sender, RoutedEventArgs e)
        {
            dynamic selectedItem = datagrid.SelectedItem;
            if (selectedItem == null)
            {
                return;
            }

            string name = RecordName.Text;
            string typeName = Types.SelectedValue?.ToString();
            int money = Math.Abs(int.Parse(sum.Text));
            bool isIncome = money > 0;

            var updatedItem = new { Name = name, TypeName = typeName, Money = money, IsIncome = isIncome };
            int selectedIndex = datagrid.SelectedIndex;

            datagrid.Items.RemoveAt(selectedIndex);
            datagrid.Items.Insert(selectedIndex, updatedItem);

            RecordName.Text = "";
            Types.SelectedValue = "";
            sum.Text = "";

            total.Content = $"Итог: {Recalculate()}";
        }

        private void DeleteRecord(object sender, RoutedEventArgs e)
        {
            if (datagrid.SelectedItem != null)
            {
                int selectedIndex = datagrid.SelectedIndex;
                datagrid.Items.RemoveAt(selectedIndex);

                RecordName.Text = "";
                Types.SelectedValue = "";
                sum.Text = "";
                total.Content = $"Итог: {Recalculate()}";
            }
        }
    }
}