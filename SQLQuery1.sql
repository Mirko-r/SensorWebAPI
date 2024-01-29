-- Creating the Machines table
CREATE TABLE Machines (
    machineid INT PRIMARY KEY,
    location VARCHAR(255) NOT NULL
);

-- Creating the Productions table
CREATE TABLE Productions (
    productionid INT PRIMARY KEY,
    machineid INT FOREIGN KEY REFERENCES Machines(machineid),
    year INT,
    month INT,
    day INT,
    hourofday INT CHECK (hourofday >= 0 AND hourofday <= 24),
    make int -- Adjust the data type based on the actual nature of 'make'
);

