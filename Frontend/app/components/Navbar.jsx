import { NavLink, useNavigate } from "react-router";
import { useEffect, useState } from "react";

export default function Navbar() {
  const [isLoggedIn, setIsLoggedIn] = useState(false);

  const getNavLinkClass = ({ isActive }) => {
    return `hover:underline ${isActive ? "font-bold" : ""}`;
  };

  useEffect(() => {
    const token = localStorage.getItem("token");
    setIsLoggedIn(!!token);

    // Optional: listen to token changes from other tabs
    const handleStorageChange = () => {
      setIsLoggedIn(!!localStorage.getItem("token"));
    };
    window.addEventListener("storage", handleStorageChange);

    return () => window.removeEventListener("storage", handleStorageChange);
  }, []);

  const handleLogout = () => {
    localStorage.removeItem("token");
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
            <button
              onClick={handleLogout}
              className="text-white hover:underline font-semibold"
            >
              Logout
            </button>
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
