import React from 'react'
import ReactDOM from 'react-dom/client'
import axios from 'axios';
import App from './App.tsx'
import './index.css'

axios.interceptors.request.use((config) => {
  const token = localStorage.getItem("token");
  config.headers.Authorization = token ? `${token}` : '';
  return config;
}, (error) => {
  return Promise.reject(error);
});

ReactDOM.createRoot(document.getElementById('root') as HTMLElement).render(
  <React.StrictMode>
    <App />
  </React.StrictMode>,
)
