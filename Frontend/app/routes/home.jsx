import { useState } from "react";

import Tile from "../components/Tile";
import LogoTile from "../components/LogoTile";
import ItemTile from "../components/ItemTile";

import { searchItems } from "@/services/itemService";

export default function HomePage() {
  const [searchQuery, setSearchQuery] = useState("");
  const [items, setItems] = useState([]); // store fetched items
  const [message, setMessage] = useState("Search for an item!");

  const handleSearch = async (e) => {
    e.preventDefault(); // prevent reload on form submission
    try {
      const results = await searchItems(searchQuery);
      if (results.length === 0) {
        setMessage("No items found.");
      }
      setItems(results);
    } catch (error) {
      console.error("Error searching items:", error);
      setMessage("An error occurred while searching for items: " + error.message);
    }
  };

  return (
    <>
      <LogoTile>
        <form onSubmit={handleSearch} className="flex flex-row items-center gap-2 w-full">
          <input
            type="text"
            placeholder="Search for items..."
            id="itemSearchInput"
            className="bg-neutral-700 p-2 rounded w-full focus:outline-none focus:ring-2 focus:ring-neutral-600"
            value={searchQuery}
            onChange={(e) => setSearchQuery(e.target.value)}
          />
          <button
            id="itemSearchButton"
            className="bg-neutral-600 p-2 rounded hover:bg-neutral-500"
          >
            {/* magnifying glass icon */}
            <svg
              xmlns="http://www.w3.org/2000/svg"
              className="h-6 w-6"
              fill="none"
              viewBox="0 0 24 24"
              stroke="currentColor"
              strokeWidth={2}
            >
              <path
                strokeLinecap="round"
                strokeLinejoin="round"
                d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z"
              />
            </svg>
          </button>
        </form>
      </LogoTile>

      <div className="container mx-auto grid grid-cols-[repeat(auto-fit,240px))] justify-center gap-4 py-8">
        {/* render item tiles or error message if no items */}
        {
          items.length > 0
            ? items.map((item) => <ItemTile key={item.id} orderId={item.value} item={item} />)
            : <Tile><p className="text-center">{message}</p></Tile>
        }
      </div>
    </>
  );
}
