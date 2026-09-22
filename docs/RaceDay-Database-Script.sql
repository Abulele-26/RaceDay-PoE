
-- 1. CREATE TABLES


-- Create Users table
CREATE TABLE Users (
    UserId INT IDENTITY(1,1) PRIMARY KEY,
    Email NVARCHAR(255) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
    FullName NVARCHAR(100) NOT NULL,
    Role NVARCHAR(20) NOT NULL CHECK (Role IN ('Organiser', 'Participant')),
    CreatedAt DATETIME DEFAULT GETDATE()
);

-- Create Events table
CREATE TABLE Events (
    EventId INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500),
    EventDate DATETIME NOT NULL,
    Location NVARCHAR(200) NOT NULL,
    OrganiserId INT NOT NULL,
    FOREIGN KEY (OrganiserId) REFERENCES Users(UserId)
);

-- Create Categories table
CREATE TABLE Categories (
    CategoryId INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(50) NOT NULL,
    Distance DECIMAL(5,2) NOT NULL,
    EntryFee DECIMAL(10,2) NOT NULL,
    EventId INT NOT NULL,
    FOREIGN KEY (EventId) REFERENCES Events(EventId)
);

-- Create Enrollments table
CREATE TABLE Enrollments (
    EnrollmentId INT IDENTITY(1,1) PRIMARY KEY,
    EnrollmentDate DATETIME DEFAULT GETDATE(),
    Status NVARCHAR(20) DEFAULT 'Pending',
    UserId INT NOT NULL,
    CategoryId INT NOT NULL,
    FOREIGN KEY (UserId) REFERENCES Users(UserId),
    FOREIGN KEY (CategoryId) REFERENCES Categories(CategoryId)
);

-- Create Results table
CREATE TABLE Results (
    ResultId INT IDENTITY(1,1) PRIMARY KEY,
    FinishTime TIME NOT NULL,
    Position INT,
    EnrollmentId INT NOT NULL UNIQUE,
    FOREIGN KEY (EnrollmentId) REFERENCES Enrollments(EnrollmentId)
);

-- Create Routes table
CREATE TABLE Routes (
    RouteId INT IDENTITY(1,1) PRIMARY KEY,
    RouteName NVARCHAR(100) NOT NULL,
    MapUrl NVARCHAR(500),
    TerrainType NVARCHAR(50),
    EventId INT NOT NULL,
    FOREIGN KEY (EventId) REFERENCES Events(EventId)
);

-- Create Weather table
CREATE TABLE Weather (
    WeatherId INT IDENTITY(1,1) PRIMARY KEY,
    Temperature DECIMAL(4,1),
    Condition NVARCHAR(50),
    WindSpeed DECIMAL(4,1),
    EventId INT NOT NULL,
    FOREIGN KEY (EventId) REFERENCES Events(EventId)
);


-- 2. INSERT SEED DATA


-- Seed Users (2 Organisers, 2 Participants)
INSERT INTO Users (Email, PasswordHash, FullName, Role) VALUES 
('organiser1@raceday.co.za', 'hashed_pw_1', 'Thabo Mokoena', 'Organiser'),
('organiser2@raceday.co.za', 'hashed_pw_2', 'Sarah van der Merwe', 'Organiser'),
('participant1@raceday.co.za', 'hashed_pw_3', 'Lerato Dlamini', 'Participant'),
('participant2@raceday.co.za', 'hashed_pw_4', 'David Smith', 'Participant');

-- Seed Events (3 Events)
INSERT INTO Events (Name, Description, EventDate, Location, OrganiserId) VALUES 
('Comrades Marathon', 'Iconic ultramarathon between Pietermaritzburg and Durban', '2026-06-14', 'KwaZulu-Natal', 1),
('Cape Town Cycle Tour', 'Annual cycling event around the Cape Peninsula', '2026-03-08', 'Cape Town', 2),
('Soweto Marathon', 'Annual marathon through the streets of Soweto', '2026-11-01', 'Soweto', 1);

-- Seed Categories (Categories for each event)
INSERT INTO Categories (Name, Distance, EntryFee, EventId) VALUES 
('Ultra Marathon', 89.0, 450.00, 1),
('Half Marathon', 21.1, 250.00, 1),
('42km Cycle', 42.0, 350.00, 2),
('10km Run', 10.0, 150.00, 3);

-- Seed Enrollments (Sample enrolments linking Participants to Categories)
INSERT INTO Enrollments (UserId, CategoryId, Status) VALUES 
(3, 1, 'Confirmed'),
(3, 3, 'Pending'),
(4, 4, 'Confirmed');