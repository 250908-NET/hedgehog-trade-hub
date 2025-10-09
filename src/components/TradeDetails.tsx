import React from "react";
import { Trade } from "../types";

interface Props {
  trade: Trade | null;
}

const TradeDetails: React.FC<Props> = ({ trade }) => {
  if (!trade) return <p>Select a trade to see details</p>;

  return (
    <div>
      <h2>Trade Details</h2>
      <p><strong>Item:</strong> {trade.item}</p>
      <p><strong>Status:</strong> {trade.status}</p>
      <p><strong>Notes:</strong> {trade.notes || "N/A"}</p>
    </div>
  );
};

export default TradeDetails;
