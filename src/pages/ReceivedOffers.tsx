import React from "react";

const ReceivedOffers: React.FC = () => {
  // For now, we can use dummy offers
  const offers = ["Offer 1", "Offer 2", "Offer 3"];

  return (
    <div>
      <h1>Received Offers</h1>
      <ul>
        {offers.map((offer, index) => (
          <li key={index}>{offer}</li>
        ))}
      </ul>
    </div>
  );
};

export default ReceivedOffers;
