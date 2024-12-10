using System;
using System.Collections.Generic;
using System.Windows;
using MySql.Data.MySqlClient;

namespace IngatlanApp
{
    public partial class MainWindow : Window
    {
        private readonly string connectionString = "Server=localhost;Database=ingatlan;Uid=root;Pwd=;";

        public MainWindow()
        {
            InitializeComponent();
            PopulateCategoryComboBox();
        }

        private void PopulateCategoryComboBox()
        {
            var categories = new Dictionary<int, string> {
                {1, "Ház"},
                {2, "Lakás"},
                {3, "Építési telek"},
                {4, "Garázs" },
                {5, "Mezőgazdasági terület" },
                {6, "Ipari ingatlan" }
            };
            CategoryComboBox.ItemsSource = categories;
            CategoryComboBox.DisplayMemberPath = "Value";
            CategoryComboBox.SelectedValuePath = "Key";
        }

        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            string description = DescriptionTextBox.Text;
            int category = (int)CategoryComboBox.SelectedValue;
            bool isTehermentes = TehermentesCheckBox.IsChecked ?? false;
            int price;
            string imageUrl = ImageUrlTextBox.Text;

            if (!int.TryParse(PriceTextBox.Text, out price))
            {
                MessageBox.Show("Az árnak számnak kell lennie!");
                return;
            }

            if(price !< 0)
            {
                MessageBox.Show("Az árnak 0-nál nagyobbnak kell lennie!");
                return;
            }

            AddRecordToDatabase(description, category, isTehermentes, price, imageUrl);
        }

        private void AddRecordToDatabase(string description, int category, bool isTehermentes, int price, string imageUrl)
        {
            try
            {
                using (MySqlConnection connection = new(connectionString))
                {
                    connection.Open();
                    string query = "INSERT INTO ingatlanok (kategoria, leiras, tehermentes, ar, kepUrl) " +
                                   "VALUES (@category, @description, @tehermentes, @price, @imageUrl)";

                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@category", category);
                    command.Parameters.AddWithValue("@description", description);
                    command.Parameters.AddWithValue("@tehermentes", isTehermentes ? 1 : 0);
                    command.Parameters.AddWithValue("@price", price);
                    command.Parameters.AddWithValue("@imageUrl", imageUrl);

                    command.ExecuteNonQuery();
                    MessageBox.Show("Adat feltöltve!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hiba történt: " + ex.Message);
            }
        }
    }
}
