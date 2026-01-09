CREATE DATABASE IF NOT EXISTS borrowsystem;
USE borrowsystem;

CREATE TABLE Employees (
    EmployeeID INT AUTO_INCREMENT PRIMARY KEY,
    Name VARCHAR(150) NOT NULL,
    Department VARCHAR(100) NOT NULL
);

CREATE TABLE Equipment (
    EquipmentID INT AUTO_INCREMENT PRIMARY KEY,
    EquipmentName VARCHAR(150) NOT NULL,
    Quantity INT NOT NULL
);

CREATE TABLE BorrowLogs (
    BorrowID INT AUTO_INCREMENT PRIMARY KEY,
    EmployeeID INT NOT NULL,
    EquipmentID INT NOT NULL,
    BorrowDate DATE NOT NULL,
    ReturnDate DATE,
    Status VARCHAR(20) DEFAULT 'Borrowed',
    FOREIGN KEY (EmployeeID) REFERENCES Employees(EmployeeID) ON DELETE CASCADE,
    FOREIGN KEY (EquipmentID) REFERENCES Equipment(EquipmentID) ON DELETE CASCADE
);

INSERT INTO Equipment (EquipmentName, Quantity) VALUES 
('Laptop', 2), 
('Projector', 3),
('Camera', 2);

INSERT INTO Employees (Name, Department) VALUES
('Rona', 'Computer Science'),
('Jemar','Computer Science'),
('Samson','ACT');


DROP TABLE IF EXISTS Employees;
DROP TABLE IF EXISTS Equipment;
DROP TABLE IF EXISTS BorrowLogs;