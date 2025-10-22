export interface LoginRequest {
  email: string;
  password: string;
}

export interface LoginResponse {
  success?: boolean;
  message?: string;
  data?: {
    email?: string;
    accessToken: string;
    role: string;
    id?: string;
    name?: string;
  };
  // Fallback for direct response (backward compatibility)
  token?: string;
  role?: string;
  id?: string;
  name?: string;
  email?: string;
}

export interface RegistrationRequest {
  name: string;
  email: string;
  password: string;
  address?: string;
  phoneNumber?: string;
  city: string;
}


