import React from "react";
import { Trade } from "../types";

interface Props {
  trades: Trade[];
  onSelect: (trade: Trade) => void;
  selectedTrade?: Trade | null;
}

const TradeList: React.FC<Props> = ({ trades, onSelect, selectedTrade }) => {
  return (
    <div className="trade-list">
      <h2>Trades</h2>
      <ul>
        {trades.map(trade => (
          <li
            key={trade.id}
            onClick={() => onSelect(trade)}
            className={selectedTrade?.id === trade.id ? "selected" : ""}
          >
            {trade.item} - {trade.status}
          </li>
        ))}
      </ul>
    </div>
  );
};

export default TradeList;
