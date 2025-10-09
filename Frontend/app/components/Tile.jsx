import "./Tile.css";

// represents a base "tile" for the search page
// should be overloaded with "item" and "trade" components
export default function Tile() {
  return (
    <div className="tile">
      <div className="tile-image-placeholder"></div>
      <div className="tile-text-placeholder"></div>
    </div>
  );
}
