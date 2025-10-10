import { useState } from "react";

import Tile from "../components/Tile";
import LogoTile from "../components/LogoTile";
import ItemTile from "../components/ItemTile";

import { searchItems } from "@/services/itemService";

export default function HomePage() {
  // search parameters
  const [minValue, setMinValue] = useState("");
  const [maxValue, setMaxValue] = useState("");
  const [condition, setCondition] = useState("");
  const [availability, setAvailability] = useState("Available");
  const [searchQuery, setSearchQuery] = useState("");

  const [items, setItems] = useState([]); // store fetched items
  const [message, setMessage] = useState("Search for an item!");
  const [showAdvancedSearch, setShowAdvancedSearch] = useState(false);

  const handleSearch = async (e) => {
    e.preventDefault(); // prevent reload on form submission
    try {
      setMessage("Searching for items...");
      setItems([]);
      const results = await searchItems({
        search: searchQuery,
        minValue: minValue,
        maxValue: maxValue,
        condition: condition,
        availability: availability,
      });
      if (results.length === 0) {
        setMessage("No items found.");
      }
      setItems(results);
      setMessage("Search for an item!");
      // console.log(results);
    } catch (error) {
      console.error("Error searching items:", error);
      setMessage(
        "An error occurred while searching for items: " + error.message,
      );
      setItems([]);
    }
  };

  return (
    <>
      <LogoTile>
        <form
          onSubmit={handleSearch}
          className="flex flex-col items-center gap-4 w-full"
        >
          <div className="flex flex-row items-center gap-2 w-full">
            <input
              type="text"
              placeholder="Search for items..."
              id="itemSearchInput"
              value={searchQuery}
              onChange={(e) => setSearchQuery(e.target.value)}
            />
            <button id="itemSearchButton" type="submit">
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
            <button
              id="advancedSearchToggleButton"
              type="button"
              onClick={() => setShowAdvancedSearch(!showAdvancedSearch)}
            >
              {/* filter icon */}
              <svg
                xmlns="http://www.w3.org/2000/svg"
                className="h-6 w-6"
                fill={showAdvancedSearch ? "currentColor" : "none"}
                viewBox="0 0 24 24"
                stroke="currentColor"
                strokeWidth={2}
              >
                <path
                  strokeLinecap="round"
                  strokeLinejoin="round"
                  d="M3 4a1 1 0 011-1h16a1 1 0 011 1v2.586a1 1 0 01-.293.707l-6.414 6.414a1 1 0 00-.293.707V17l-4 4v-6.586a1 1 0 00-.293-.707L3.293 7.293A1 1 0 013 6.586V4z"
                />
              </svg>
            </button>
          </div>
          {/* conditional rendering for advanced search */}
          {showAdvancedSearch && (
            <div
              id="advancedSearchContainer"
              className="grid grid-cols-2 sm:grid-cols-4 gap-2 w-full px-4"
            >
              <input
                type="number"
                placeholder="Min Estimated Value"
                value={minValue}
                onChange={(e) => setMinValue(e.target.value)}
              />
              <input
                type="number"
                placeholder="Max Estimated Value"
                value={maxValue}
                onChange={(e) => setMaxValue(e.target.value)}
              />
              <select
                value={condition}
                onChange={(e) => setCondition(e.target.value)}
              >
                <option value="">
                  Any Condition
                </option>
                <option value="New">New</option>
                <option value="Refurbished">Refurbished</option>
                <option value="UsedLikeNew">Used (Like New)</option>
                <option value="UsedGood">Used (Good)</option>
                <option value="UsedAcceptable">Used (Acceptable)</option>
                <option value="UsedBad">Used (Bad)</option>
              </select>
              <select
                value={availability}
                onChange={(e) => setAvailability(e.target.value)}
              >
                <option value="">Any Availability</option>
                <option value="Available">
                  Available
                </option>
                <option value="Unavailable">Unavailable</option>
              </select>
            </div>
          )}
        </form>
      </LogoTile>

      <div className="container mx-auto grid grid-cols-[repeat(auto-fit,240px))] justify-center gap-4 py-8">
        {/* render item tiles or error message if no items */}
        {items.length > 0 ? (
          items.map((item) => (
            <ItemTile key={item.id} orderId={item.value} item={item} />
          ))
        ) : (
          <Tile className="w-full col-span-full">
            <p className="text-center">{message}</p>
          </Tile>
        )}
      </div>
    </>
  );
}
