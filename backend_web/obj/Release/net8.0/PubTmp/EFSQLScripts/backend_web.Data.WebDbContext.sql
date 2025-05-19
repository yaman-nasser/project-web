IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250104202953_user'
)
BEGIN
    CREATE TABLE [users] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(100) NULL,
        [Email] nvarchar(max) NULL,
        [Password] nvarchar(15) NULL,
        [Phone] nvarchar(max) NULL,
        CONSTRAINT [PK_users] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250104202953_user'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20250104202953_user', N'8.0.10');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250104204349_product'
)
BEGIN
    DECLARE @var0 sysname;
    SELECT @var0 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[users]') AND [c].[name] = N'Password');
    IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [users] DROP CONSTRAINT [' + @var0 + '];');
    ALTER TABLE [users] ALTER COLUMN [Password] nvarchar(max) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250104204349_product'
)
BEGIN
    CREATE TABLE [LikedProduct] (
        [Id] int NOT NULL IDENTITY,
        [UserId] int NOT NULL,
        [ProductId] int NOT NULL,
        CONSTRAINT [PK_LikedProduct] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_LikedProduct_users_UserId] FOREIGN KEY ([UserId]) REFERENCES [users] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250104204349_product'
)
BEGIN
    CREATE TABLE [products] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(max) NULL,
        [caption] nvarchar(max) NULL,
        [price] decimal(18,2) NOT NULL,
        [type] nvarchar(max) NULL,
        [PHnum] nvarchar(max) NULL,
        [UserId] int NOT NULL,
        CONSTRAINT [PK_products] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_products_users_UserId] FOREIGN KEY ([UserId]) REFERENCES [users] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250104204349_product'
)
BEGIN
    CREATE INDEX [IX_LikedProduct_UserId] ON [LikedProduct] ([UserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250104204349_product'
)
BEGIN
    CREATE INDEX [IX_products_UserId] ON [products] ([UserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250104204349_product'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20250104204349_product', N'8.0.10');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250104204633_img'
)
BEGIN
    CREATE TABLE [Images] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(max) NOT NULL,
        [image] varbinary(max) NULL,
        [ContentType] nvarchar(max) NOT NULL,
        [ProductId] int NOT NULL,
        CONSTRAINT [PK_Images] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Images_products_ProductId] FOREIGN KEY ([ProductId]) REFERENCES [products] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250104204633_img'
)
BEGIN
    CREATE INDEX [IX_Images_ProductId] ON [Images] ([ProductId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250104204633_img'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20250104204633_img', N'8.0.10');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250104205821_like'
)
BEGIN
    ALTER TABLE [LikedProduct] DROP CONSTRAINT [FK_LikedProduct_users_UserId];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250104205821_like'
)
BEGIN
    ALTER TABLE [LikedProduct] DROP CONSTRAINT [PK_LikedProduct];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250104205821_like'
)
BEGIN
    EXEC sp_rename N'[LikedProduct]', N'LikedProducts';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250104205821_like'
)
BEGIN
    EXEC sp_rename N'[LikedProducts].[IX_LikedProduct_UserId]', N'IX_LikedProducts_UserId', N'INDEX';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250104205821_like'
)
BEGIN
    DECLARE @var1 sysname;
    SELECT @var1 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[users]') AND [c].[name] = N'Email');
    IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [users] DROP CONSTRAINT [' + @var1 + '];');
    ALTER TABLE [users] ALTER COLUMN [Email] nvarchar(450) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250104205821_like'
)
BEGIN
    ALTER TABLE [LikedProducts] ADD CONSTRAINT [PK_LikedProducts] PRIMARY KEY ([Id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250104205821_like'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_users_Email] ON [users] ([Email]) WHERE [Email] IS NOT NULL');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250104205821_like'
)
BEGIN
    CREATE INDEX [IX_LikedProducts_ProductId] ON [LikedProducts] ([ProductId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250104205821_like'
)
BEGIN
    ALTER TABLE [LikedProducts] ADD CONSTRAINT [FK_LikedProducts_products_ProductId] FOREIGN KEY ([ProductId]) REFERENCES [products] ([Id]) ON DELETE NO ACTION;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250104205821_like'
)
BEGIN
    ALTER TABLE [LikedProducts] ADD CONSTRAINT [FK_LikedProducts_users_UserId] FOREIGN KEY ([UserId]) REFERENCES [users] ([Id]) ON DELETE CASCADE;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250104205821_like'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20250104205821_like', N'8.0.10');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250328202247_azerefir'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20250328202247_azerefir', N'8.0.10');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250328205626_azeretow'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20250328205626_azeretow', N'8.0.10');
END;
GO

COMMIT;
GO

