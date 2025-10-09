import Tile from '../components/Tile';
import hedgehog from '../assets/hedgehog.png';

export default function HomePage() {
  return (
    <div className="home-page-container">
      <header className="home-header">
        <div className="logo-section">
          <img src={hedgehog} alt="Hedgehog" className="hedgehog-logo" />
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
