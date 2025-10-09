export interface Trade {
  id: number;
  item: string;
  status: "Pending" | "Accepted" | "Rejected" | "Completed";
  notes?: string;
}

export interface Offer {
  id: number;
  tradeId: number;
  description: string;
}
