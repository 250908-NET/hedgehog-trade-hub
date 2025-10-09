import { NavLink } from "react-router";

export default function Navbar() {
  const getNavLinkClass = ({ isActive }) => {
    return `hover:underline ${isActive ? "font-bold" : ""}`;
  };

  return (
    <div className="bg-neutral-700 fixed top-0 left-0 w-full">
      <nav className="container mx-auto flex items-center gap-8 p-4">
        <NavLink className={getNavLinkClass} to="/">
          Home
        </NavLink>
        <NavLink className={getNavLinkClass} to="/search">
          Search
        </NavLink>
        <NavLink className={getNavLinkClass + " ml-auto"} to="/login">
          Login
        </NavLink>
      </nav>
    </div>
  );
}
