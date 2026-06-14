/**
 * Represents a financial transaction with related metadata.
 *
 * @interface Transaction
 * @property {string} id - Unique identifier for the transaction.
 * @property {string} type - Type of the transaction (e.g., income, expense).
 * @property {number} amount - Monetary value of the transaction.
 * @property {string} category - Category under which the transaction falls.
 * @property {string | null} creatorId - Identifier for the user or entity that created the transaction. Can be null if no creator is associated.
 * @property {Date} createdAt - Timestamp indicating when the transaction was created.
 * @property {Date} updatedAt - Timestamp indicating when the transaction was last updated.
 */
export interface Transaction {
  id: string;
  type: string;
  amount: number;
  category: string;
  creatorId: string | null;
  createdAt: Date;
  updatedAt: Date;
}
