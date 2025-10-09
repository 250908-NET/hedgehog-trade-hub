import React, { useState } from "react";
import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import Home from "./pages/Home";
import MyTrades from "./pages/MyTrades";
import ReceivedOffers from "./pages/ReceivedOffers";
import { Trade } from "./types";

const App: React.FC = () => {

  const [trades, setTrades] = useState<Trade[]>([
  { id: 1, item: "TV", status: "Pending" },
  { id: 2, item: "Mobile", status: "Accepted" },
  { id: 3, item: "Car", status: "Completed" },
]);


  return (
    <Router>
      <Routes>
        <Route path="/" element={<Home trades={trades} setTrades={setTrades} />} />
        <Route path="/my-trades" element={<MyTrades trades={trades} />} />
        <Route path="/received-offers" element={<ReceivedOffers />} />
      </Routes>
    </Router>
  );
};

export default App;
