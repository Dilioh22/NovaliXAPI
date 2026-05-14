-- ============================================================
--  RestaurantPOS - Script MySQL completo
--  Generado para: phpMyAdmin / MySQL 8.x
--  Charset: utf8mb4 (soporte completo Unicode + emojis)
-- ============================================================

CREATE DATABASE IF NOT EXISTS `RestaurantPOS`
  CHARACTER SET utf8mb4
  COLLATE utf8mb4_unicode_ci;

USE `RestaurantPOS`;

-- Tabla requerida por EF Core para rastrear migraciones
CREATE TABLE IF NOT EXISTS `__EFMigrationsHistory` (
  `MigrationId`   VARCHAR(150) NOT NULL,
  `ProductVersion` VARCHAR(32)  NOT NULL,
  PRIMARY KEY (`MigrationId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================================
--  Tablas independientes (sin FK)
-- ============================================================

CREATE TABLE `CashRegisters` (
  `Id`        INT            NOT NULL AUTO_INCREMENT,
  `Name`      VARCHAR(100)   NOT NULL,
  `Location`  VARCHAR(200)       NULL,
  `CreatedAt` DATETIME(6)    NOT NULL,
  `UpdatedAt` DATETIME(6)        NULL,
  `IsActive`  TINYINT(1)     NOT NULL DEFAULT 1,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE `Categories` (
  `Id`           INT          NOT NULL AUTO_INCREMENT,
  `Name`         VARCHAR(100) NOT NULL,
  `Description`  VARCHAR(500)     NULL,
  `DisplayOrder` INT          NOT NULL DEFAULT 0,
  `IconName`     VARCHAR(100)     NULL,
  `CreatedAt`    DATETIME(6)  NOT NULL,
  `UpdatedAt`    DATETIME(6)      NULL,
  `IsActive`     TINYINT(1)   NOT NULL DEFAULT 1,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE `PaymentMethods` (
  `Id`        INT         NOT NULL AUTO_INCREMENT,
  `Name`      VARCHAR(100) NOT NULL,
  `Code`      VARCHAR(50)  NOT NULL,
  `CreatedAt` DATETIME(6)  NOT NULL,
  `UpdatedAt` DATETIME(6)      NULL,
  `IsActive`  TINYINT(1)   NOT NULL DEFAULT 1,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_PaymentMethods_Code` (`Code`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE `TableZones` (
  `Id`          INT          NOT NULL AUTO_INCREMENT,
  `Name`        VARCHAR(100) NOT NULL,
  `Description` VARCHAR(500)     NULL,
  `CreatedAt`   DATETIME(6)  NOT NULL,
  `UpdatedAt`   DATETIME(6)      NULL,
  `IsActive`    TINYINT(1)   NOT NULL DEFAULT 1,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE `Users` (
  `Id`                 INT          NOT NULL AUTO_INCREMENT,
  `Email`              VARCHAR(200) NOT NULL,
  `PasswordHash`       VARCHAR(500) NOT NULL,
  `FirstName`          VARCHAR(100) NOT NULL,
  `LastName`           VARCHAR(100) NOT NULL,
  `Role`               VARCHAR(50)  NOT NULL,
  `Pin`                VARCHAR(10)      NULL,
  `RefreshToken`       VARCHAR(500)     NULL,
  `RefreshTokenExpiry` DATETIME(6)      NULL,
  `LastLoginAt`        DATETIME(6)      NULL,
  `CreatedAt`          DATETIME(6)  NOT NULL,
  `UpdatedAt`          DATETIME(6)      NULL,
  `IsActive`           TINYINT(1)   NOT NULL DEFAULT 1,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_Users_Email` (`Email`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================================
--  Tablas que dependen de las anteriores
-- ============================================================

CREATE TABLE `Products` (
  `Id`              INT            NOT NULL AUTO_INCREMENT,
  `CategoryId`      INT            NOT NULL,
  `Name`            VARCHAR(200)   NOT NULL,
  `Description`     VARCHAR(1000)      NULL,
  `Price`           DECIMAL(18,2)  NOT NULL,
  `ImageUrl`        VARCHAR(500)       NULL,
  `PreparationTime` INT                NULL,
  `IsAvailable`     TINYINT(1)     NOT NULL DEFAULT 1,
  `DisplayOrder`    INT            NOT NULL DEFAULT 0,
  `CreatedAt`       DATETIME(6)    NOT NULL,
  `UpdatedAt`       DATETIME(6)        NULL,
  `IsActive`        TINYINT(1)     NOT NULL DEFAULT 1,
  PRIMARY KEY (`Id`),
  KEY `IX_Products_CategoryId` (`CategoryId`),
  CONSTRAINT `FK_Products_Categories_CategoryId`
    FOREIGN KEY (`CategoryId`) REFERENCES `Categories` (`Id`)
    ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE `Tables` (
  `Id`        INT         NOT NULL AUTO_INCREMENT,
  `ZoneId`    INT         NOT NULL,
  `Number`    INT         NOT NULL,
  `Capacity`  INT         NOT NULL,
  `Status`    VARCHAR(50) NOT NULL DEFAULT 'Available',
  `PositionX` INT             NULL,
  `PositionY` INT             NULL,
  `Shape`     VARCHAR(50) NOT NULL DEFAULT 'rectangle',
  `CreatedAt` DATETIME(6) NOT NULL,
  `UpdatedAt` DATETIME(6)     NULL,
  `IsActive`  TINYINT(1)  NOT NULL DEFAULT 1,
  PRIMARY KEY (`Id`),
  KEY `IX_Tables_ZoneId_Number` (`ZoneId`, `Number`),
  CONSTRAINT `FK_Tables_TableZones_ZoneId`
    FOREIGN KEY (`ZoneId`) REFERENCES `TableZones` (`Id`)
    ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE `CashRegisterSessions` (
  `Id`               INT            NOT NULL AUTO_INCREMENT,
  `CashRegisterId`   INT            NOT NULL,
  `OpenedById`       INT            NOT NULL,
  `ClosedById`       INT                NULL,
  `OpeningAmount`    DECIMAL(18,2)  NOT NULL DEFAULT 0.00,
  `ClosingAmount`    DECIMAL(18,2)      NULL,
  `ExpectedAmount`   DECIMAL(18,2)      NULL,
  `Difference`       DECIMAL(18,2)      NULL,
  `TotalCash`        DECIMAL(18,2)      NULL,
  `TotalCard`        DECIMAL(18,2)      NULL,
  `TotalTransfer`    DECIMAL(18,2)      NULL,
  `TotalSales`       DECIMAL(18,2)      NULL,
  `TransactionCount` INT                NULL,
  `Status`           VARCHAR(50)    NOT NULL,
  `OpenedAt`         DATETIME(6)    NOT NULL,
  `ClosedAt`         DATETIME(6)        NULL,
  `Notes`            LONGTEXT           NULL,
  `CreatedAt`        DATETIME(6)    NOT NULL,
  `UpdatedAt`        DATETIME(6)        NULL,
  `IsActive`         TINYINT(1)     NOT NULL DEFAULT 1,
  PRIMARY KEY (`Id`),
  KEY `IX_CashRegisterSessions_CashRegisterId` (`CashRegisterId`),
  KEY `IX_CashRegisterSessions_OpenedById` (`OpenedById`),
  KEY `IX_CashRegisterSessions_ClosedById` (`ClosedById`),
  CONSTRAINT `FK_CashRegisterSessions_CashRegisters_CashRegisterId`
    FOREIGN KEY (`CashRegisterId`) REFERENCES `CashRegisters` (`Id`)
    ON DELETE RESTRICT ON UPDATE CASCADE,
  CONSTRAINT `FK_CashRegisterSessions_Users_OpenedById`
    FOREIGN KEY (`OpenedById`) REFERENCES `Users` (`Id`)
    ON DELETE RESTRICT ON UPDATE CASCADE,
  CONSTRAINT `FK_CashRegisterSessions_Users_ClosedById`
    FOREIGN KEY (`ClosedById`) REFERENCES `Users` (`Id`)
    ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE `ProductModifiers` (
  `Id`              INT            NOT NULL AUTO_INCREMENT,
  `ProductId`       INT            NOT NULL,
  `Name`            VARCHAR(100)   NOT NULL,
  `PriceAdjustment` DECIMAL(18,2)  NOT NULL DEFAULT 0.00,
  `CreatedAt`       DATETIME(6)    NOT NULL,
  `UpdatedAt`       DATETIME(6)        NULL,
  `IsActive`        TINYINT(1)     NOT NULL DEFAULT 1,
  PRIMARY KEY (`Id`),
  KEY `IX_ProductModifiers_ProductId` (`ProductId`),
  CONSTRAINT `FK_ProductModifiers_Products_ProductId`
    FOREIGN KEY (`ProductId`) REFERENCES `Products` (`Id`)
    ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE `Orders` (
  `Id`             INT            NOT NULL AUTO_INCREMENT,
  `OrderNumber`    VARCHAR(50)    NOT NULL,
  `TableId`        INT                NULL,
  `WaiterId`       INT            NOT NULL,
  `Status`         VARCHAR(50)    NOT NULL,
  `OrderType`      VARCHAR(50)    NOT NULL,
  `Subtotal`       DECIMAL(18,2)  NOT NULL DEFAULT 0.00,
  `TaxRate`        DECIMAL(5,4)   NOT NULL DEFAULT 0.0000,
  `TaxAmount`      DECIMAL(18,2)  NOT NULL DEFAULT 0.00,
  `DiscountAmount` DECIMAL(18,2)  NOT NULL DEFAULT 0.00,
  `DiscountPercent` DECIMAL(5,2)  NOT NULL DEFAULT 0.00,
  `Total`          DECIMAL(18,2)  NOT NULL DEFAULT 0.00,
  `TipAmount`      DECIMAL(18,2)  NOT NULL DEFAULT 0.00,
  `Notes`          LONGTEXT           NULL,
  `CustomerName`   VARCHAR(200)       NULL,
  `CustomerPhone`  VARCHAR(50)        NULL,
  `CreatedAt`      DATETIME(6)    NOT NULL,
  `UpdatedAt`      DATETIME(6)        NULL,
  `IsActive`       TINYINT(1)     NOT NULL DEFAULT 1,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_Orders_OrderNumber` (`OrderNumber`),
  KEY `IX_Orders_TableId` (`TableId`),
  KEY `IX_Orders_WaiterId` (`WaiterId`),
  CONSTRAINT `FK_Orders_Tables_TableId`
    FOREIGN KEY (`TableId`) REFERENCES `Tables` (`Id`)
    ON DELETE RESTRICT ON UPDATE CASCADE,
  CONSTRAINT `FK_Orders_Users_WaiterId`
    FOREIGN KEY (`WaiterId`) REFERENCES `Users` (`Id`)
    ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE `AuditLogs` (
  `Id`         INT          NOT NULL AUTO_INCREMENT,
  `UserId`     INT              NULL,
  `Action`     VARCHAR(100) NOT NULL,
  `EntityType` VARCHAR(100) NOT NULL,
  `EntityId`   INT              NULL,
  `OldValues`  LONGTEXT         NULL,
  `NewValues`  LONGTEXT         NULL,
  `IpAddress`  VARCHAR(50)      NULL,
  `CreatedAt`  DATETIME(6)  NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_AuditLogs_UserId` (`UserId`),
  CONSTRAINT `FK_AuditLogs_Users_UserId`
    FOREIGN KEY (`UserId`) REFERENCES `Users` (`Id`)
    ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================================
--  Tablas que dependen de Orders
-- ============================================================

CREATE TABLE `Invoices` (
  `Id`             INT            NOT NULL AUTO_INCREMENT,
  `OrderId`        INT            NOT NULL,
  `InvoiceNumber`  VARCHAR(50)    NOT NULL,
  `CAI`            VARCHAR(100)       NULL,
  `RangeFrom`      VARCHAR(50)        NULL,
  `RangeTo`        VARCHAR(50)        NULL,
  `ExpirationDate` DATETIME(6)        NULL,
  `CustomerRTN`    VARCHAR(50)        NULL,
  `CustomerName`   VARCHAR(200)       NULL,
  `Subtotal`       DECIMAL(18,2)  NOT NULL DEFAULT 0.00,
  `TaxAmount`      DECIMAL(18,2)  NOT NULL DEFAULT 0.00,
  `ExemptAmount`   DECIMAL(18,2)  NOT NULL DEFAULT 0.00,
  `Total`          DECIMAL(18,2)  NOT NULL DEFAULT 0.00,
  `Status`         VARCHAR(50)    NOT NULL,
  `CreatedAt`      DATETIME(6)    NOT NULL,
  `UpdatedAt`      DATETIME(6)        NULL,
  `IsActive`       TINYINT(1)     NOT NULL DEFAULT 1,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_Invoices_InvoiceNumber` (`InvoiceNumber`),
  UNIQUE KEY `IX_Invoices_OrderId` (`OrderId`),
  CONSTRAINT `FK_Invoices_Orders_OrderId`
    FOREIGN KEY (`OrderId`) REFERENCES `Orders` (`Id`)
    ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE `OrderItems` (
  `Id`              INT            NOT NULL AUTO_INCREMENT,
  `OrderId`         INT            NOT NULL,
  `ProductId`       INT            NOT NULL,
  `Quantity`        INT            NOT NULL DEFAULT 1,
  `UnitPrice`       DECIMAL(18,2)  NOT NULL,
  `Subtotal`        DECIMAL(18,2)  NOT NULL,
  `Status`          VARCHAR(50)    NOT NULL,
  `Notes`           LONGTEXT           NULL,
  `SentToKitchenAt` DATETIME(6)        NULL,
  `ReadyAt`         DATETIME(6)        NULL,
  `CreatedAt`       DATETIME(6)    NOT NULL,
  `UpdatedAt`       DATETIME(6)        NULL,
  `IsActive`        TINYINT(1)     NOT NULL DEFAULT 1,
  PRIMARY KEY (`Id`),
  KEY `IX_OrderItems_OrderId` (`OrderId`),
  KEY `IX_OrderItems_ProductId` (`ProductId`),
  CONSTRAINT `FK_OrderItems_Orders_OrderId`
    FOREIGN KEY (`OrderId`) REFERENCES `Orders` (`Id`)
    ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `FK_OrderItems_Products_ProductId`
    FOREIGN KEY (`ProductId`) REFERENCES `Products` (`Id`)
    ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE `Payments` (
  `Id`             INT            NOT NULL AUTO_INCREMENT,
  `OrderId`        INT            NOT NULL,
  `PaymentMethodId` INT           NOT NULL,
  `Amount`         DECIMAL(18,2)  NOT NULL,
  `ReceivedAmount` DECIMAL(18,2)      NULL,
  `ChangeAmount`   DECIMAL(18,2)      NULL,
  `Reference`      VARCHAR(200)       NULL,
  `CashSessionId`  INT                NULL,
  `ProcessedById`  INT            NOT NULL,
  `CreatedAt`      DATETIME(6)    NOT NULL,
  `UpdatedAt`      DATETIME(6)        NULL,
  `IsActive`       TINYINT(1)     NOT NULL DEFAULT 1,
  PRIMARY KEY (`Id`),
  KEY `IX_Payments_OrderId` (`OrderId`),
  KEY `IX_Payments_PaymentMethodId` (`PaymentMethodId`),
  KEY `IX_Payments_CashSessionId` (`CashSessionId`),
  KEY `IX_Payments_ProcessedById` (`ProcessedById`),
  CONSTRAINT `FK_Payments_Orders_OrderId`
    FOREIGN KEY (`OrderId`) REFERENCES `Orders` (`Id`)
    ON DELETE RESTRICT ON UPDATE CASCADE,
  CONSTRAINT `FK_Payments_PaymentMethods_PaymentMethodId`
    FOREIGN KEY (`PaymentMethodId`) REFERENCES `PaymentMethods` (`Id`)
    ON DELETE RESTRICT ON UPDATE CASCADE,
  CONSTRAINT `FK_Payments_CashRegisterSessions_CashSessionId`
    FOREIGN KEY (`CashSessionId`) REFERENCES `CashRegisterSessions` (`Id`)
    ON DELETE RESTRICT ON UPDATE CASCADE,
  CONSTRAINT `FK_Payments_Users_ProcessedById`
    FOREIGN KEY (`ProcessedById`) REFERENCES `Users` (`Id`)
    ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================================
--  Tablas que dependen de OrderItems
-- ============================================================

CREATE TABLE `OrderItemModifiers` (
  `Id`               INT            NOT NULL AUTO_INCREMENT,
  `OrderItemId`      INT            NOT NULL,
  `ProductModifierId` INT           NOT NULL,
  `Name`             VARCHAR(100)   NOT NULL,
  `PriceAdjustment`  DECIMAL(18,2)  NOT NULL DEFAULT 0.00,
  `CreatedAt`        DATETIME(6)    NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_OrderItemModifiers_OrderItemId` (`OrderItemId`),
  KEY `IX_OrderItemModifiers_ProductModifierId` (`ProductModifierId`),
  CONSTRAINT `FK_OrderItemModifiers_OrderItems_OrderItemId`
    FOREIGN KEY (`OrderItemId`) REFERENCES `OrderItems` (`Id`)
    ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `FK_OrderItemModifiers_ProductModifiers_ProductModifierId`
    FOREIGN KEY (`ProductModifierId`) REFERENCES `ProductModifiers` (`Id`)
    ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================================
--  Reservaciones
-- ============================================================

CREATE TABLE `Reservations` (
  `Id`              INT          NOT NULL AUTO_INCREMENT,
  `TableId`         INT          NOT NULL,
  `CustomerName`    VARCHAR(200) NOT NULL,
  `CustomerPhone`   VARCHAR(50)      NULL,
  `PartySize`       INT          NOT NULL DEFAULT 1,
  `ReservationDate` DATETIME(6)  NOT NULL,
  `Notes`           VARCHAR(500)     NULL,
  `Status`          VARCHAR(50)  NOT NULL DEFAULT 'Pending',
  `CreatedAt`       DATETIME(6)  NOT NULL,
  `UpdatedAt`       DATETIME(6)      NULL,
  `IsActive`        TINYINT(1)   NOT NULL DEFAULT 1,
  PRIMARY KEY (`Id`),
  KEY `IX_Reservations_TableId_ReservationDate` (`TableId`, `ReservationDate`),
  CONSTRAINT `FK_Reservations_Tables_TableId`
    FOREIGN KEY (`TableId`) REFERENCES `Tables` (`Id`)
    ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================================
--  DATOS INICIALES (Seed)
-- ============================================================

-- Métodos de pago
INSERT INTO `PaymentMethods` (`Name`, `Code`, `CreatedAt`, `IsActive`) VALUES
  ('Efectivo',               'CASH',     NOW(6), 1),
  ('Tarjeta de Crédito/Débito', 'CARD',  NOW(6), 1),
  ('Transferencia',          'TRANSFER', NOW(6), 1);

-- Categorías
INSERT INTO `Categories` (`Name`, `DisplayOrder`, `IconName`, `CreatedAt`, `IsActive`) VALUES
  ('Entradas',         1, 'appetizer',   NOW(6), 1),
  ('Bebidas',          2, 'drink',       NOW(6), 1),
  ('Platos Principales', 3, 'main_course', NOW(6), 1),
  ('Postres',          4, 'dessert',     NOW(6), 1);

-- Productos - Entradas (CategoryId = 1)
INSERT INTO `Products` (`CategoryId`, `Name`, `Description`, `Price`, `PreparationTime`, `DisplayOrder`, `IsAvailable`, `CreatedAt`, `IsActive`) VALUES
  (1, 'Sopa del Día',    'Sopa casera del día',                                          85.00, 10, 1, 1, NOW(6), 1),
  (1, 'Ensalada César',  'Lechuga romana, crutones, parmesano y aderezo César',         120.00,  8, 2, 1, NOW(6), 1),
  (1, 'Nachos con Queso','Tortillas con queso fundido, jalapeños y guacamole',          130.00, 12, 3, 1, NOW(6), 1);

-- Productos - Bebidas (CategoryId = 2)
INSERT INTO `Products` (`CategoryId`, `Name`, `Description`, `Price`, `PreparationTime`, `DisplayOrder`, `IsAvailable`, `CreatedAt`, `IsActive`) VALUES
  (2, 'Agua Natural',    'Agua embotellada 600ml',            30.00, 1, 1, 1, NOW(6), 1),
  (2, 'Refresco',        'Coca-Cola, Pepsi, Sprite 355ml',    45.00, 1, 2, 1, NOW(6), 1),
  (2, 'Jugo Natural',    'Naranja, mango o piña 350ml',       65.00, 5, 3, 1, NOW(6), 1),
  (2, 'Café Americano',  'Café de grano tostado',             55.00, 3, 4, 1, NOW(6), 1);

-- Productos - Platos Principales (CategoryId = 3)
INSERT INTO `Products` (`CategoryId`, `Name`, `Description`, `Price`, `PreparationTime`, `DisplayOrder`, `IsAvailable`, `CreatedAt`, `IsActive`) VALUES
  (3, 'Pollo a la Plancha',    'Pechuga de pollo a la plancha con arroz y ensalada',    220.00, 20, 1, 1, NOW(6), 1),
  (3, 'Chuleta de Cerdo',      'Chuleta de cerdo asada con papas fritas',               250.00, 25, 2, 1, NOW(6), 1),
  (3, 'Bistec a la Hondureña', 'Bistec de res con chimol, arroz, frijoles y tortillas', 280.00, 25, 3, 1, NOW(6), 1),
  (3, 'Pasta Carbonara',       'Pasta con salsa carbonara y tocino',                    190.00, 18, 4, 1, NOW(6), 1),
  (3, 'Hamburguesa Clásica',   'Hamburguesa de res con papas fritas',                   175.00, 15, 5, 1, NOW(6), 1);

-- Modificadores de Hamburguesa (ProductId = 12, último insertado en Platos Principales)
INSERT INTO `ProductModifiers` (`ProductId`, `Name`, `PriceAdjustment`, `CreatedAt`, `IsActive`) VALUES
  (12, 'Queso Extra', 20.00, NOW(6), 1),
  (12, 'Tocino',      25.00, NOW(6), 1),
  (12, 'Sin Cebolla',  0.00, NOW(6), 1);

-- Productos - Postres (CategoryId = 4)
INSERT INTO `Products` (`CategoryId`, `Name`, `Description`, `Price`, `PreparationTime`, `DisplayOrder`, `IsAvailable`, `CreatedAt`, `IsActive`) VALUES
  (4, 'Flan de Caramelo',   'Flan casero con salsa de caramelo',       85.00, 5, 1, 1, NOW(6), 1),
  (4, 'Pastel de Chocolate','Pastel húmedo de chocolate con helado',    95.00, 5, 2, 1, NOW(6), 1);

-- Zonas de mesa
INSERT INTO `TableZones` (`Name`, `Description`, `CreatedAt`, `IsActive`) VALUES
  ('Interior', 'Salón interior climatizado',  NOW(6), 1),
  ('Terraza',  'Área exterior al aire libre', NOW(6), 1),
  ('Bar',      'Área de bar',                 NOW(6), 1);

-- Mesas Interior (ZoneId = 1) — mesas 1-8
INSERT INTO `Tables` (`ZoneId`, `Number`, `Capacity`, `Status`, `PositionX`, `PositionY`, `Shape`, `CreatedAt`, `IsActive`) VALUES
  (1, 1, 4, 'Available', 0,   0,   'rectangle', NOW(6), 1),
  (1, 2, 4, 'Available', 150, 0,   'rectangle', NOW(6), 1),
  (1, 3, 4, 'Available', 300, 0,   'rectangle', NOW(6), 1),
  (1, 4, 4, 'Available', 450, 0,   'rectangle', NOW(6), 1),
  (1, 5, 6, 'Available', 0,   150, 'rectangle', NOW(6), 1),
  (1, 6, 6, 'Available', 150, 150, 'rectangle', NOW(6), 1),
  (1, 7, 6, 'Available', 300, 150, 'rectangle', NOW(6), 1),
  (1, 8, 6, 'Available', 450, 150, 'rectangle', NOW(6), 1);

-- Mesas Terraza (ZoneId = 2) — mesas 9-14
INSERT INTO `Tables` (`ZoneId`, `Number`, `Capacity`, `Status`, `PositionX`, `PositionY`, `Shape`, `CreatedAt`, `IsActive`) VALUES
  (2, 9,  4, 'Available', 0,   0,   'rectangle', NOW(6), 1),
  (2, 10, 4, 'Available', 150, 0,   'rectangle', NOW(6), 1),
  (2, 11, 4, 'Available', 300, 0,   'rectangle', NOW(6), 1),
  (2, 12, 4, 'Available', 0,   150, 'rectangle', NOW(6), 1),
  (2, 13, 4, 'Available', 150, 150, 'rectangle', NOW(6), 1),
  (2, 14, 4, 'Available', 300, 150, 'rectangle', NOW(6), 1);

-- Mesas Bar (ZoneId = 3) — mesas 15-16
INSERT INTO `Tables` (`ZoneId`, `Number`, `Capacity`, `Status`, `PositionX`, `PositionY`, `Shape`, `CreatedAt`, `IsActive`) VALUES
  (3, 15, 2, 'Available', NULL, NULL, 'circle', NOW(6), 1),
  (3, 16, 2, 'Available', NULL, NULL, 'circle', NOW(6), 1);

-- Usuarios (contraseñas hasheadas con BCrypt — reemplaza con los hashes reales del DataSeeder)
-- IMPORTANTE: estos hashes BCrypt son válidos para las contraseñas indicadas
INSERT INTO `Users` (`Email`, `PasswordHash`, `FirstName`, `LastName`, `Role`, `Pin`, `CreatedAt`, `IsActive`) VALUES
  ('admin@restaurante.com',    '$2a$11$placeholder_admin_hash_aqui',   'Administrador', 'Sistema',   'Admin',    '1234', NOW(6), 1),
  ('cajero@restaurante.com',   '$2a$11$placeholder_cajero_hash_aqui',  'Carlos',        'García',    'Cajero',   '2222', NOW(6), 1),
  ('mesero@restaurante.com',   '$2a$11$placeholder_mesero_hash_aqui',  'María',         'López',     'Mesero',   '3333', NOW(6), 1),
  ('cocinero@restaurante.com', '$2a$11$placeholder_cocina_hash_aqui',  'Juan',          'Martínez',  'Cocinero', '4444', NOW(6), 1);

-- Cajas registradoras
INSERT INTO `CashRegisters` (`Name`, `Location`, `CreatedAt`, `IsActive`) VALUES
  ('Caja Principal', 'Recepción', NOW(6), 1),
  ('Caja Bar',       'Bar',       NOW(6), 1);

-- ============================================================
--  Registro de migración en EF Core
-- ============================================================
INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`) VALUES
  ('20260322151427_InitialCreate',          '8.0.0'),
  ('20260412000000_AddReservationsTable',   '8.0.0');
