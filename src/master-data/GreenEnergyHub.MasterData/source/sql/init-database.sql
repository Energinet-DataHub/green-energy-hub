-- Initial DDL script (destructive: only run when initializing database)

-- DROP ANY EXISTING OBJECTS
DROP TABLE IF EXISTS dbo.MarketParticipantAddress
DROP TABLE IF EXISTS dbo.Relationship
DROP TABLE IF EXISTS dbo.MarketParticipant
DROP TABLE IF EXISTS dbo.MeterGroup
DROP TABLE IF EXISTS dbo.RelationshipType
DROP TABLE IF EXISTS dbo.RelationshipStates
DROP TABLE IF EXISTS dbo.MarketEvaluationPoint
DROP TABLE IF EXISTS dbo.MarketEvaluationPointType
DROP TABLE IF EXISTS dbo.Address
DROP TABLE IF EXISTS dbo.AddressType
DROP TABLE IF EXISTS dbo.MarketEvaluationPointGroup
GO

-- CREATE TABLES
CREATE TABLE dbo.MarketParticipant(
    mRID   INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    [Name] NVARCHAR(255) NOT NULL
)
GO
CREATE TABLE dbo.MeterGroup(
    MeterGroupId INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    [Name]       NVARCHAR(255) NOT NULL
)
GO
CREATE TABLE dbo.RelationshipType(
    RelationshipTypeId INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    [Name]             NVARCHAR(255) NOT NULL
)
GO
CREATE TABLE dbo.RelationshipStates(
    RelationshipStateId INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    [Name]              NVARCHAR(255) NOT NULL
)
GO
CREATE TABLE dbo.MarketEvaluationPointGroup(
    mRID         INT NOT NULL,
    MeterGroupId INT NOT NULL,
    CONSTRAINT PK_MarketEvaluationPointGroup PRIMARY KEY (mRID, MeterGroupId)
)
GO
CREATE TABLE dbo.Relationship(
    mRID                 INT NOT NULL,
    MeterGroupId         INT NOT NULL,
    RelationshipTypeId   INT NOT NULL,
    RelationshipStateId  INT NOT NULL,
    CorrelationId        INT NOT NULL,
    StartDate            DATETIME NOT NULL,
    EndDate              DATETIME NOT NULL,
    CONSTRAINT PK_Relationship PRIMARY KEY (mRID, MeterGroupId),
    CONSTRAINT FK_Relationship_MarketParticipant FOREIGN KEY (mRID) REFERENCES dbo.MarketParticipant (mRID),
    CONSTRAINT FK_Relationship_MeterGroup FOREIGN KEY (MeterGroupId) REFERENCES dbo.MeterGroup (MeterGroupId)
)
GO
CREATE TABLE dbo.Address(
    AddressId           INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    StreetName          NVARCHAR(255) NULL,
    StreetCode          NVARCHAR(255) NULL,
    BuildNumber         NVARCHAR(255) NULL,
    FloorIdentification NVARCHAR(255) NULL,
    RoomIdentification  NVARCHAR(255) NULL,
    PostOfficeBox       NVARCHAR(255) NULL,
    CitySubDivisionName NVARCHAR(255) NULL,
    Postcode            NVARCHAR(255) NULL,
    CityName            NVARCHAR(255) NULL,
    MunicipalityCode    NVARCHAR(255) NULL,
    CountryName         NVARCHAR(255) NULL
)
GO
CREATE TABLE dbo.AddressType(
    AddressTypeId INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    [Name]        NVARCHAR(255) NOT NULL
)
GO
CREATE TABLE dbo.MarketParticipantAddress(
    mRID          INT NOT NULL,
    AddressId     INT NOT NULL,
    AddressTypeId INT NOT NULL,
    CONSTRAINT PK_MarketPaticipantAddress PRIMARY KEY (mRID, AddressId),
    CONSTRAINT FK_MarketParticipantAddress_MarketParticipant FOREIGN KEY (mRID) REFERENCES dbo.MarketParticipant (mRID),
    CONSTRAINT FK_MarketParticipantAddress_Address FOREIGN KEY (AddressId) REFERENCES dbo.Address (AddressId),
    CONSTRAINT FK_MarketParticipantAddress_Type FOREIGN KEY (AddressTypeId) REFERENCES dbo.AddressType (AddressTypeId)
)
GO
CREATE TABLE dbo.MarketEvaluationPointType(
    MarketEvaluationPointTypeId INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    [Name]              NVARCHAR(255) NOT NULL
)
GO
CREATE TABLE dbo.MarketEvaluationPoint(
    mRID                INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    MarketEvaluationPointTypeId INT NOT NULL,
    AddressId           INT NOT NULL,
    CONSTRAINT FK_MarketEvaluationPoint_Type FOREIGN KEY (MarketEvaluationPointTypeId) REFERENCES dbo.MarketEvaluationPointType (MarketEvaluationPointTypeId),
    CONSTRAINT FK_MarketEvaluationPoint_Address FOREIGN KEY (AddressId) REFERENCES dbo.Address (AddressId)
)
GO
