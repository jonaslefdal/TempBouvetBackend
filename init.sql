-- Insert two companies
INSERT INTO companies ("Name") VALUES
('Ingen Firma Valgt'),
('Company A'),
('Company B');

-- Insert four users
INSERT INTO users ("AzureId", "Name", "Email", "CompanyId", "TotalScore") VALUES
('azure-uuid-1', 'Alice Johnson', 'alice@example.com', 1, 1200),
('azure-uuid-2', 'Bob Smith', 'bob@example.com', 1, 900),
('azure-uuid-3', 'Charlie Brown', 'charlie@example.com', 2, 1500),
('azure-uuid-4', 'Diana Prince', 'diana@example.com', 2, 1100);

-- Insert Challenges
INSERT INTO challenges
("Description", "Points", "RotationGroup", "MaxAttempts", "RequiredTransportMethod", "ConditionType", "RequiredDistanceKm", "CreatedAt")
VALUES
-- Rotation Group 1
('Ta bussen til jobb minst 3 ganger', 30, 1, 3, 'bus', 'Standard', NULL, NOW()),
('Sykle totalt minst 15 km denne uken', 40, 1, 1, 'cycling', 'Distance', 15, NOW()),
('Gå en tur med en kollega', 50, 1, 1, 'custom', 'Custom', NULL, NOW()),

-- Rotation Group 2
('Gå til jobb minst 2 ganger', 30, 2, 2, 'walking', 'Standard', NULL, NOW()),
('Samkjør minst 20 km denne uken', 40, 2, 1, 'car', 'Distance', 20, NOW()),
('Delta på en organisert treningsøkt', 50, 2, 1, 'custom', 'Custom', NULL, NOW()),

-- Rotation Group 3
('Ta bussen minst 4 ganger', 30, 3, 4, 'bus', 'Standard', NULL, NOW()),
('Gå totalt minst 10 km', 40, 3, 1, 'walking', 'Distance', 10, NOW()),
('Ta en løpetur på minst 5 km', 50, 3, 1, 'custom', 'Custom', 5, NOW()),

-- Rotation Group 4
('Sykle til jobb 2 ganger', 30, 4, 2, 'cycling', 'Standard', NULL, NOW()),
('Gå til jobb minst én gang', 30, 4, 1, 'walking', 'Standard', NULL, NOW()),
('Utforsk en ny tursti', 50, 4, 1, 'custom', 'Custom', NULL, NOW()),

-- Rotation Group 5
('Gå til jobb minst én gang', 30, 5, 1, 'walking', 'Standard', NULL, NOW()),
('Sykle totalt 20 km denne uken', 40, 5, 1, 'cycling', 'Distance', 20, NOW()),
('Dra på en fjelltur', 50, 5, 1, 'custom', 'Custom', NULL, NOW()),

-- Rotation Group 6
('Bruk kollektivtransport minst 3 ganger', 30, 6, 3, 'bus', 'Standard', NULL, NOW()),
('Gå totalt minst 15 km denne uken', 40, 6, 1, 'walking', 'Distance', 15, NOW()),
('Delta i en gruppetreningstime', 50, 6, 1, 'custom', 'Custom', NULL, NOW()),

-- Rotation Group 7
('Samkjør minst 2 ganger', 30, 7, 2, 'car', 'Standard', NULL, NOW()),
('Sykle minst 25 km totalt', 40, 7, 1, 'cycling', 'Distance', 25, NOW()),
('Ta en yogatime', 50, 7, 1, 'custom', 'Custom', NULL, NOW()),

-- Rotation Group 8
('Gå minst 3 ganger til jobb', 30, 8, 3, 'walking', 'Standard', NULL, NOW()),
('Sykle totalt 20 km denne uken', 40, 8, 1, 'cycling', 'Distance', 20, NOW()),
('Joggetur med venner eller familie', 50, 8, 1, 'custom', 'Custom', NULL, NOW()),

-- Rotation Group 9
('Sykle minst 2 ganger', 30, 9, 2, 'cycling', 'Standard', NULL, NOW()),
('Gå minst 5 km totalt denne uken', 40, 9, 1, 'walking', 'Distance', 5, NOW()),
('Besøk en ny park eller friluftsområde', 50, 9, 1, 'custom', 'Custom', NULL, NOW()),

-- Rotation Group 10
('Samkjør minst 3 ganger til jobb', 30, 10, 3, 'car', 'Standard', NULL, NOW()),
('Sykle minst 30 km denne uken', 40, 10, 1, 'cycling', 'Distance', 30, NOW()),
('Fullfør en valgfri fysisk aktivitet med andre', 50, 10, 1, 'custom', 'Custom', NULL, NOW());



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
(28, 'Opplåser I',       9, 3,    'Lås opp 3 unike utfordringer'),
(29, 'Opplåser II',      9, 6,    'Lås opp 6 unike utfordringer'),
(30, 'Opplåser III',     9, 10,   'Lås opp 10 unike utfordringer');


