import axios, { AxiosResponse } from 'axios';

const BASE_URL = "https://localhost:5001/api"; 

const instance = axios.create({
    baseURL: BASE_URL,
});

interface RegisterData {
    username: string;
    email: string;
    password: string;
}

interface LoginData {
    username: string;
    password: string;
}

interface UserResponse {
    username: string;
    password: string;
    email: string;
}

interface ForgotPasswordData {
    email: string;
}

interface ForgotPasswordResponse {
    message: string;
}

export const registerUser = async (data: RegisterData): Promise<UserResponse> => {
    const response: AxiosResponse<UserResponse> = await instance.post('/Account/register', data);
    return response.data;
};

export const loginUser = async (data: LoginData): Promise<UserResponse> => {
    const response: AxiosResponse<UserResponse> = await instance.post('/Account/login', data);
    return response.data;
};

export const sendResetPasswordLink = async (data: ForgotPasswordData): Promise<ForgotPasswordResponse> => {
    const response: AxiosResponse<ForgotPasswordResponse> = await instance.post('/Account/forgot-password', data);
    return response.data;
};
