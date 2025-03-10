CREATE DATABASE TestDatabase;
go

USE TestDatabase;

CREATE TABLE TestTable (
    ID INT PRIMARY KEY,
    Name NVARCHAR(50)
);

INSERT INTO TestTable (ID, Name) VALUES (1, 'data 1');
INSERT INTO TestTable (ID, Name) VALUES (2, 'data 2');
INSERT INTO TestTable (ID, Name) VALUES (3, 'data 3');
go