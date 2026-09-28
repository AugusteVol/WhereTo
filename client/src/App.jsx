import { useEffect, useState } from 'react'
import { CalendarDays, MapPin, Search, Ticket, ArrowRight } from 'lucide-react'

const fallbackEvents = [
  { id: 1, title: 'Night Market Sessions', category: 'Music', venue: 'The Foundry', city: 'Austin, TX', date: '2025-06-14', price: 28, accent: 'coral' },
  { id: 2, title: 'Designing Tomorrow', category: 'Talks', venue: 'The Line Hotel', city: 'Austin, TX', date: '2025-06-21', price: 45, accent: 'blue' },
  { id: 3, title: 'Sunday Clay Club', category: 'Workshop', venue: 'Eastside Ceramics', city: 'Austin, TX', date: '2025-06-29', price: 32, accent: 'yellow' },
]

function App() {
  const [events, setEvents] = useState(fallbackEvents)
  const [query, setQuery] = useState('')

  useEffect(() => {
    fetch('/api/events')
      .then((response) => (response.ok ? response.json() : Promise.reject()))
      .then(setEvents)
      .catch(() => {})
  }, [])

  const visibleEvents = events.filter((event) =>
    `${event.title} ${event.category} ${event.venue} ${event.city}`.toLowerCase().includes(query.toLowerCase()),
  )

  return (
    <main>
      <nav className="nav shell">
        <a className="wordmark" href="/">WhereTo<span>.</span></a>
        <div className="nav-links"><a href="#discover">Discover</a><a href="#for-organizers">For organizers</a></div>
        <button className="outline-button"><Ticket size={16} /> My tickets</button>
      </nav>

      <section className="hero shell">
        <div className="hero-copy">
          <p className="eyebrow">Your city, switched on</p>
          <h1>Make plans<br /><em>worth leaving home for.</em></h1>
          <p className="hero-intro">Find the music, ideas, food, and little moments that make a place feel alive.</p>
          <div className="search-box">
            <Search size={19} />
            <input value={query} onChange={(event) => setQuery(event.target.value)} placeholder="Search events, venues, or cities" />
            <button aria-label="Search"><ArrowRight size={18} /></button>
          </div>
        </div>
        <div className="hero-art" aria-label="Abstract event collage" role="img"><div className="art-note">LIVE<br />LOCAL<br />NOW</div><div className="art-stamp">AUSTIN<br /><small>TX</small></div></div>
      </section>

      <section className="discover shell" id="discover">
        <div className="section-heading"><div><p className="eyebrow">The good stuff</p><h2>Events near you</h2></div><a className="text-link" href="#discover">View all <ArrowRight size={16} /></a></div>
        <div className="event-grid">
          {visibleEvents.map((event) => <EventCard event={event} key={event.id} />)}
        </div>
        {!visibleEvents.length && <p className="empty-state">No events found. Try a different search.</p>}
      </section>

      <section className="organizer-band" id="for-organizers"><div className="shell organizer-content"><div><p className="eyebrow">Have something to share?</p><h2>Put your event<br />on the map.</h2></div><button className="dark-button">List an event <ArrowRight size={17} /></button></div></section>
      <footer className="footer shell"><span className="wordmark">WhereTo<span>.</span></span><span>Built for finding your next favorite thing.</span></footer>
    </main>
  )
}

function EventCard({ event }) {
  return <article className="event-card"><div className={`event-image ${event.accent}`}><span>{event.category}</span><strong>{event.title.split(' ').slice(0, 2).join(' ')}<br />{event.title.split(' ').slice(2).join(' ')}</strong></div><div className="event-details"><h3>{event.title}</h3><p className="event-meta"><CalendarDays size={15} /> {new Date(event.date).toLocaleDateString('en-US', { month: 'short', day: 'numeric' })}<span>•</span><MapPin size={15} /> {event.city}</p><div className="event-footer"><span>From <b>${event.price}</b></span><button aria-label={`Get tickets for ${event.title}`}><ArrowRight size={17} /></button></div></div></article>
}

export default App