export interface User {
  id: string;
  email: string;
  firstName: string | null;
  lastName: string | null;
  dateOfBirth: string | null;
  dateOfRegistration: string | null;
  dateOfLastLogin: string | null;
}
