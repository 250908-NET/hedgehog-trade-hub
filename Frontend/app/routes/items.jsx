import { useState, useEffect } from "react";
import { useNavigate } from "react-router";
import AuthFormInput from "../components/Auth/AuthFormInput"; // Adjust path if necessary

export default function AddItemPage() {
  const navigate = useNavigate();
  const [ownerId, setOwnerId] = useState(null);
  const [name, setName] = useState("");
  const [description, setDescription] = useState("");
  const [imageUrl, setImageUrl] = useState("");
  const [value, setValue] = useState("");
  const [tags, setTags] = useState("");
  const [condition, setCondition] = useState("New"); // Default condition
  const [availability, setAvailability] = useState("Available"); // Default availability
  const [estimateValue, setEstimateValue] = useState(false);

  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState("");
  const [successMessage, setSuccessMessage] = useState("");

  // Fetch ownerId from localStorage on component mount
  useEffect(() => {
    const storedUserId = localStorage.getItem("userId");
    if (!storedUserId) {
      // If no userId, user is not logged in, redirect to login
      navigate("/login", { replace: true });
    } else {
      setOwnerId(parseInt(storedUserId, 10));
    }
  }, [navigate]);

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError("");
    setSuccessMessage("");
    setIsLoading(true);

    if (!ownerId) {
      setError("User not logged in. Please log in again.");
      setIsLoading(false);
      return;
    }

    try {
      const newItem = {
        name,
        description,
        image: imageUrl, // Backend expects 'image', frontend uses 'imageUrl'
        value: parseFloat(value),
        ownerId,
        tags,
        condition,
        availability,
        estimateValue,
      };

      const token = localStorage.getItem("token");
      if (!token) {
        throw new Error("Authentication token not found. Please log in.");
      }

      const response = await fetch("/api/items", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${token}`,
        },
        body: JSON.stringify(newItem),
      });

      if (!response.ok) {
        const errorData = await response.json();
        throw new Error(errorData.message || "Failed to add item.");
      }

      const result = await response.json();
      setSuccessMessage("Item added successfully!");
      // Optionally clear form or redirect
      setName("");
      setDescription("");
      setImageUrl("");
      setValue("");
      setTags("");
      setCondition("New");
      setAvailability("Available");
      setEstimateValue(false);
      console.log("Item created:", result);
    } catch (err) {
      console.error("Error adding item:", err);
      setError(err.message || "An unexpected error occurred.");
    } finally {
      setIsLoading(false);
    }
  };

  if (!ownerId) {
    return null; // Or a loading spinner, while redirecting
  }

  return (
    <div className="min-h-screen flex items-center justify-center bg-[#121212] text-white">
      <div className="bg-[#1b1b1b] p-8 rounded-2xl shadow-lg w-full max-w-md border border-gray-800">
        <h2 className="text-3xl font-bold mb-6 text-center text-amber-400">
          Add New Item
        </h2>
        {error && <p className="text-red-400 mb-4 text-sm">{error}</p>}
        {successMessage && (
          <p className="text-green-400 mb-4 text-sm">{successMessage}</p>
        )}

        <form onSubmit={handleSubmit} className="space-y-4">
          <AuthFormInput
            label="Item Name"
            type="text"
            value={name}
            onChange={(e) => setName(e.target.value)}
            required
          />
          <AuthFormInput
            label="Description"
            type="textarea"
            value={description}
            onChange={(e) => setDescription(e.target.value)}
            required
          />
          <AuthFormInput
            label="Image URL"
            type="url"
            value={imageUrl}
            onChange={(e) => setImageUrl(e.target.value)}
            // removed 'required'
          />
          <AuthFormInput
            label="Estimated Value"
            type="number"
            value={value}
            onChange={(e) => setValue(e.target.value)}
            step="0.01"
            min="0"
            // removed 'required'
          />
          <AuthFormInput
            label="Tags (comma-separated)"
            type="text"
            value={tags}
            onChange={(e) => setTags(e.target.value)}
            // removed 'required'
          />

          <div>
            <label className="block text-gray-300 mb-1">Condition</label>
            <select
              value={condition}
              onChange={(e) => setCondition(e.target.value)}
              className="w-full p-2 rounded-md bg-gray-800 border border-gray-700 text-gray-100 focus:outline-none focus:ring-2 focus:ring-amber-500"
              required
            >
              <option value="New">New</option>
              <option value="Refurbished">Refurbished</option>
              <option value="UsedLikeNew">Used (Like New)</option>
              <option value="UsedGood">Used (Good)</option>
              <option value="UsedAcceptable">Used (Acceptable)</option>
              <option value="UsedBad">Used (Bad)</option>
            </select>
          </div>

          <div>
            <label className="block text-gray-300 mb-1">Availability</label>
            <select
              value={availability}
              onChange={(e) => setAvailability(e.target.value)}
              className="w-full p-2 rounded-md bg-gray-800 border border-gray-700 text-gray-100 focus:outline-none focus:ring-2 focus:ring-amber-500"
              required
            >
              <option value="Available">Available</option>
              <option value="Unavailable">Unavailable</option>
            </select>
          </div>

          <div className="flex items-center">
            <input
              type="checkbox"
              id="estimateValue"
              checked={estimateValue}
              onChange={(e) => setEstimateValue(e.target.checked)}
              className="h-4 w-4 text-amber-500 focus:ring-amber-500 border-gray-700 rounded"
            />
            <label htmlFor="estimateValue" className="ml-2 block text-gray-300">
              Estimate Value (if checked, LLM will estimate value)
            </label>
          </div>

          <button
            type="submit"
            className="w-full bg-amber-500 hover:bg-amber-600 text-black font-semibold py-2 rounded-md transition duration-200"
            disabled={isLoading}
          >
            {isLoading ? "Adding Item..." : "Add Item"}
          </button>
        </form>
      </div>
    </div>
  );
}
