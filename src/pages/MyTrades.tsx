import React, { useState } from "react";
import { Trade } from "../types";
import TradeList from "../components/TradeList";
import "./MyTrades.css";

interface Props {
  trades: Trade[];
}

const MyTrades: React.FC<Props> = ({ trades }) => {
  const [selectedTrade, setSelectedTrade] = useState<Trade | null>(null);

  return (
    <div className="my-trades-container">
      <h1>My Trades</h1>
      <TradeList trades={trades} onSelect={setSelectedTrade} selectedTrade={selectedTrade} />

      {selectedTrade && (
        <div className="selected-trade">
          <h2>Selected Trade Details</h2>
          <p>Item: {selectedTrade.item}</p>
          <p>Status: {selectedTrade.status}</p>
          <div className="trade-actions">
            <button className="action-btn">Create Offer</button>
            <button className="action-btn">Confirm Completion</button>
          </div>
        </div>
      )}
    </div>
  );
};

export default MyTrades;
