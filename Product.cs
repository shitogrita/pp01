namespace pp01
{
    public class Product
    {
        public string IdProduct { get; set; } = string.Empty;

        public string ProductName { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public int IdCategory { get; set; }

        public string CategoryName { get; set; } = string.Empty;

        public int IdSupplier { get; set; }

        public string SupplierName { get; set; } = string.Empty;
    }
}
