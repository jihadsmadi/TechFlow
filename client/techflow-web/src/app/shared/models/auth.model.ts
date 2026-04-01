// ── Requests 

export interface LoginRequest {
  email:    string;
  password: string;
}

export interface RegisterRequest {
  // Company info
  companyName:  string;
  companySlug:  string;
  companyEmail: string;
  industry:     string | null;

  // User info
  firstName: string;
  lastName:  string;
  email:     string;
  password:  string;
}

// ── Responses 

export interface UserAuthDto {
  id:        string;
  email:     string;
  firstName: string;
  lastName:  string;
  fullName:  string;
  companyId: string;
  role:      string;
}

export interface AuthResponse {
  accessToken:           string;
  refreshToken:          string;
  expiresAt:             string;   
  refreshTokenExpiresAt: string;
  user:                  UserAuthDto;
}