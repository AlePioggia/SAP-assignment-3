export interface User {
    id: string;
    username: string;
    createdAt: Date;
  }
  
  export interface UserRegistrationRequest {
    username: string;
    password: string;
  }
  
  export interface UserAuthenticationRequest {
    username: string;
    password: string;
  }
  
  export interface UserResponse {
    id: string;
    username: string;
    createdAt: Date;
  }
  