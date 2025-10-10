import { NavLink, useNavigate } from "react-router";

export default function Navbar() {
  const getNavLinkClass = ({ isActive }) => {
    return `hover:underline ${isActive ? "font-bold" : ""}`;
  };

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
        <button
          onClick={handleLogout}
          className="ml-auto text-white hover:underline font-semibold"
        >
          Logout
        </button>
      </nav>
    </div>
  );
}
