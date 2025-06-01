-- Insert two companies
INSERT INTO companies ("Name") VALUES
('Hennig-Olsen Is'),
('Glencore Nikkelverk AS');

-- Insert Challenges
INSERT INTO challenges
("Description", "Points", "RotationGroup", "MaxAttempts", "RequiredTransportMethod", "ConditionType", "RequiredDistanceKm", "CreatedAt")
VALUES
-- Rotation Group 1
('Ta bussen til jobb minst 3 ganger', 30, 1, 3, 3, 'Standard', NULL, NOW()),
('Sykle totalt minst 15 km denne uken', 40, 1, 1, 1, 'Distance', 15, NOW()),
('Gå en tur med en kollega', 50, 1, 1, 4, 'Custom', NULL, NOW()),

-- Rotation Group 2
('Gå til jobb minst 2 ganger', 30, 2, 2, 2, 'Standard', NULL, NOW()),
('Samkjør minst 20 km denne uken', 40, 2, 1, 0, 'Distance', 20, NOW()),
('Delta på en organisert treningsøkt', 50, 2, 1, 4, 'Custom', NULL, NOW()),

-- Rotation Group 3
('Ta bussen minst 4 ganger', 30, 3, 4, 3, 'Standard', NULL, NOW()),
('Gå totalt minst 10 km', 40, 3, 1, 2, 'Distance', 10, NOW()),
('Ta en løpetur på minst 5 km', 50, 3, 1, 4, 'Custom', 5, NOW()),

-- Rotation Group 4
('Sykle til jobb 2 ganger', 30, 4, 2, 1, 'Standard', NULL, NOW()),
('Gå til jobb minst én gang', 30, 4, 1, 2, 'Standard', NULL, NOW()),
('Utforsk en ny tursti', 50, 4, 1, 4, 'Custom', NULL, NOW()),

-- Rotation Group 5
('Gå til jobb minst én gang', 30, 5, 1, 2, 'Standard', NULL, NOW()),
('Sykle totalt 20 km denne uken', 40, 5, 1, 1, 'Distance', 20, NOW()),
('Dra på en fjelltur', 50, 5, 1, 4, 'Custom', NULL, NOW()),

-- Rotation Group 6
('Bruk kollektivtransport minst 3 ganger', 30, 6, 3, 3, 'Standard', NULL, NOW()),
('Gå totalt minst 15 km denne uken', 40, 6, 1, 2, 'Distance', 15, NOW()),
('Delta i en gruppetreningstime', 50, 6, 1, 4, 'Custom', NULL, NOW()),

-- Rotation Group 7
('Samkjør minst 2 ganger', 30, 7, 2, 0, 'Standard', NULL, NOW()),
('Sykle minst 25 km totalt', 40, 7, 1, 1, 'Distance', 25, NOW()),
('Ta en yogatime', 50, 7, 1, 4, 'Custom', NULL, NOW()),

-- Rotation Group 8
('Gå minst 3 ganger til jobb', 30, 8, 3, 2, 'Standard', NULL, NOW()),
('Sykle totalt 20 km denne uken', 40, 8, 1, 1, 'Distance', 20, NOW()),
('Joggetur med venner eller familie', 50, 8, 1, 4, 'Custom', NULL, NOW()),

-- Rotation Group 9
('Sykle minst 2 ganger', 30, 9, 2, 1, 'Standard', NULL, NOW()),
('Gå minst 5 km totalt denne uken', 40, 9, 1, 2, 'Distance', 5, NOW()),
('Besøk en ny park eller friluftsområde', 50, 9, 1, 4, 'Custom', NULL, NOW()),

-- Rotation Group 10
('Samkjør minst 3 ganger til jobb', 30, 10, 3, 0, 'Standard', NULL, NOW()),
('Sykle minst 30 km denne uken', 40, 10, 1, 1, 'Distance', 30, NOW()),
('Fullfør en valgfri fysisk aktivitet med andre', 50, 10, 1, 4, 'Custom', NULL, NOW());



INSERT INTO achievement ("AchievementId", "Name", "ConditionType", "Threshold", "Description") VALUES
-- Walking
(1,  'Turgåer I',        0, 10,   'Gå totalt 10 km'),
(2,  'Turgåer II',       0, 50,   'Gå totalt 50 km'),
(3,  'Turgåer III',      0, 100,  'Gå totalt 100 km'),

-- Cycling
(4,  'Syklist I',        1, 10,   'Sykle totalt 10 km'),
(5,  'Syklist II',       1, 50,   'Sykle totalt 50 km'),
(6,  'Syklist III',      1, 100,  'Sykle totalt 100 km'),

-- Bus
(7,  'Bussreisende I',   2, 10,   'Reis 10 km med buss'),
(8,  'Bussreisende II',  2, 50,   'Reis 50 km med buss'),
(9,  'Bussreisende III', 2, 100,  'Reis 100 km med buss'),

-- Car
(10, 'Samkjører I',      3, 10,   'Kjør 10 km med bil'),
(11, 'Samkjører II',     3, 50,   'Kjør 50 km med bil'),
(12, 'Samkjører III',    3, 100,  'Kjør 100 km med bil'),

-- Total entries
(13, 'Utforsker I',      4, 5,    'Registrer 5 reiser totalt'),
(14, 'Utforsker II',     4, 20,   'Registrer 20 reiser totalt'),
(15, 'Utforsker III',    4, 50,   'Registrer 50 reiser totalt'),

-- Custom challenges
(16, 'Egen vei I',       5, 1,    'Fullfør 1 egendefinert utfordring'),
(17, 'Egen vei II',      5, 5,    'Fullfør 5 egendefinerte utfordringer'),
(18, 'Egen vei III',     5, 10,   'Fullfør 10 egendefinerte utfordringer'),

-- Points
(19, 'Poengjeger I',     6, 100,  'Samle 100 poeng'),
(20, 'Poengjeger II',    6, 500,  'Samle 500 poeng'),
(21, 'Poengjeger III',   6, 1000, 'Samle 1000 poeng'),

-- CO₂ saved
(22, 'CO₂-sparer I',     7, 5,    'Spar 5 kg CO₂'),
(23, 'CO₂-sparer II',    7, 20,   'Spar 20 kg CO₂'),
(24, 'CO₂-sparer III',   7, 50,   'Spar 50 kg CO₂'),

-- Money saved
(25, 'Pengebesparer I',  8, 50,   'Spar 50 kroner'),
(26, 'Pengebesparer II', 8, 200,  'Spar 200 kroner'),
(27, 'Pengebesparer III',8, 500,  'Spar 500 kroner'),

-- Challenges unlocked
(28, 'Opplåser I',       10, 3,    'Lås opp 3 unike utfordringer'),
(29, 'Opplåser II',      10, 6,    'Lås opp 6 unike utfordringer'),
(30, 'Opplåser III',     10, 10,   'Lås opp 10 unike utfordringer');


-- Eksempel teamnavn og bilder
INSERT INTO teams ("Name", "TeamProfilePicture", "CompanyId", "MaxMembers")
VALUES 
-- Bedrift 1 (CompanyId = 1)
('Team Aurora',      'teamAvatar3.png', 1, 5),
('Team Nimbus',      'teamAvatar12.png', 1, 5),
('Team Polar Force', 'teamAvatar7.png', 1, 5),
('Team Trailblazers','teamAvatar19.png', 1, 5),
('Team Velocity',    'teamAvatar1.png', 1, 5),

-- Bedrift 2 (CompanyId = 2)
('Team Zenith',      'teamAvatar5.png', 2, 5),
('Team EcoMotion',   'teamAvatar17.png', 2, 5),
('Team GreenWheels', 'teamAvatar9.png', 2, 5),
('Team SwiftSteps',  'teamAvatar14.png', 2, 5),
('Team Skybound',    'teamAvatar20.png', 2, 5);


INSERT INTO users ("AzureId", "Email", "Name", "CompanyId", "TotalScore", "NickName", "Address", "TeamId", "ProfilePicture")
VALUES
-- Bedrift 1, Lag 1–5
('azure-uid-001', 'user1@example.com', 'User One', 1, FLOOR(RANDOM() * 4501 + 500), 'MiljøRev',  'Some Street 1', 1, 'Avatar1.png'),
('azure-uid-002', 'user2@example.com', 'User Two', 1, FLOOR(RANDOM() * 4501 + 500), 'GåMester', NULL, 2, 'Avatar2.png'),
('azure-uid-003', 'user3@example.com', 'User Three', 1, FLOOR(RANDOM() * 4501 + 500), 'SykkelSjef', 'Green Ave 3', 3, 'Avatar3.png'),
('azure-uid-004', 'user4@example.com', 'User Four', 1, FLOOR(RANDOM() * 4501 + 500), 'BussMagiker', NULL, 4, 'Avatar4.png'),
('azure-uid-005', 'user5@example.com', 'User Five', 1, FLOOR(RANDOM() * 4501 + 500), 'CO2Knuser', NULL, 5, 'Avatar5.png'),
('azure-uid-006', 'user6@example.com', 'User Six', 1, FLOOR(RANDOM() * 4501 + 500), 'Skrittkonge', NULL, 1, 'Avatar6.png'),
('azure-uid-007', 'user7@example.com', 'User Seven', 1, FLOOR(RANDOM() * 4501 + 500), 'StiDronning', NULL, 2, 'Avatar7.png'),
('azure-uid-008', 'user8@example.com', 'User Eight', 1, FLOOR(RANDOM() * 4501 + 500), 'Fartsfantom', NULL, 3, 'Avatar8.png'),
('azure-uid-009', 'user9@example.com', 'User Nine', 1, FLOOR(RANDOM() * 4501 + 500), 'Tråkkekamerat', NULL, 4, 'Avatar9.png'),
('azure-uid-010', 'user10@example.com', 'User Ten', 1, FLOOR(RANDOM() * 4501 + 500), 'Miljøstråle', NULL, 5, 'Avatar10.png'),

-- Bedrift 2, Lag 6–10
('azure-uid-011', 'user11@example.com', 'User Eleven', 2, FLOOR(RANDOM() * 4501 + 500), 'LøvLøper', NULL, 6, 'Avatar11.png'),
('azure-uid-012', 'user12@example.com', 'User Twelve', 2, FLOOR(RANDOM() * 4501 + 500), 'Turstjerne', NULL, 7, 'Avatar12.png'),
('azure-uid-013', 'user13@example.com', 'User Thirteen', 2, FLOOR(RANDOM() * 4501 + 500), 'Sykkelpilot', NULL, 8, 'Avatar13.png'),
('azure-uid-014', 'user14@example.com', 'User Fourteen', 2, FLOOR(RANDOM() * 4501 + 500), 'Luftbøyer', NULL, 9, 'Avatar14.png'),
('azure-uid-015', 'user15@example.com', 'User Fifteen', 2, FLOOR(RANDOM() * 4501 + 500), 'VoltVandrer', NULL, 10, 'Avatar15.png'),
('azure-uid-016', 'user16@example.com', 'User Sixteen', 2, FLOOR(RANDOM() * 4501 + 500), 'T-baneTass', NULL, 6, 'Avatar16.png'),
('azure-uid-017', 'user17@example.com', 'User Seventeen', 2, FLOOR(RANDOM() * 4501 + 500), 'Fartsmunk', NULL, 7, 'Avatar10.png'),
('azure-uid-018', 'user18@example.com', 'User Eighteen', 2, FLOOR(RANDOM() * 4501 + 500), 'SykkelReaper', NULL, 8, 'Avatar5.png'),
('azure-uid-019', 'user19@example.com', 'User Nineteen', 2, FLOOR(RANDOM() * 4501 + 500), 'MiljøSkygge', NULL, 9, 'Avatar7.png'),
('azure-uid-020', 'user20@example.com', 'User Twenty', 2, FLOOR(RANDOM() * 4501 + 500), 'BussNinja', NULL, 10, 'Avatar14.png');
