import React from "react";

interface Props {
  onConfirm: () => void;
}

const ConfirmCompletion: React.FC<Props> = ({ onConfirm }) => {
  return (
    <div>
      <h2>Confirm Trade Completion</h2>
      <button onClick={onConfirm}>Mark All Completed</button>
    </div>
  );
};

export default ConfirmCompletion;
