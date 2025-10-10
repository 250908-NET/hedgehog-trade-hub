// src/components/AuthFormInput.jsx
export default function AuthFormInput({ label, type, value, onChange }) {
  return (
    <div className="mb-4">
      <label className="block text-gray-300 mb-1">{label}</label>
      <input
        type={type}
        value={value}
        onChange={onChange}
        className="w-full p-2 rounded-md bg-gray-800 border border-gray-700 text-gray-100 focus:outline-none focus:ring-2 focus:ring-amber-500"
      />
    </div>
  );
}
