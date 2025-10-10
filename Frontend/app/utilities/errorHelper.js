export async function handleResponseError(response) {
  let errorMessage = `${response.status} ${response.statusText}`;

  try {
    // Attempt to parse the response body as JSON
    const errorData = await response.json();
    if (errorData && errorData.message) {
      errorMessage += `: ${errorData.message}`;
    } else if (errorData && errorData.errors) {
      // For validation errors, often an 'errors' object is returned
      errorMessage += `: ${JSON.stringify(errorData.errors)}`;
    } else {
      // If JSON is present but no specific message, stringify the whole object
      errorMessage += `: ${JSON.stringify(errorData)}`;
    }
  } catch (jsonError) {
    // Log the JSON parsing error for debugging
    console.warn("Failed to parse response body as JSON:", jsonError);
    // If parsing as JSON fails, try to get the raw text
    try {
      const textError = await response.text();
      if (textError) {
        errorMessage += `: ${textError}`;
      }
    } catch (textReadError) {
      // Fallback if even reading as text fails
      console.warn("Failed to read response body as text:", textReadError);
      errorMessage += `: Could not read response body.`;
    }
  }
  return new Error(errorMessage);
}
