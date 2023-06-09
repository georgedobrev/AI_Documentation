import axios, { AxiosResponse } from 'axios';

const BASE_URL = "https://localhost:43658/api";

const instance = axios.create({
  baseURL: BASE_URL,
  headers: {
    'Authorization': `Bearer ${localStorage.getItem('token') || ''}`
  }
});

const setBearerToken = (token: string): void => {
  localStorage.setItem('token', `Bearer ${token}`);
};


interface RegisterData {
    username: string;
    email: string;
    password: string;
  }
  
  interface LoginData {
    username: string;
    password: string;
  }
  
  interface User {
    username: string;
    email: string;
  }
  
  interface ForgotPasswordData {
    email: string;
  }
  
  interface ForgotPasswordResponse {
    message: string;
  }
  
  interface TokenResponse {
    token: string;
  }
  
  interface RabbitMQMessage {
    id: number;
    message: string;
    timestamp: string;
  }

  export const registerUser = async (data: RegisterData): Promise<TokenResponse> => {
    const response: AxiosResponse<TokenResponse> = await instance.post('/Account/register', data);
    setBearerToken(response.data.token);
    return response.data;
  };
  
  export const loginUser = async (data: LoginData): Promise<TokenResponse> => {
    const response: AxiosResponse<TokenResponse> = await instance.post('/Account/login', data);
    setBearerToken(response.data.token);
    return response.data;
  };
  
  export const sendResetPasswordLink = async (data: ForgotPasswordData): Promise<ForgotPasswordResponse> => {
    const response: AxiosResponse<ForgotPasswordResponse> = await instance.post('/Account/ForgotPassword', data);
    return response.data;
  };
  
  export const sendGoogleToken = async (tokenId: string): Promise<TokenResponse> => {
    const response: AxiosResponse<TokenResponse> = await instance.post(`${BASE_URL}/Account/GoogleResponse`, { tokenId });
    setBearerToken(response.data.token);
    return response.data;
  };
  
  export const resetPassword = async (email: string) => {
    const response = await instance.post("/Account/ResetPassword", { email });
    if (response.status !== 200) {
      throw new Error(`Error: ${response.status} ${response.statusText}`);
    }
    return response.data;
  };

 
  
  export const postRabbitMQMessage = async (message: string): Promise<RabbitMQMessage> => {
    const response = await instance.post('/RabittMQ/message', { message });
    return response.data;
  };

