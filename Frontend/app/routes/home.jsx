import ItemTile from "../components/ItemTile";
import hedgehog from "../assets/hedgehog.png";

export default function HomePage() {
  return (
    <>
      <div className="container mx-auto bg-neutral-800 rounded mt-16">
        <div className="container flex flex-col items-center sm:flex-row sm:items-end gap-8 p-4">
          <img
            src={hedgehog}
            alt="Hedgehog"
            className="hedgehog-logo"
            width="256"
          />
          <h1 className="text-6xl font-bold">TradeHub</h1>
        </div>

        <div className="container flex p-4">
          <input
            type="text"
            placeholder="Search for items..."
            className="bg-neutral-700 p-2 rounded w-full focus:outline-none focus:ring-2 focus:ring-neutral-600"
          />
        </div>
      </div>

      <div className="container mx-auto grid grid-cols-2 md:grid-cols-4 gap-4 py-8">
        {/* render 8 placeholder tiles */}
        {Array.from({ length: 8 }).map((_, index) => (
          <ItemTile key={index} id={index + 1} item={{ name: `Item ${index + 1}`, price: `$${index + 1}` }} />
        ))}
      </div>
    </>
  );
}
