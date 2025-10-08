import { NavLink } from "react-router";

import "./Navbar.css";

export default function Navbar() {
  return (
    <div className="nav-container">
      <nav className="nav-bar">
        <NavLink to="/">Home</NavLink>
        <NavLink to="/search">Search</NavLink>
        <NavLink to="/login">Login</NavLink>
      </nav>
    </div>
  );
}
