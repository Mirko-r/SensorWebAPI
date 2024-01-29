-- Creating the Machines table
CREATE TABLE Machines (
    machineid varchar(50) PRIMARY KEY,
    location VARCHAR(255) NOT NULL
);

-- Creating the Productions table
CREATE TABLE Productions (
    productionid INT PRIMARY KEY,
    machineid varchar(50) FOREIGN KEY REFERENCES Machines(machineid),
    year INT,
    month INT,
    day INT,
    hourofday INT CHECK (hourofday >= 0 AND hourofday <= 24),
    make int -- Adjust the data type based on the actual nature of 'make'
);

<<<<<<< HEAD

insert into Machines (machineid, location) 
values('LSD23431OP', 'Milan'),
('LLOPRER13P', 'Rome')
=======
>>>>>>> 855ba0e6190e875d5ee33465a7d1b07b7dea8bc9
