export interface Transaction {
  id: number;
  type: string;
  amount: number;
  category: string;
  createdAt: Date;
  updateAt: Date;
}
