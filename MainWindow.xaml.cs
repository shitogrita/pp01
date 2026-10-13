using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Windows;

namespace pp01
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            LoadProducts();
        }

        private void LoadProducts()
        {
            try
            {
                List<Product> products = new List<Product>();

                // using автоматически освобождает подключение после использования.
                using SqlConnection connection = new SqlConnection(Database.ConnectionString);

                connection.Open();

                string query = @"
                    SELECT
                        p.IdProduct,
                        p.ProductName,
                        p.Price,
                        p.IdCategory,
                        c.CategoryName,
                        p.IdSupplier,
                        s.SupplierName
                    FROM Product p
                    INNER JOIN Category c
                        ON p.IdCategory = c.IdCategory
                    INNER JOIN Supplier s
                        ON p.IdSupplier = s.IdSupplier
                    ORDER BY p.IdProduct;
                ";

                using SqlCommand command = new SqlCommand(query, connection);
                using SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Product product = new Product
                    {
                        IdProduct = reader["IdProduct"].ToString() ?? string.Empty,
                        ProductName = reader["ProductName"].ToString() ?? string.Empty,
                        Price = Convert.ToDecimal(reader["Price"]),
                        IdCategory = Convert.ToInt32(reader["IdCategory"]),
                        CategoryName = reader["CategoryName"].ToString() ?? string.Empty,
                        IdSupplier = Convert.ToInt32(reader["IdSupplier"]),
                        SupplierName = reader["SupplierName"].ToString() ?? string.Empty
                    };

                    products.Add(product);
                }

                ProductsGrid.ItemsSource = null;
                ProductsGrid.Items.Clear();
                ProductsGrid.ItemsSource = products;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Ошибка загрузки",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }

        private void AddProduct_Click(object sender, RoutedEventArgs e)
        {
            ProductWindow window = new ProductWindow
            {
                Owner = this
            };

            if (window.ShowDialog() == true)
            {
                LoadProducts();
            }
        }

        private void EditProduct_Click(object sender, RoutedEventArgs e)
        {
            Product? selectedProduct = ProductsGrid.SelectedItem as Product;

            if (selectedProduct == null)
            {
                MessageBox.Show(
                    "Выберите товар.",
                    "Редактирование",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );

                return;
            }

            ProductWindow window = new ProductWindow(selectedProduct)
            {
                Owner = this
            };

            if (window.ShowDialog() == true)
            {
                LoadProducts();
            }
        }

        private void DeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            Product? selectedProduct = ProductsGrid.SelectedItem as Product;

            if (selectedProduct == null)
            {
                MessageBox.Show(
                    "Выберите товар.",
                    "Удаление",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );

                return;
            }

            MessageBoxResult result = MessageBox.Show(
                "Удалить выбранный товар?",
                "Удаление",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning
            );

            if (result != MessageBoxResult.Yes)
            {
                return;
            }

            try
            {
                using SqlConnection connection = new SqlConnection(Database.ConnectionString);

                connection.Open();

                string query = @"
                    DELETE FROM Product
                    WHERE IdProduct = @IdProduct;
                ";

                using SqlCommand command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue(
                    "@IdProduct",
                    selectedProduct.IdProduct
                );

                command.ExecuteNonQuery();

                LoadProducts();
            }
            catch (SqlException)
            {
                MessageBox.Show(
                    "Не удалось удалить товар. Возможно, он используется в другой таблице.",
                    "Ошибка удаления",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }
    }
}