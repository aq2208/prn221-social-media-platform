--create database PRN221_SocialMedia
--go

use PRN221_SocialMedia
go

-- create tables
CREATE TABLE Users (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Username NVARCHAR(50) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(256) NOT NULL,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETDATE()
);

USE [PRN221_SocialMedia]
CREATE TABLE UserProfiles (
    Id INT PRIMARY KEY,
    UserId INT NOT NULL,
    ProfilePicture NVARCHAR(500),
	CoverPicture NVARCHAR(500),
	Nickname NVARCHAR(500),
    Bio NVARCHAR(500),
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
);

DROP TABLE [PRN221_SocialMedia].dbo.UserProfiles;

ALTER TABLE [PRN221_SocialMedia].dbo.UserProfiles
ADD PhoneNumber varchar(20) NOT NULL;

ALTER TABLE UserProfiles
ADD ProfileImage VARBINARY(MAX), CoverImage VARBINARY(MAX);


CREATE TABLE Posts (
    Id INT PRIMARY KEY IDENTITY(1,1),
    UserId INT NOT NULL,
	Title NVARCHAR(MAX) NOT NULL,
	Media NVARCHAR(500),
    Content NVARCHAR(MAX) NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
);

ALTER TABLE Posts
ADD PostImage VARBINARY(MAX);

CREATE TABLE Comments (
    Id INT PRIMARY KEY IDENTITY(1,1),
    PostId INT NOT NULL,
    UserId INT NOT NULL,
    Content NVARCHAR(MAX) NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
	FOREIGN KEY (PostId) REFERENCES Posts(Id) ON DELETE CASCADE,
    FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE NO ACTION
);

CREATE TABLE Likes (
    Id INT PRIMARY KEY IDENTITY(1,1),
    PostId INT NOT NULL,
    UserId INT NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
	FOREIGN KEY (PostId) REFERENCES Posts(Id) ON DELETE CASCADE,
    FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE NO ACTION
);

CREATE TABLE ChatMessages (
    Id INT PRIMARY KEY IDENTITY(1,1),
    SenderId INT NOT NULL,
    ReceiverId INT NOT NULL,
    Content NVARCHAR(MAX) NOT NULL,
	Media NVARCHAR(MAX),
    SentAt DATETIME2 NOT NULL DEFAULT GETDATE(),
	FOREIGN KEY (SenderId) REFERENCES Users(Id) ON DELETE CASCADE,
    FOREIGN KEY (ReceiverId) REFERENCES Users(Id) ON DELETE NO ACTION
);



-- insert data
INSERT INTO Users (Username, PasswordHash, Email) VALUES
('test_user1', 'hashed_password1', 'test_user1@example.com');

INSERT INTO [PRN221_SocialMedia].dbo.Users (Username, PasswordHash, Email) VALUES
('user2', '123456', 'quancuanam2003@gmail.com');

INSERT INTO [PRN221_SocialMedia].dbo.Users (Username, PasswordHash, Email) VALUES
('aq2208', '8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92', 'aq2208@gmail.com');

insert into [PRN221_SocialMedia].dbo.UserProfiles (UserId, PhoneNumber) values
(4, '0123456789');

-- "8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92"

-- query data
select * from [PRN221_SocialMedia].dbo.Users;
select * from [PRN221_SocialMedia].dbo.UserProfiles where UserId = 14;
select * from [PRN221_SocialMedia].dbo.Posts;
select * from [PRN221_SocialMedia].dbo.Comments;
select * from [PRN221_SocialMedia].dbo.ChatMessages;

-- delete from [PRN221_SocialMedia].dbo.Users where [PRN221_SocialMedia].dbo.Users.Id in (1,2,3,4,5,6,7,8,9,10,11,12,13);

insert into [PRN221_SocialMedia].dbo.ChatMessages (SenderId, ReceiverId, Content) values 
(15, 14, 'Im Naruto');
