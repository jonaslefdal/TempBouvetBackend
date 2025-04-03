-- Insert two companies
INSERT INTO companies ("Name") VALUES
('Ingen Firma Valgt'),
('Company A'),
('Company B');

-- Insert four users
INSERT INTO users ("AzureId", "Name", "NickName", "Email", "CompanyId", "TeamId", "TotalScore") VALUES
('azure-uuid-101', 'Emma Hansen', 'Emma', 'emma.hansen@example.com', 1, 1, 7243),
('azure-uuid-102', 'Jonas Berg', 'Jonas', 'jonas.berg@example.com', 2, 1, 8391),
('azure-uuid-103', 'Ingrid Larsen', 'Ingrid', 'ingrid.larsen@example.com', 1, 1, 5120),

('azure-uuid-104', 'Oskar Nilsen', 'Oskar', 'oskar.nilsen@example.com', 2, 2, 9502),
('azure-uuid-105', 'Sofie Aas', 'Sofie', 'sofie.aas@example.com', 1, 2, 6133),
('azure-uuid-106', 'Marius Solberg', 'Marius', 'marius.solberg@example.com', 2, 2, 7730),

('azure-uuid-107', 'Ida Johansen', 'Ida', 'ida.johansen@example.com', 1, 3, 8850),
('azure-uuid-108', 'Henrik Olsen', 'Henrik', 'henrik.olsen@example.com', 2, 3, 5217),
('azure-uuid-109', 'Nora Lie', 'Nora', 'nora.lie@example.com', 1, 3, 9034),

('azure-uuid-110', 'Emil Kristoffersen', 'Emil', 'emil.kristoffersen@example.com', 2, 4, 9943),
('azure-uuid-111', 'Linnea Evensen', 'Linnea', 'linnea.evensen@example.com', 1, 4, 6001),
('azure-uuid-112', 'Lars Hagen', 'Lars', 'lars.hagen@example.com', 2, 4, 7305),

('azure-uuid-113', 'Thea Andersen', 'Thea', 'thea.andersen@example.com', 1, 5, 8052),
('azure-uuid-114', 'Sander Moe', 'Sander', 'sander.moe@example.com', 2, 5, 9609),
('azure-uuid-115', 'Mia Halvorsen', 'Mia', 'mia.halvorsen@example.com', 1, 5, 5487),

('azure-uuid-116', 'Elias Bakken', 'Elias', 'elias.bakken@example.com', 2, 6, 7492),
('azure-uuid-117', 'Aurora Skogen', 'Aurora', 'aurora.skogen@example.com', 1, 6, 8674),
('azure-uuid-118', 'Noah Andresen', 'Noah', 'noah.andresen@example.com', 2, 6, 9940),

('azure-uuid-119', 'Julie Tangen', 'Julie', 'julie.tangen@example.com', 1, 7, 6890),
('azure-uuid-120', 'Oliver Brekke', 'Oliver', 'oliver.brekke@example.com', 2, 8, 9155);

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


