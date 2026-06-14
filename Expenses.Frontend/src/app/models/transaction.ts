export interface Transaction {
  id: string;
  type: string;
  amount: number;
  category: string;
  creatorId: string | null;
  createdAt: Date;
  updatedAt: Date;
}
