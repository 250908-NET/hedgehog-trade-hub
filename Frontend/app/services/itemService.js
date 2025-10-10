import { buildQueryString } from "@/utilities/queryHelper";
import { handleResponseError } from "@/utilities/errorHelper";

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

// WHY CAN'T I USE TYPESCRIPT AAAAAAAAAAAAAAAA
export async function searchItems(params = {}) {
  // assemble query
  const rawParams = {
    page: params.page || 1,
    pageSize: params.pageSize || 10,
    minValue: params.minValue,
    maxValue: params.maxValue,
    condition: params.condition,
    availability: params.availability,
    search: params.search,
  };
  const queryString = buildQueryString(rawParams);

  try {
    const response = await fetch(`/api/items${queryString}`);

    // try to get error message
    if (!response.ok) {
      throw await handleResponseError(response);
    }

    const data = await response.json();
    // console.log(data);
    return data;
  } catch (error) {
    console.error("Error searching items:", error);
    throw error;
  }
}
