// Expected item data format:
// {
//   "id": 1,
//   "name": "Item 1",
//   "description": "Description about Item 1",
//   "imgURL": "",
//   "value": 10,
//   "userId": 1,
//   "tags": "",
//   "condition": "New",
//   "availability": "Available"
// }

export async function searchItems(query) {
  try {
    // Construct the URL with the 'search' query parameter
    const response = await fetch(`/api/items?search=${encodeURIComponent(query)}`);
    if (!response.ok) {
      throw new Error(`${response.status} ${response.statusText}`);
    }
    const data = await response.json();
    console.log(data);
    return data;
  } catch (error) {
    console.error("Error searching items:", error);
    throw error;
  }
}
