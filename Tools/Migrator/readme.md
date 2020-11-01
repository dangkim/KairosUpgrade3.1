- all SQL files should put under Scripts folder
- naming convention: 
  - yyyyMMdd.{revision:00} - {summary}.sql
  - maximum length of revision is 2
    - example: 
        1. 20170610.01 - Initial tables.sql
        1. 20170610.02 - Alter User table.sql
- usages:
  - `dotnet run` execute migration
  - `dotnet run --drop` drop database
  - `dotnet run --new` drop database and execute migrate from the beginning version
  







