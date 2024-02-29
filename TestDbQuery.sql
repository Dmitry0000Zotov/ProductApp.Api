-- Создание базы данных
CREATE DATABASE [TestDb]
 CONTAINMENT = NONE
 ON  PRIMARY 
 ( NAME = N'TestDb', 
   FILENAME = N'D:\repos\TestDb.mdf' , 
   SIZE = 8192KB , 
   MAXSIZE = UNLIMITED, 
   FILEGROWTH = 65536KB )
 LOG ON 
 ( NAME = N'TestDb _log', 
   FILENAME = N'D:\repos\TestDb_log.ldf' , 
   SIZE = 8192KB , 
   MAXSIZE = 2048GB , 
   FILEGROWTH = 65536KB )
GO


-- Использование новой базы данных
USE TestDb;
GO

-- Создание таблицы Product
CREATE TABLE Product (
    ID uniqueidentifier NOT NULL,
    Name nvarchar(255) NOT NULL,
    Description nvarchar(MAX) NULL,
    CONSTRAINT PK_Product PRIMARY KEY CLUSTERED (ID),
    CONSTRAINT UQ_Product_Name UNIQUE NONCLUSTERED (Name)
);
GO

-- Создание таблицы ProductVersion
CREATE TABLE ProductVersion (
    ID uniqueidentifier NOT NULL,
    ProductID uniqueidentifier NOT NULL,
    Name nvarchar(255) NOT NULL,
    Description nvarchar(MAX) NULL,
    CreatingDate datetime NOT NULL DEFAULT GETDATE(),
    Width decimal(10, 2) NOT NULL,
    Height decimal(10, 2) NOT NULL,
    Length decimal(10, 2) NOT NULL,
    CONSTRAINT PK_ProductVersion PRIMARY KEY CLUSTERED (ID),
    CONSTRAINT FK_ProductVersion_Product FOREIGN KEY (ProductID) REFERENCES Product (ID) ON DELETE CASCADE
);
GO

-- Создание таблицы EventLog
CREATE TABLE EventLog (
    ID uniqueidentifier NOT NULL,
    EventDate datetime NOT NULL DEFAULT GETDATE(),
    Description nvarchar(MAX) NULL,
    CONSTRAINT PK_EventLog PRIMARY KEY CLUSTERED (ID)
);
GO

-- Создание индексов для таблицы Product
CREATE NONCLUSTERED INDEX IX_Product_Name ON Product (Name) 
WITH (PAD_INDEX = OFF, 
	STATISTICS_NORECOMPUTE = OFF, 
	SORT_IN_TEMPDB = OFF, 
	DROP_EXISTING = OFF, 
	ONLINE = OFF, 
	ALLOW_ROW_LOCKS = ON, 
	ALLOW_PAGE_LOCKS = ON);
GO

-- Создание индексов для таблицы ProductVersion
CREATE NONCLUSTERED INDEX IX_ProductVersion_Name ON ProductVersion (Name) 
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON);

CREATE NONCLUSTERED INDEX IX_ProductVersion_CreatingDate ON ProductVersion (CreatingDate) 
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON);

CREATE NONCLUSTERED INDEX IX_ProductVersion_Width ON ProductVersion (Width) 
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON);

CREATE NONCLUSTERED INDEX IX_ProductVersion_Height ON ProductVersion (Height) 
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON);

CREATE NONCLUSTERED INDEX IX_ProductVersion_Length ON ProductVersion (Length) 
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON);
GO

-- Создание индекса для таблицы EventLog
CREATE NONCLUSTERED INDEX IX_EventLog_EventDate ON EventLog (EventDate) 
WITH (PAD_INDEX = OFF, 
	STATISTICS_NORECOMPUTE = OFF, 
	SORT_IN_TEMPDB = OFF, 
	DROP_EXISTING = OFF, 
	ONLINE = OFF, 
	ALLOW_ROW_LOCKS = ON, 
	ALLOW_PAGE_LOCKS = ON);
GO

-- Создание триггеров для таблиц Product и ProductVersion
CREATE TRIGGER Product_InsertUpdateDeleteTrigger ON Product
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    DECLARE @EventType nvarchar(10);
    SET @EventType = CASE
        WHEN EXISTS (SELECT * FROM inserted) THEN 'INSERT'
        WHEN EXISTS (SELECT * FROM deleted) THEN 'DELETE'
        ELSE 'UPDATE'
    END;

INSERT INTO EventLog (ID, EventDate, Description)
    SELECT NEWID(), GETDATE(), CONCAT(N'Версия изделия ', @EventType, ': ', P.Name)
    FROM Product P
    WHERE EXISTS (SELECT * FROM inserted) OR EXISTS (SELECT * FROM deleted);
END;
GO

CREATE TRIGGER ProductVersion_InsertUpdateDeleteTrigger ON ProductVersion
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    DECLARE @EventType nvarchar(10);
    SET @EventType = CASE
        WHEN EXISTS (SELECT * FROM inserted) THEN 'INSERT'
        WHEN EXISTS (SELECT * FROM deleted) THEN 'DELETE'
        ELSE 'UPDATE'
    END;

INSERT INTO EventLog (ID, EventDate, Description)
    SELECT NEWID(), GETDATE(), CONCAT(N'Версия изделия ', @EventType, ': ', PV.Name)
    FROM ProductVersion PV
    WHERE EXISTS (SELECT * FROM inserted) OR EXISTS (SELECT * FROM deleted);
END;
GO

-- Создание хранимой функции для поиска версий изделия
CREATE FUNCTION SearchProductVersion (@Name nvarchar(255), @VersionName nvarchar(255), @MinWidth decimal(10, 2), @MaxWidth decimal(10, 2), @MinHeight decimal(10, 2), @MaxHeight decimal(10, 2), @MinLength decimal(10, 2), @MaxLength decimal(10, 2))
RETURNS TABLE
AS
RETURN
(
    SELECT PV.ID, P.Name, PV.Name as VersionName, PV.Width, PV.Height, PV.Length
    FROM Product P
    INNER JOIN ProductVersion PV ON P.ID = PV.ProductID
    WHERE (@Name IS NULL OR P.Name LIKE '%' + @Name + '%')
	AND (@VersionName IS NULL OR PV.Name LIKE '%' + @VersionName + '%')
    AND (@MinWidth IS NULL OR PV.Width >= @MinWidth)
    AND (@MaxWidth IS NULL OR PV.Width <= @MaxWidth)
    AND (@MinHeight IS NULL OR PV.Height >= @MinHeight)
    AND (@MaxHeight IS NULL OR PV.Height <= @MaxHeight)
    AND (@MinLength IS NULL OR PV.Length >= @MinLength)
    AND (@MaxLength IS NULL OR PV.Length <= @MaxLength)
);
GO

-- Заполнение таблиц Product и ProductVersion тестовыми данными
INSERT INTO Product (ID, Name) 
VALUES ('cbcbbb88-7e40-4fed-9806-8395854cc656', N'Продукт 1'), 
	('c76b1419-4df3-4f04-b891-dd88cadbab90', N'Продукт 2'), 
	('c23697f1-6135-4e7b-9fca-2b33e09d83a2', N'Продукт 3');
GO

INSERT INTO ProductVersion (ID, ProductID, Name, Width, Height, Length) 
VALUES ('cd1b4760-86c5-474e-807c-81cc72c6cb32', 'cbcbbb88-7e40-4fed-9806-8395854cc656', N'Версия 1', '10', '20', '30'), 
	('9d32f665-0c06-4ebf-b627-93c7ef7bd2e9', 'cbcbbb88-7e40-4fed-9806-8395854cc656', N'Версия 2', '20', '30', '40'), 
	('75e5ca7e-d7c0-4dbd-a5f9-45d3fd49c911', 'c76b1419-4df3-4f04-b891-dd88cadbab90', N'Версия 1', '30', '40', '50');
GO

-- Проверка работы триггеров
INSERT INTO Product (ID, Name) 
VALUES ('55626d3c-3568-40f3-9403-03574684de73', N'Продукт 4');
GO

INSERT INTO ProductVersion (ID, ProductID, Name, Width, Height, Length) 
VALUES ('8a3a45fa-687c-4bf4-af0c-f20785b58733', '55626d3c-3568-40f3-9403-03574684de73', N'Версия 1', '40', '50', '60');
GO

SELECT * FROM Product;
SELECT * FROM ProductVersion;
SELECT * FROM EventLog;

-- Проверка работы хранимой функции
SELECT * FROM SearchProductVersion (N'Продукт 1', NULL, NULL, NULL, NULL, NULL, NULL, NULL);
GO

-- Удаление тестовых данных
DELETE FROM Product WHERE ID = '55626d3c-3568-40f3-9403-03574684de73';
GO

DELETE FROM Product WHERE ID IS NOT NULL;
DELETE FROM ProductVersion WHERE ID = 'cd1b4760-86c5-474e-807c-81cc72c6cb32';
DELETE FROM EventLog WHERE ID IS NOT NULL;












-- Удаление таблиц и индексов
DROP TABLE Product;
DROP TABLE ProductVersion;
DROP TABLE EventLog;
GO

-- Удаление базы данных
DROP DATABASE TestDb;