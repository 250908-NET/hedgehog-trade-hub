import React, { useState } from "react";

interface Props {
  onAddOffer: (description: string) => void;
}

const OfferForm: React.FC<Props> = ({ onAddOffer }) => {
  const [offer, setOffer] = useState("");

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    if (!offer) return;
    onAddOffer(offer);
    setOffer("");
  };

  return (
    <div>
      <h2>Create Offer</h2>
      <form onSubmit={handleSubmit}>
        <input 
          type="text" 
          value={offer} 
          onChange={(e) => setOffer(e.target.value)} 
          placeholder="Offer description" 
        />
        <button type="submit">Add Offer</button>
      </form>
    </div>
  );
};

export default OfferForm;
