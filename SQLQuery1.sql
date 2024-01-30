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

create table Users(
	UserId bigint identity,
	Username varchar(50),
	Password varchar(500),
	Role varchar(500),
	HashToVerify varchar(100),
	SaltToVerify varchar(100)
)

insert into Machines (machineid, location) 
values('LSD23431OP', 'Milan'),
('LLOPRER13P', 'Rome')
