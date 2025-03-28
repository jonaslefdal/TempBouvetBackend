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


INSERT INTO Achievement ("AchievementId", "Name", "Description", "ConditionType", "Threshold")
VALUES
  (1, 'Miljøkriger', 'Bruk kollektivtransport eller sykkel 10 ganger.', 'eco_warrior', 10),
  (2, 'Skrittmester', 'Gå 5 ganger i løpet av én uke.', 'walking_weekly', 5),
  (3, 'Bussentusiast', 'Ta bussen 20 ganger.', 'bus_count', 20),
  (4, 'Syklist', 'Sykle totalt 100 km.', 'cycling_distance_total', 100),
  (5, 'Samkjøringshelt', 'Samkjør 5 ganger.', 'car_count', 5),
  (6, '15 Reiser', 'Registrer 15 reiser totalt.', 'total_entries', 15),
  (7, 'Allsidig Reisende', 'Bruk 4 forskjellige transportmidler i én uke.', 'versatile_weekly', 1),
  (8, 'Midnattsreisende', 'Reis mellom kl. 00:00 og 05:00.', 'midnight_ride', 1),
  (9, 'Utfordringsmester', 'Fullfør 3 egendefinerte utfordringer.', 'custom_challenge_count', 3),
  (10, 'Prestasjonssamler', 'Lås opp 5 forskjellige prestasjoner.', 'achievement_count', 5);

