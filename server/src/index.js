import cors from 'cors'
import express from 'express'
import 'dotenv/config'
import { pool } from './db.js'

const app = express()
const port = Number(process.env.PORT || 4000)

app.use(cors({ origin: process.env.CLIENT_ORIGIN || 'http://localhost:5173' }))
app.use(express.json())

app.get('/api/health', (_request, response) => response.json({ status: 'ok' }))

app.post('/api/organizations', async (request, response) => {
  const { name, description = null, email = null, website = null, isApproved = false } = request.body

  if (!name || !name.trim()) {
    return response.status(400).json({ message: 'Organization name is required.' })
  }

  try {
    const [result] = await pool.execute(
      `INSERT INTO organizations (name, description, email, website, is_approved)
       VALUES (?, ?, ?, ?, ?)`,
      [name.trim(), description, email, website, Boolean(isApproved)],
    )

    const [organizations] = await pool.execute(
      'SELECT id, name, description, email, website, is_approved AS isApproved, created_at AS createdAt FROM organizations WHERE id = ?',
      [result.insertId],
    )

    return response.status(201).json(organizations[0])
  } catch (error) {
    console.error(error)
    return response.status(500).json({ message: 'Unable to create organization.' })
  }
})
//GET BE ID
app.get('/api/organizations', async (_request, response) => {
  try {
    const [organizations] = await pool.execute(
      `SELECT id, name, description, email, website,
              is_approved AS isApproved, created_at AS createdAt
       FROM organizations ORDER BY name ASC`,
    )

    return response.json(organizations)
  } catch (error) {
    console.error(error)
    return response.status(500).json({ message: 'Unable to load organizations.' })
  }
})
// CIA SU ID
app.get('/api/organizations/:id', async (request, response) => {
  try {
    const [organizations] = await pool.execute(
      `SELECT id, name, description, email, website,
              is_approved AS isApproved, created_at AS createdAt
       FROM organizations WHERE id = ?`,
      [request.params.id],
    )

    if (!organizations.length) {
      return response.status(404).json({ message: 'Organization not found.' })
    }

    return response.json(organizations[0])
  } catch (error) {
    console.error(error)
    return response.status(500).json({ message: 'Unable to load organization.' })
  }
})

app.put('/api/organizations/:id', async (request, response) => {
  const { name, description = null, email = null, website = null, isApproved = false } = request.body

  if (!name || !name.trim()) {
    return response.status(400).json({ message: 'Organization name is required.' })
  }

  try {
    const [result] = await pool.execute(
      `UPDATE organizations
       SET name = ?, description = ?, email = ?, website = ?, is_approved = ?
       WHERE id = ?`,
      [name.trim(), description, email, website, Boolean(isApproved), request.params.id],
    )

    if (!result.affectedRows) {
      return response.status(404).json({ message: 'Organization not found.' })
    }

    const [organizations] = await pool.execute(
      'SELECT id, name, description, email, website, is_approved AS isApproved, created_at AS createdAt FROM organizations WHERE id = ?',
      [request.params.id],
    )

    return response.json(organizations[0])
  } catch (error) {
    console.error(error)
    return response.status(500).json({ message: 'Unable to update organization.' })
  }
})

app.delete('/api/organizations/:id', async (request, response) => {
  try {
    const [result] = await pool.execute('DELETE FROM organizations WHERE id = ?', [request.params.id])

    if (!result.affectedRows) {
      return response.status(404).json({ message: 'Organization not found.' })
    }

    return response.status(204).send()
  } catch (error) {
    console.error(error)
    return response.status(500).json({ message: 'Unable to delete organization.' })
  }
})

app.get('/api/events', async (_request, response) => {
  try {
    const [events] = await pool.query(`
      SELECT id, title, category, venue, city, event_date AS date, starting_price AS price
      FROM events WHERE event_date >= CURDATE() ORDER BY event_date ASC
    `)
    response.json(events)
  } catch (error) {
    console.error(error)
    response.status(500).json({ message: 'Unable to load events.' })
  }
})

app.listen(port, () => console.log(`API running at http://localhost:${port}`))