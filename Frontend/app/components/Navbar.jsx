import { NavLink } from "react-router";

import "./Navbar.css";

export default function Navbar() {
  return (
    <div className="nav-container">
      <nav className="nav-bar">
        <NavLink to="/">
          Hedgehog Trade Hub
        </NavLink>
        <NavLink to="/search">
          Search
        </NavLink>
      </nav>
    </div>
  );
}
