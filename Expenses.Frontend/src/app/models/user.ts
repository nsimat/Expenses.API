/**
 * Represents a user with personal and account details.
 *
 * @interface User
 * @property {string} id - A unique identifier for the user.
 * @property {string} email - The email address associated with the user.
 * @property {(string | null)} firstName - The first name of the user. Can be null if not provided.
 * @property {(string | null)} lastName - The last name of the user. Can be null if not provided.
 * @property {(string | null)} dateOfBirth - The user's date of birth in ISO format. Can be null if not provided.
 * @property {(string | null)} createdAt - The timestamp when the user was created, in ISO format. Can be null if not stored.
 * @property {(string | null)} updatedAt - The timestamp of the user's last update, in ISO format. Can be null if not stored.
 */
export interface User {
  id: string;
  email: string;
  firstName: string | null;
  lastName: string | null;
  dateOfBirth: string | null;
  createdAt: string | null;
  updatedAt: string | null;
}
