import React from 'react';
import { BrowserRouter as Router, Route, Routes } from 'react-router-dom';
import LoginPage from './Components/Auth/LoginPage';
import RegisterPage from './Components/Auth/RegisterPage';
import ForgotPasswordPage from './Components/Auth/ForgotPassword';
import ResetPasswordPage from './Components/Auth/ResetPasswordPage';
import MainPage from './Components/MainPage';

function App() {
  return (
    <Router>
      <Routes>
        <Route path="/login" element={<LoginPage />} />
        <Route path="/register" element={<RegisterPage />} />
        <Route path="/forgot-password" element={<ForgotPasswordPage />} />
        <Route path="/reset-password" element={<ResetPasswordPage />} />
        <Route path="/" element={<MainPage />} /> 
      </Routes>
    </Router>
  );
}

export default App;