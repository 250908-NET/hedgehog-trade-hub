import LogoTile from "../components/LogoTile";
import ItemTile from "../components/ItemTile";

export default function HomePage() {
  return (
    <>
      <LogoTile>
        <input
          type="text"
          placeholder="Search for items..."
          className="bg-neutral-700 p-2 rounded w-full focus:outline-none focus:ring-2 focus:ring-neutral-600"
        />
      </LogoTile>

      <div className="container mx-auto grid grid-cols-[repeat(auto-fit,minmax(240px,1fr))] gap-4 py-8">
        {/* render 8 placeholder tiles */}
        {Array.from({ length: 8 }).map((_, index) => (
          <ItemTile
            key={index}
            id={index + 1}
            item={{ name: `Item ${index + 1}`, price: `$${index + 1}` }}
          />
        ))}
      </div>
    </>
  );
}
