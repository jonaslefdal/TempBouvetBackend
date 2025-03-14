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
INSERT INTO challenges ("Description", "Points", "MaxAttempts", "RotationGroup", "CreatedAt")
VALUES
  ('Walk in the woods', 100, 3, 1, CURRENT_TIMESTAMP),
  ('Cycle for 30 minutes', 80, 3, 2, CURRENT_TIMESTAMP),
  ('Take public transit', 70, 4, 3, CURRENT_TIMESTAMP),
  ('Yoga session', 50, 3, 4, CURRENT_TIMESTAMP),
  ('Morning run', 90, 3, 5, CURRENT_TIMESTAMP),
  ('Swim 20 laps', 110, 4, 6, CURRENT_TIMESTAMP),
  ('Attend a fitness class', 120, 3, 7, CURRENT_TIMESTAMP),
  ('Climb stairs for 10 minutes', 80, 2, 8, CURRENT_TIMESTAMP),
  ('Dance for 15 minutes', 70, 3, 9, CURRENT_TIMESTAMP),
  ('Meditation practice', 60, 3, 10, CURRENT_TIMESTAMP);
