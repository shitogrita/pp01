using Microsoft.Data.SqlClient;
using System;
using System.Windows;

namespace pp01
{
    public partial class ProductWindow : Window
    {
        private Product? editingProduct;

        public ProductWindow()
        {
            InitializeComponent();

            LoadCategories();
            LoadSuppliers();
        }

        public ProductWindow(Product product)
        {
            InitializeComponent();

            editingProduct = product;

            LoadCategories();
            LoadSuppliers();

            Title = "Редактирование товара";
            TitleTextBlock.Text = "Редактирование товара";

            ProductIdTextBox.Text = product.IdProduct;
            ProductNameTextBox.Text = product.ProductName;
            PriceTextBox.Text = product.Price.ToString();

            CategoryComboBox.SelectedValue = product.IdCategory;
            SupplierComboBox.SelectedValue = product.IdSupplier;

            ProductIdTextBox.IsReadOnly = true;
        }

        private void LoadCategories()
        {
            CategoryComboBox.Items.Clear();

            try
            {
                using SqlConnection connection = new SqlConnection(Database.ConnectionString);

                connection.Open();

                string query = @"
                    SELECT IdCategory, CategoryName
                    FROM Category
                    ORDER BY CategoryName;
                ";

                using SqlCommand command = new SqlCommand(query, connection);
                using SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    LookupItem item = new LookupItem
                    {
                        Id = Convert.ToInt32(reader["IdCategory"]),
                        Name = reader["CategoryName"].ToString() ?? string.Empty
                    };

                    CategoryComboBox.Items.Add(item);
                }

                if (CategoryComboBox.Items.Count > 0)
                {
                    CategoryComboBox.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Ошибка загрузки категорий",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }

        private void LoadSuppliers()
        {
            SupplierComboBox.Items.Clear();

            try
            {
                using SqlConnection connection = new SqlConnection(Database.ConnectionString);

                connection.Open();

                string query = @"
                    SELECT IdSupplier, SupplierName
                    FROM Supplier
                    ORDER BY SupplierName;
                ";

                using SqlCommand command = new SqlCommand(query, connection);
                using SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    LookupItem item = new LookupItem
                    {
                        Id = Convert.ToInt32(reader["IdSupplier"]),
                        Name = reader["SupplierName"].ToString() ?? string.Empty
                    };

                    SupplierComboBox.Items.Add(item);
                }

                if (SupplierComboBox.Items.Count > 0)
                {
                    SupplierComboBox.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Ошибка загрузки поставщиков",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ProductIdTextBox.Text))
            {
                MessageBox.Show("Введите артикул.");
                return;
            }

            if (string.IsNullOrWhiteSpace(ProductNameTextBox.Text))
            {
                MessageBox.Show("Введите название.");
                return;
            }

            if (!decimal.TryParse(PriceTextBox.Text, out decimal price))
            {
                MessageBox.Show("Введите корректную цену.");
                return;
            }

            if (price < 0)
            {
                MessageBox.Show("Цена не может быть отрицательной.");
                return;
            }

            if (CategoryComboBox.SelectedValue == null ||
                SupplierComboBox.SelectedValue == null)
            {
                MessageBox.Show("Выберите категорию и поставщика.");
                return;
            }

            int categoryId =
                Convert.ToInt32(CategoryComboBox.SelectedValue);

            int supplierId =
                Convert.ToInt32(SupplierComboBox.SelectedValue);

            try
            {
                using SqlConnection connection = new SqlConnection(Database.ConnectionString);

                connection.Open();

                if (editingProduct == null)
                {
                    InsertProduct(
                        connection,
                        categoryId,
                        supplierId,
                        price
                    );
                }
                else
                {
                    UpdateProduct(
                        connection,
                        categoryId,
                        supplierId,
                        price
                    );
                }

                DialogResult = true;

                Close();
            }
            catch (SqlException ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Ошибка сохранения",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }

        private void InsertProduct(SqlConnection connection, int categoryId, int supplierId, decimal price)
        {
            string query = @"
                INSERT INTO Product
                (
                    IdProduct,
                    ProductName,
                    Price,
                    IdCategory,
                    IdSupplier
                )
                VALUES
                (
                    @IdProduct,
                    @ProductName,
                    @Price,
                    @IdCategory,
                    @IdSupplier
                );
            ";

            using SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue(
                "@IdProduct",
                ProductIdTextBox.Text.Trim()
            );

            command.Parameters.AddWithValue(
                "@ProductName",
                ProductNameTextBox.Text.Trim()
            );

            command.Parameters.AddWithValue(
                "@Price",
                price
            );

            command.Parameters.AddWithValue(
                "@IdCategory",
                categoryId
            );

            command.Parameters.AddWithValue(
                "@IdSupplier",
                supplierId
            );

            command.ExecuteNonQuery();
        }

        private void UpdateProduct(SqlConnection connection, int categoryId, int supplierId, decimal price)
        {
            string query = @"
                UPDATE Product
                SET
                    ProductName = @ProductName,
                    Price = @Price,
                    IdCategory = @IdCategory,
                    IdSupplier = @IdSupplier
                WHERE IdProduct = @IdProduct;
            ";

            using SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue(
                "@IdProduct",
                editingProduct!.IdProduct
            );

            command.Parameters.AddWithValue(
                "@ProductName",
                ProductNameTextBox.Text.Trim()
            );

            command.Parameters.AddWithValue(
                "@Price",
                price
            );

            command.Parameters.AddWithValue(
                "@IdCategory",
                categoryId
            );

            command.Parameters.AddWithValue(
                "@IdSupplier",
                supplierId
            );

            command.ExecuteNonQuery();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;

            Close();
        }
    }
}