USE master;
GO

IF DB_ID('test_simple') IS NOT NULL
BEGIN
    ALTER DATABASE test_simple SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE test_simple;
END;
GO

CREATE DATABASE test_simple;
GO

USE test_simple;
GO


CREATE TABLE Category
(
    IdCategory INT PRIMARY KEY,
    CategoryName VARCHAR(100) NOT NULL UNIQUE
);
GO


CREATE TABLE Supplier
(
    IdSupplier INT PRIMARY KEY,
    SupplierName VARCHAR(100) NOT NULL,
    SupplierPhone VARCHAR(30)
);
GO


CREATE TABLE Product
(
    IdProduct VARCHAR(10) PRIMARY KEY,
    ProductName VARCHAR(150) NOT NULL,
    Price DECIMAL(10, 2) NOT NULL CHECK (Price >= 0),

    IdCategory INT NOT NULL,
    IdSupplier INT NOT NULL,

    FOREIGN KEY (IdCategory)
        REFERENCES Category(IdCategory),

    FOREIGN KEY (IdSupplier)
        REFERENCES Supplier(IdSupplier)
);
GO


INSERT INTO Category
(
    IdCategory,
    CategoryName
)
VALUES
(1, 'Спортивная обувь'),
(2, 'Зимняя обувь'),
(3, 'Классическая обувь');
GO


INSERT INTO Supplier
(
    IdSupplier,
    SupplierName,
    SupplierPhone
)
VALUES
(1, 'СпортОпт', '+79990000001'),
(2, 'ОбувьСнаб', '+79990000002'),
(3, 'СтильТорг', '+79990000003');
GO


INSERT INTO Product
(
    IdProduct,
    ProductName,
    Price,
    IdCategory,
    IdSupplier
)
VALUES
('P101', 'Кроссовки', 5990.00, 1, 1),
('P102', 'Кеды', 3490.00, 1, 2),
('P103', 'Зимние ботинки', 8990.00, 2, 2),
('P104', 'Туфли', 7490.00, 3, 3),
('P105', 'Ботинки', 6790.00, 2, 3);
GO


SELECT *
FROM Category;
GO


SELECT *
FROM Supplier;
GO


SELECT *
FROM Product;
GO


SELECT
    p.IdProduct,
    p.ProductName,
    p.Price,
    c.CategoryName,
    s.SupplierName
FROM Product p
INNER JOIN Category c
    ON p.IdCategory = c.IdCategory
INNER JOIN Supplier s
    ON p.IdSupplier = s.IdSupplier
ORDER BY p.IdProduct;
GO
