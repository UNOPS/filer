# filer

Filer is a very simple .NET Standard library to help you manage documents inside your app.

# How to use?

Install the nuget package:

```
Install-Package Filer.EntityFrameworkCore -Pre
```

Initialize database with this script:

```
CREATE TABLE [File] (
    [Id] int NOT NULL IDENTITY,
    [CompressionFormat] tinyint NOT NULL,
    [CreatedByUserId] int,
    [CreatedOn] datetime2 NOT NULL,
    [Extension] nvarchar(20),
    [MimeType] varchar(100),
    [Name] nvarchar(255),
    [Size] bigint NOT NULL,
    CONSTRAINT [PK_File] PRIMARY KEY ([Id])
);

GO

CREATE TABLE [FileContext] (
    [FileId] int NOT NULL,
    [Value] varchar(50) NOT NULL,
    CONSTRAINT [PK_FileContext] PRIMARY KEY ([FileId], [Value]),
    CONSTRAINT [FK_FileContext_File_FileId] FOREIGN KEY ([FileId]) REFERENCES [File] ([Id]) ON DELETE NO ACTION
);

GO

CREATE TABLE [FileData] (
    [Id] int NOT NULL,
    [Data] varbinary(max),
    CONSTRAINT [PK_FileData] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_FileData_File_Id] FOREIGN KEY ([Id]) REFERENCES [File] ([Id]) ON DELETE CASCADE
);

GO

CREATE INDEX [IX_File_CreatedByUserId] ON [File] ([CreatedByUserId]);

GO
```