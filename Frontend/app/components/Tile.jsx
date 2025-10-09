// represents a base "tile" for the search page
// should be overloaded with "item" and "trade" components
export default function Tile({ children, className = "" }) {
  return (
    <div className={`bg-neutral-800 rounded p-4 ${className}`}>
      {children}
    </div>
  );
}
