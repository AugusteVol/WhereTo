CREATE DATABASE IF NOT EXISTS whereto;
USE whereto;

CREATE TABLE IF NOT EXISTS organizations (
  id INT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
  name VARCHAR(150) NOT NULL,
  description TEXT,
  email VARCHAR(150),
  website VARCHAR(255),
  is_approved BOOLEAN NOT NULL DEFAULT FALSE,
  created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE IF NOT EXISTS events (
  id INT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
  organization_id INT UNSIGNED,
  title VARCHAR(100) NOT NULL,
  category VARCHAR(80) NOT NULL,
  description TEXT,
  venue VARCHAR(150) NOT NULL,
  city VARCHAR(100) NOT NULL,
  event_date DATETIME NOT NULL,
  starting_price DECIMAL(10, 2) NOT NULL DEFAULT 0,
  total_tickets INT UNSIGNED NOT NULL DEFAULT 0,
  available_tickets INT UNSIGNED NOT NULL DEFAULT 0,
  created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  CONSTRAINT fk_events_organization
    FOREIGN KEY (organization_id) REFERENCES organizations(id)
    ON DELETE SET NULL
);

INSERT INTO organizations (name, description, email, website, is_approved)
VALUES
  ('Vilniaus kultūros centras', 'Kultūros renginių organizatorius.', 'info@kultura.example', 'https://kultura.example', TRUE),
  ('WhereTo Events', 'Nepriklausomų renginių organizatorius.', 'events@whereto.example', 'https://whereto.example', TRUE);

INSERT INTO events (organization_id, title, category, venue, city, event_date, starting_price, total_tickets, available_tickets)
VALUES
  (1, 'Night Market Sessions', 'Music', 'The Foundry', 'Austin, TX', '2025-06-14 19:00:00', 28, 300, 300),
  (2, 'Designing Tomorrow', 'Talks', 'The Line Hotel', 'Austin, TX', '2025-06-21 10:00:00', 45, 180, 180),
  (1, 'Sunday Clay Club', 'Workshop', 'Eastside Ceramics', 'Austin, TX', '2025-06-29 14:00:00', 32, 24, 24);