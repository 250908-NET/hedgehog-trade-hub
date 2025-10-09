import React from "react";

interface Props {
  offers: string[];
}

const OfferList: React.FC<Props> = ({ offers }) => {
  return (
    <div>
      <h2>Offers</h2>
      <ul>
        {offers.length === 0 ? <li>No offers yet</li> : offers.map((offer, i) => <li key={i}>{offer}</li>)}
      </ul>
    </div>
  );
};

export default OfferList;
