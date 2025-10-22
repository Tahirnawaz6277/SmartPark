export interface UserRegistrationRequest {
  name: string;
  email: string;
  password: string;
  address?: string;
  phoneNumber?: string;
  city: string;
}

export interface UserLoginRequest {
  email: string;
  password: string;
}

export interface UserLoginResponse {
  token: string;
  role: string;
  id?: string;
  name?: string;
  email?: string;
}

export interface UserDto {
  id: string;
  name: string;
  email: string;
  address?: string;
  phoneNumber?: string;
  city?: string;
  roleId?: string;
  roleName?: string;
}

export interface UserUpdateRequest {
  name: string;
  email: string;
  address?: string;
  phoneNumber?: string;
  city: string;
}
