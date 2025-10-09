import React, { useState } from "react";
import { Trade } from "../types";
import TradeList from "../components/TradeList";
import TradeDetails from "../components/TradeDetails";
import OfferForm from "../components/OfferForm";
import OfferList from "../components/OfferList";
import ConfirmCompletion from "../components/ConfirmCompletion";
import ItemSelector from "../components/ItemSelector";

interface Props {
  trades: Trade[];
  setTrades: React.Dispatch<React.SetStateAction<Trade[]>>;
}

const Home: React.FC<Props> = ({ trades, setTrades }) => {
  const [selectedTrade, setSelectedTrade] = useState<Trade | null>(null);
  const [offers, setOffers] = useState<string[]>([]);
  const items = trades.map(t => t.item);

  const addOffer = (desc: string) => setOffers(prev => [...prev, desc]);
  const confirmTrades = () => setTrades(prev => prev.map(t => ({ ...t, status: "Completed" })));
  const selectItem = (item: string) => setSelectedTrade(trades.find(t => t.item === item) || null);

  return (
    <div>
      <ItemSelector items={items} onSelect={selectItem} />
      <TradeList trades={trades} onSelect={setSelectedTrade} />
      <TradeDetails trade={selectedTrade} />
      <OfferForm onAddOffer={addOffer} />
      <OfferList offers={offers} />
      <ConfirmCompletion onConfirm={confirmTrades} />
    </div>
  );
};

export default Home;
