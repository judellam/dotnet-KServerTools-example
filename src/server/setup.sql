-- Note: to be fancy, you could track the version of the database schema in a table and run the appropriate 
-- scripts to update the schema to the latest version. But for this simple example, we'll just create the 
-- database and table if they don't already exist.
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'UserDb')
BEGIN
    CREATE DATABASE UserDb;
END
GO

USE UserDb;

-- DROP TABLE IF EXISTS Users;

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Users')
BEGIN
    CREATE TABLE Users (
        Id INT IDENTITY(1,1) NOT NULL,
        Username VARCHAR(255) NOT NULL,
        Email VARCHAR(255) NOT NULL,
        Hash VARBINARY(256) NOT NULL,
        Salt VARBINARY(256) NOT NULL,
        IsActive BIT NOT NULL,
        PRIMARY KEY (Username),
        UNIQUE (Email)
    );
END
GO