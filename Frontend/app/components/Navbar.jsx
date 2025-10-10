import { NavLink } from "react-router";
import { useEffect, useState } from "react";

export default function Navbar() {
  const [isLoggedIn, setIsLoggedIn] = useState(false);
  const [username, setUsername] = useState(""); // Add state for username

  const getNavLinkClass = ({ isActive }) => {
    return `hover:underline ${isActive ? "font-bold" : ""}`;
  };

  useEffect(() => {
    const token = localStorage.getItem("token");
    const storedUsername = localStorage.getItem("username"); // Get username from localStorage
    setIsLoggedIn(!!token);
    setUsername(storedUsername || ""); // Set username state

    // Optional: listen to token changes from other tabs
    const handleStorageChange = () => {
      const newToken = localStorage.getItem("token");
      const newUsername = localStorage.getItem("username");
      setIsLoggedIn(!!newToken);
      setUsername(newUsername || "");
    };
    window.addEventListener("storage", handleStorageChange);

    return () => window.removeEventListener("storage", handleStorageChange);
  }, []);

  const handleLogout = () => {
    localStorage.removeItem("token");
    localStorage.removeItem("username");
    localStorage.removeItem("userid");
    window.location.href = "/login";
  };

  return (
    <div className="bg-neutral-700 fixed top-0 left-0 w-full z-20">
      <nav className="container mx-auto flex items-center gap-8 p-4">
        <NavLink className={getNavLinkClass} to="/">
          Home
        </NavLink>
        <NavLink className={getNavLinkClass} to="/trade">
          Trades
        </NavLink>
        <div className="ml-auto flex items-center gap-4">
          {isLoggedIn ? (
            <>
              <NavLink className={getNavLinkClass} to="/items">
                Items
              </NavLink>
              <span>Welcome, {username}!</span> {/* Display username */}
              <button
                onClick={handleLogout}
                className="hover:underline font-semibold"
              >
                Logout
              </button>
            </>
          ) : (
            <>
              <NavLink className={getNavLinkClass} to="/login">
                Login
              </NavLink>
              <NavLink className={getNavLinkClass} to="/register">
                Register
              </NavLink>
            </>
          )}
        </div>
      </nav>
    </div>
  );
}
