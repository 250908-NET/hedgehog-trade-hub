// src/pages/RegisterPage.jsx
import { useState } from "react";
import { useNavigate } from "react-router";
import AuthFormInput from "../../components/Auth/AuthFormInput";

export default function RegisterPage() {
  const [username, setUsername] = useState("");
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [description, setDescription] = useState("");
  const [error, setError] = useState("");
  const [message, setMessage] = useState("");
  const navigate = useNavigate();

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError("");
    setMessage("");

    try {
      const res = await fetch("/api/auth/register/user", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ username, email, password, description }),
      });

      if (!res.ok) {
        const err = await res.json().catch(() => ({ message: res.statusText }));
        throw new Error(
          err[0]?.description || err.message || "Registration failed"
        );
      }

      setMessage("Registration successful! Redirecting...");
      setTimeout(() => navigate("/login"), 2000);
    } catch (err) {
      setError(err.message);
    }
  };

  return (
    <div className="min-h-screen flex items-center justify-center bg-[#121212] text-white">
      <div className="bg-[#1b1b1b] p-8 rounded-2xl shadow-lg w-96 border border-gray-800">
        <h2 className="text-3xl font-bold mb-6 text-center text-amber-400">
          Create Account
        </h2>

        {error && <p className="text-red-400 mb-4 text-sm">{error}</p>}
        {message && <p className="text-green-400 mb-4 text-sm">{message}</p>}

        <form onSubmit={handleSubmit}>
          <AuthFormInput
            label="Username"
            type="text"
            value={username}
            onChange={(e) => setUsername(e.target.value)}
          />
          <AuthFormInput
            label="Email"
            type="email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
          />
          <AuthFormInput
            label="Password"
            type="password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
          />
          <AuthFormInput
            label="Description"
            type="text"
            value={description}
            onChange={(e) => setDescription(e.target.value)}
          />

          <button
            type="submit"
            className="w-full bg-amber-500 hover:bg-amber-600 text-black font-semibold py-2 rounded-md transition duration-200"
          >
            Register
          </button>
        </form>

        <p className="mt-4 text-center text-gray-400 text-sm">
          Already have an account?{" "}
          <span
            onClick={() => navigate("/login")}
            className="text-amber-400 cursor-pointer hover:underline"
          >
            Login
          </span>
        </p>
      </div>
    </div>
  );
}
