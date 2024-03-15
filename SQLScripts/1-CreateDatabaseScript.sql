IF NOT EXISTS(SELECT * FROM sys.databases WHERE name = 'ExchangeRates')
BEGIN
    CREATE DATABASE [ExchangeRates]
END
GO
       USE [ExchangeRates]
GO

IF NOT EXISTS (SELECT * FROM sys.objects
               WHERE object_id = OBJECT_ID(N'[dbo].[ForeignExchangeRates]') AND type in (N'U'))
BEGIN
CREATE TABLE ForeignExchangeRates (
    Id UNIQUEIDENTIFIER PRIMARY KEY NOT NULL ,
    FromCurrencyCode VARCHAR(3),
    ToCurrencyCode VARCHAR(3),
    FromCurrencyName NVARCHAR(100),
    ToCurrencyName NVARCHAR(100),
    Bid MONEY,
    Ask MONEY,
    Created DateTime,
    Updated DateTime
)

PRINT ('Inserting Data')

INSERT INTO [ForeignExchangeRates](Id, FromCurrencyCode, ToCurrencyCode, FromCurrencyName, ToCurrencyName, Bid, Ask)
VALUES ('a693e148-1071-4b7f-b9ef-0f1595f0a8b0', 'USD', 'GBP', 'United States Dollar', 'British Pound Sterling', 0.7854, 0.7854)

INSERT INTO [ForeignExchangeRates](Id, FromCurrencyCode, ToCurrencyCode, FromCurrencyName, ToCurrencyName, Bid, Ask)
VALUES ('7538f361-fc06-4d09-bbdf-75ff5e14a2e2', 'USD', 'EUR', 'United States Dollar', 'Euro', 0.9183, 0.9183)

PRINT ('Finished database setup')

END