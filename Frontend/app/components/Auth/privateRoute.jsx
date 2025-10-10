// src/components/Auth/PrivateRoute.jsx
import { Navigate, Outlet } from "react-router-dom";
import { useState, useEffect } from "react";

export default function PrivateRoute() {
  const [token, setToken] = useState(null);
  const [checked, setChecked] = useState(false);

  useEffect(() => {
    const t = localStorage.getItem("token");
    console.log(t);
    setToken(t);
    setChecked(true);
  }, []);

  if (!checked) return null;

  return token ? <Outlet /> : <Navigate to="/login" replace />;
}
