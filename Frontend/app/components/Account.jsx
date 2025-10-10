import { useState } from "react";

export default function Account() {
  const [formData, setFormData] = useState({
    name: "John Doe",
    email: "john.doe@example.com",
    aboutMe: "",
  });

  const [isEditing, setIsEditing] = useState(false);

  const handleInputChange = (e) => {
    const { name, value } = e.target;
    setFormData((prev) => ({
      ...prev,
      [name]: value,
    }));
  };

  const handleSubmit = (e) => {
    e.preventDefault();
    // TODO: Implement API call to update profile
    console.log("Profile updated:", formData);
    setIsEditing(false);
  };

  const handleCancel = () => {
    // Reset form data to original values if needed
    setIsEditing(false);
  };

  return (
    <div className="container mx-auto p-6">
      <div className="bg-white rounded-lg shadow-md p-8 max-w-2xl mx-auto">
        <h1 className="text-3xl font-bold mb-6">My Account</h1>

        <form onSubmit={handleSubmit} className="space-y-6">
          <div className="border-b pb-6">
            <h2 className="text-xl font-semibold mb-4">Profile Information</h2>
            <div className="grid grid-cols-1 gap-4">
              <div>
                <label htmlFor="name" className="block text-gray-600 mb-1">
                  Name
                </label>
                {isEditing ? (
                  <input
                    type="text"
                    id="name"
                    name="name"
                    value={formData.name}
                    onChange={handleInputChange}
                    className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                    required
                  />
                ) : (
                  <p className="font-medium py-2">{formData.name}</p>
                )}
              </div>
              <div>
                <label htmlFor="email" className="block text-gray-600 mb-1">
                  Email
                </label>
                {isEditing ? (
                  <input
                    type="email"
                    id="email"
                    name="email"
                    value={formData.email}
                    onChange={handleInputChange}
                    className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                    required
                  />
                ) : (
                  <p className="font-medium py-2">{formData.email}</p>
                )}
              </div>
              <div>
                <label htmlFor="aboutMe" className="block text-gray-600 mb-1">
                  About Me
                </label>
                {isEditing ? (
                  <textarea
                    id="aboutMe"
                    name="aboutMe"
                    value={formData.aboutMe}
                    onChange={handleInputChange}
                    rows={4}
                    className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 resize-vertical"
                    placeholder="Tell us about yourself..."
                  />
                ) : (
                  <p className="font-medium py-2 min-h-[2rem]">
                    {formData.aboutMe || "No description provided"}
                  </p>
                )}
              </div>
            </div>
          </div>

          <div className="pt-4">
            <h2 className="text-xl font-semibold mb-4">Account Settings</h2>
            <div className="flex gap-3">
              {isEditing ? (
                <>
                  <button
                    type="submit"
                    className="bg-green-500 text-white px-4 py-2 rounded hover:bg-green-600 transition-colors"
                  >
                    Save Changes
                  </button>
                  <button
                    type="button"
                    onClick={handleCancel}
                    className="bg-gray-500 text-white px-4 py-2 rounded hover:bg-gray-600 transition-colors"
                  >
                    Cancel
                  </button>
                </>
              ) : (
                <button
                  type="button"
                  onClick={() => setIsEditing(true)}
                  className="bg-blue-500 text-white px-4 py-2 rounded hover:bg-blue-600 transition-colors"
                >
                  Edit Profile
                </button>
              )}
            </div>
          </div>
        </form>
      </div>
    </div>
  );
}
