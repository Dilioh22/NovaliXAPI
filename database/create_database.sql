-- ============================================================
-- RestaurantPOS - Script de creación de base de datos
-- SQL Server
-- ============================================================

CREATE DATABASE RestaurantPOS;
GO

USE RestaurantPOS;
GO

-- ============================================================
-- 1. Categories
-- ============================================================
CREATE TABLE Categories (
    Id              INT             NOT NULL IDENTITY(1,1),
    Name            NVARCHAR(100)   NOT NULL,
    Description     NVARCHAR(500)   NULL,
    DisplayOrder    INT             NOT NULL DEFAULT 0,
    IconName        NVARCHAR(100)   NULL,
    CreatedAt       DATETIME2       NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt       DATETIME2       NULL,
    IsActive        BIT             NOT NULL DEFAULT 1,
    CONSTRAINT PK_Categories PRIMARY KEY (Id)
);

-- ============================================================
-- 2. Products
-- ============================================================
CREATE TABLE Products (
    Id              INT             NOT NULL IDENTITY(1,1),
    CategoryId      INT             NOT NULL,
    Name            NVARCHAR(200)   NOT NULL,
    Description     NVARCHAR(1000)  NULL,
    Price           DECIMAL(18,2)   NOT NULL,
    ImageUrl        NVARCHAR(500)   NULL,
    PreparationTime INT             NULL,
    IsAvailable     BIT             NOT NULL DEFAULT 1,
    DisplayOrder    INT             NOT NULL DEFAULT 0,
    CreatedAt       DATETIME2       NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt       DATETIME2       NULL,
    IsActive        BIT             NOT NULL DEFAULT 1,
    CONSTRAINT PK_Products PRIMARY KEY (Id),
    CONSTRAINT FK_Products_Categories FOREIGN KEY (CategoryId)
        REFERENCES Categories(Id) ON DELETE NO ACTION
);

-- ============================================================
-- 3. ProductModifiers
-- ============================================================
CREATE TABLE ProductModifiers (
    Id              INT             NOT NULL IDENTITY(1,1),
    ProductId       INT             NOT NULL,
    Name            NVARCHAR(100)   NOT NULL,
    PriceAdjustment DECIMAL(18,2)   NOT NULL DEFAULT 0,
    CreatedAt       DATETIME2       NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt       DATETIME2       NULL,
    IsActive        BIT             NOT NULL DEFAULT 1,
    CONSTRAINT PK_ProductModifiers PRIMARY KEY (Id),
    CONSTRAINT FK_ProductModifiers_Products FOREIGN KEY (ProductId)
        REFERENCES Products(Id) ON DELETE CASCADE
);

-- ============================================================
-- 4. TableZones
-- ============================================================
CREATE TABLE TableZones (
    Id          INT             NOT NULL IDENTITY(1,1),
    Name        NVARCHAR(100)   NOT NULL,
    Description NVARCHAR(500)   NULL,
    CreatedAt   DATETIME2       NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt   DATETIME2       NULL,
    IsActive    BIT             NOT NULL DEFAULT 1,
    CONSTRAINT PK_TableZones PRIMARY KEY (Id)
);

-- ============================================================
-- 5. Tables
-- ============================================================
CREATE TABLE Tables (
    Id          INT             NOT NULL IDENTITY(1,1),
    ZoneId      INT             NOT NULL,
    Number      INT             NOT NULL,
    Capacity    INT             NOT NULL DEFAULT 4,
    Status      NVARCHAR(50)    NOT NULL DEFAULT 'Libre',
    PositionX   INT             NULL,
    PositionY   INT             NULL,
    Shape       NVARCHAR(50)    NOT NULL DEFAULT 'rectangle',
    CreatedAt   DATETIME2       NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt   DATETIME2       NULL,
    IsActive    BIT             NOT NULL DEFAULT 1,
    CONSTRAINT PK_Tables PRIMARY KEY (Id),
    CONSTRAINT FK_Tables_TableZones FOREIGN KEY (ZoneId)
        REFERENCES TableZones(Id) ON DELETE NO ACTION
);

CREATE INDEX IX_Tables_ZoneId_Number ON Tables (ZoneId, Number);

-- ============================================================
-- 6. Users
-- ============================================================
CREATE TABLE Users (
    Id                  INT             NOT NULL IDENTITY(1,1),
    Email               NVARCHAR(200)   NOT NULL,
    PasswordHash        NVARCHAR(500)   NOT NULL,
    FirstName           NVARCHAR(100)   NOT NULL,
    LastName            NVARCHAR(100)   NOT NULL,
    Role                NVARCHAR(50)    NOT NULL,
    Pin                 NVARCHAR(10)    NULL,
    RefreshToken        NVARCHAR(500)   NULL,
    RefreshTokenExpiry  DATETIME2       NULL,
    LastLoginAt         DATETIME2       NULL,
    CreatedAt           DATETIME2       NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt           DATETIME2       NULL,
    IsActive            BIT             NOT NULL DEFAULT 1,
    CONSTRAINT PK_Users PRIMARY KEY (Id)
);

CREATE UNIQUE INDEX IX_Users_Email ON Users (Email);

-- ============================================================
-- 7. CashRegisters
-- ============================================================
CREATE TABLE CashRegisters (
    Id          INT             NOT NULL IDENTITY(1,1),
    Name        NVARCHAR(100)   NOT NULL,
    Location    NVARCHAR(200)   NULL,
    CreatedAt   DATETIME2       NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt   DATETIME2       NULL,
    IsActive    BIT             NOT NULL DEFAULT 1,
    CONSTRAINT PK_CashRegisters PRIMARY KEY (Id)
);

-- ============================================================
-- 8. CashRegisterSessions
-- ============================================================
CREATE TABLE CashRegisterSessions (
    Id               INT             NOT NULL IDENTITY(1,1),
    CashRegisterId   INT             NOT NULL,
    OpenedById       INT             NOT NULL,
    ClosedById       INT             NULL,
    OpeningAmount    DECIMAL(18,2)   NOT NULL,
    ClosingAmount    DECIMAL(18,2)   NULL,
    ExpectedAmount   DECIMAL(18,2)   NULL,
    Difference       DECIMAL(18,2)   NULL,
    TotalCash        DECIMAL(18,2)   NULL,
    TotalCard        DECIMAL(18,2)   NULL,
    TotalTransfer    DECIMAL(18,2)   NULL,
    TotalSales       DECIMAL(18,2)   NULL,
    TransactionCount INT             NULL,
    Status           NVARCHAR(50)    NOT NULL DEFAULT 'Open',
    OpenedAt         DATETIME2       NOT NULL DEFAULT GETUTCDATE(),
    ClosedAt         DATETIME2       NULL,
    Notes            NVARCHAR(MAX)   NULL,
    CreatedAt        DATETIME2       NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt        DATETIME2       NULL,
    IsActive         BIT             NOT NULL DEFAULT 1,
    CONSTRAINT PK_CashRegisterSessions PRIMARY KEY (Id),
    CONSTRAINT FK_CashRegisterSessions_CashRegisters FOREIGN KEY (CashRegisterId)
        REFERENCES CashRegisters(Id) ON DELETE NO ACTION,
    CONSTRAINT FK_CashRegisterSessions_Users_OpenedBy FOREIGN KEY (OpenedById)
        REFERENCES Users(Id) ON DELETE NO ACTION,
    CONSTRAINT FK_CashRegisterSessions_Users_ClosedBy FOREIGN KEY (ClosedById)
        REFERENCES Users(Id) ON DELETE NO ACTION
);

-- ============================================================
-- 9. Orders
-- ============================================================
CREATE TABLE Orders (
    Id              INT             NOT NULL IDENTITY(1,1),
    OrderNumber     NVARCHAR(50)    NOT NULL,
    TableId         INT             NULL,
    WaiterId        INT             NOT NULL,
    Status          NVARCHAR(50)    NOT NULL DEFAULT 'Pendiente',
    OrderType       NVARCHAR(50)    NOT NULL DEFAULT 'DineIn',
    Subtotal        DECIMAL(18,2)   NOT NULL DEFAULT 0,
    TaxRate         DECIMAL(5,4)    NOT NULL DEFAULT 0.15,
    TaxAmount       DECIMAL(18,2)   NOT NULL DEFAULT 0,
    DiscountAmount  DECIMAL(18,2)   NOT NULL DEFAULT 0,
    DiscountPercent DECIMAL(5,2)    NOT NULL DEFAULT 0,
    Total           DECIMAL(18,2)   NOT NULL DEFAULT 0,
    TipAmount       DECIMAL(18,2)   NOT NULL DEFAULT 0,
    Notes           NVARCHAR(MAX)   NULL,
    CustomerName    NVARCHAR(200)   NULL,
    CustomerPhone   NVARCHAR(50)    NULL,
    CreatedAt       DATETIME2       NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt       DATETIME2       NULL,
    IsActive        BIT             NOT NULL DEFAULT 1,
    CONSTRAINT PK_Orders PRIMARY KEY (Id),
    CONSTRAINT FK_Orders_Tables FOREIGN KEY (TableId)
        REFERENCES Tables(Id) ON DELETE NO ACTION,
    CONSTRAINT FK_Orders_Users_Waiter FOREIGN KEY (WaiterId)
        REFERENCES Users(Id) ON DELETE NO ACTION
);

CREATE UNIQUE INDEX IX_Orders_OrderNumber ON Orders (OrderNumber);

-- ============================================================
-- 10. OrderItems
-- ============================================================
CREATE TABLE OrderItems (
    Id              INT             NOT NULL IDENTITY(1,1),
    OrderId         INT             NOT NULL,
    ProductId       INT             NOT NULL,
    Quantity        INT             NOT NULL DEFAULT 1,
    UnitPrice       DECIMAL(18,2)   NOT NULL,
    Subtotal        DECIMAL(18,2)   NOT NULL,
    Status          NVARCHAR(50)    NOT NULL DEFAULT 'Pendiente',
    Notes           NVARCHAR(MAX)   NULL,
    SentToKitchenAt DATETIME2       NULL,
    ReadyAt         DATETIME2       NULL,
    CreatedAt       DATETIME2       NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt       DATETIME2       NULL,
    IsActive        BIT             NOT NULL DEFAULT 1,
    CONSTRAINT PK_OrderItems PRIMARY KEY (Id),
    CONSTRAINT FK_OrderItems_Orders FOREIGN KEY (OrderId)
        REFERENCES Orders(Id) ON DELETE CASCADE,
    CONSTRAINT FK_OrderItems_Products FOREIGN KEY (ProductId)
        REFERENCES Products(Id) ON DELETE NO ACTION
);

-- ============================================================
-- 11. OrderItemModifiers
-- ============================================================
CREATE TABLE OrderItemModifiers (
    Id                INT             NOT NULL IDENTITY(1,1),
    OrderItemId       INT             NOT NULL,
    ProductModifierId INT             NOT NULL,
    Name              NVARCHAR(100)   NOT NULL,
    PriceAdjustment   DECIMAL(18,2)   NOT NULL DEFAULT 0,
    CreatedAt         DATETIME2       NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT PK_OrderItemModifiers PRIMARY KEY (Id),
    CONSTRAINT FK_OrderItemModifiers_OrderItems FOREIGN KEY (OrderItemId)
        REFERENCES OrderItems(Id) ON DELETE CASCADE,
    CONSTRAINT FK_OrderItemModifiers_ProductModifiers FOREIGN KEY (ProductModifierId)
        REFERENCES ProductModifiers(Id) ON DELETE NO ACTION
);

-- ============================================================
-- 12. PaymentMethods
-- ============================================================
CREATE TABLE PaymentMethods (
    Id          INT             NOT NULL IDENTITY(1,1),
    Name        NVARCHAR(100)   NOT NULL,
    Code        NVARCHAR(50)    NOT NULL,
    CreatedAt   DATETIME2       NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt   DATETIME2       NULL,
    IsActive    BIT             NOT NULL DEFAULT 1,
    CONSTRAINT PK_PaymentMethods PRIMARY KEY (Id)
);

CREATE UNIQUE INDEX IX_PaymentMethods_Code ON PaymentMethods (Code);

-- ============================================================
-- 13. Payments
-- ============================================================
CREATE TABLE Payments (
    Id              INT             NOT NULL IDENTITY(1,1),
    OrderId         INT             NOT NULL,
    PaymentMethodId INT             NOT NULL,
    Amount          DECIMAL(18,2)   NOT NULL,
    ReceivedAmount  DECIMAL(18,2)   NULL,
    ChangeAmount    DECIMAL(18,2)   NULL,
    Reference       NVARCHAR(200)   NULL,
    CashSessionId   INT             NULL,
    ProcessedById   INT             NOT NULL,
    CreatedAt       DATETIME2       NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt       DATETIME2       NULL,
    IsActive        BIT             NOT NULL DEFAULT 1,
    CONSTRAINT PK_Payments PRIMARY KEY (Id),
    CONSTRAINT FK_Payments_Orders FOREIGN KEY (OrderId)
        REFERENCES Orders(Id) ON DELETE NO ACTION,
    CONSTRAINT FK_Payments_PaymentMethods FOREIGN KEY (PaymentMethodId)
        REFERENCES PaymentMethods(Id) ON DELETE NO ACTION,
    CONSTRAINT FK_Payments_Users_ProcessedBy FOREIGN KEY (ProcessedById)
        REFERENCES Users(Id) ON DELETE NO ACTION,
    CONSTRAINT FK_Payments_CashRegisterSessions FOREIGN KEY (CashSessionId)
        REFERENCES CashRegisterSessions(Id) ON DELETE NO ACTION
);

-- ============================================================
-- 14. Invoices
-- ============================================================
CREATE TABLE Invoices (
    Id              INT             NOT NULL IDENTITY(1,1),
    OrderId         INT             NOT NULL,
    InvoiceNumber   NVARCHAR(50)    NOT NULL,
    CAI             NVARCHAR(100)   NULL,
    RangeFrom       NVARCHAR(50)    NULL,
    RangeTo         NVARCHAR(50)    NULL,
    ExpirationDate  DATETIME2       NULL,
    CustomerRTN     NVARCHAR(50)    NULL,
    CustomerName    NVARCHAR(200)   NULL,
    Subtotal        DECIMAL(18,2)   NOT NULL,
    TaxAmount       DECIMAL(18,2)   NOT NULL,
    ExemptAmount    DECIMAL(18,2)   NOT NULL,
    Total           DECIMAL(18,2)   NOT NULL,
    Status          NVARCHAR(50)    NOT NULL DEFAULT 'Emitida',
    CreatedAt       DATETIME2       NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt       DATETIME2       NULL,
    IsActive        BIT             NOT NULL DEFAULT 1,
    CONSTRAINT PK_Invoices PRIMARY KEY (Id),
    CONSTRAINT FK_Invoices_Orders FOREIGN KEY (OrderId)
        REFERENCES Orders(Id) ON DELETE NO ACTION
);

CREATE UNIQUE INDEX IX_Invoices_InvoiceNumber ON Invoices (InvoiceNumber);

-- ============================================================
-- 15. AuditLogs
-- ============================================================
CREATE TABLE AuditLogs (
    Id          INT             NOT NULL IDENTITY(1,1),
    UserId      INT             NULL,
    Action      NVARCHAR(100)   NOT NULL,
    EntityType  NVARCHAR(100)   NOT NULL,
    EntityId    INT             NULL,
    OldValues   NVARCHAR(MAX)   NULL,
    NewValues   NVARCHAR(MAX)   NULL,
    IpAddress   NVARCHAR(50)    NULL,
    CreatedAt   DATETIME2       NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT PK_AuditLogs PRIMARY KEY (Id),
    CONSTRAINT FK_AuditLogs_Users FOREIGN KEY (UserId)
        REFERENCES Users(Id) ON DELETE NO ACTION
);

-- ============================================================
-- Datos iniciales (Seed)
-- ============================================================

-- Métodos de pago
INSERT INTO PaymentMethods (Name, Code, CreatedAt, IsActive)
VALUES
    ('Efectivo',        'CASH',     GETUTCDATE(), 1),
    ('Tarjeta Crédito', 'CREDIT',   GETUTCDATE(), 1),
    ('Tarjeta Débito',  'DEBIT',    GETUTCDATE(), 1),
    ('Transferencia',   'TRANSFER', GETUTCDATE(), 1);

-- Usuario administrador por defecto
-- IMPORTANTE: reemplazar PasswordHash con un hash BCrypt real antes de usar en producción
INSERT INTO Users (Email, PasswordHash, FirstName, LastName, Role, CreatedAt, IsActive)
VALUES ('admin@restaurant.com', '$2a$12$REEMPLAZAR_CON_HASH_REAL', 'Admin', 'Sistema', 'Admin', GETUTCDATE(), 1);
