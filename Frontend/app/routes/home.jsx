import Tile from '../components/Tile';

export default function HomePage() {
  return (
    <div className="home-page-container">
      <header className="home-header">
        <div className="logo-section">
          {/* Replace the emoji with an actual image when available */}
          {/* <img src={hedgehogImage} alt="Hedgehog" className="hedgehog-logo" /> */}
          <span className="hedgehog-logo-placeholder" role="img" aria-label="Hedgehog">🦔</span>
          <h1>TradeHub</h1>
        </div>
        <div className="welcome-message">
          Welcome back, User
        </div>
      </header>

      <div className="search-bar-container">
        <input type="text" placeholder="Search for items..." className="search-input" />
      </div>

      <main className="tiles-grid">
        {/* Render 8 placeholder tiles as seen in the image */}
        {Array.from({ length: 8 }).map((_, index) => (
          <Tile key={index} id={index + 1} />
        ))}
      </main>
    </div>
  );
}
