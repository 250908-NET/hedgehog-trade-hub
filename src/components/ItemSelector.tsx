import React from "react";

interface Props {
  items: string[];
  onSelect: (item: string) => void;
}

const ItemSelector: React.FC<Props> = ({ items, onSelect }) => {
  return (
    <div>
      <h2>Select Item</h2>
      {items.map((item, i) => (
        <button key={i} onClick={() => onSelect(item)} style={{ margin: "5px" }}>
          {item}
        </button>
      ))}
    </div>
  );
};

export default ItemSelector;
