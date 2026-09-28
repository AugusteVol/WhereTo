# WhereTo

Starter full-stack app for discovering events and buying tickets.

## Stack

- React + Vite in `client/`
- Express + Node.js API in `server/`
- MySQL via `mysql2`

## Run locally

1. Install Node.js 20+ and MySQL.
2. Create the database and starter table: `mysql -u root -p < server/schema.sql`
3. Copy `server/.env.example` to `server/.env` and set your MySQL credentials.
4. Install dependencies: `npm run install:all`
5. Start both apps: `npm run dev`

The client runs at `http://localhost:5173` and the API at `http://localhost:4000`.

The API currently exposes `GET /api/health` and `GET /api/events`. Add ticket reservations, authentication, and event management routes in `server/src/index.js` as the project grows.