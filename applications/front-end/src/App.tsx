import React from 'react';
import { BrowserRouter, useLocation } from 'react-router-dom';
import { AppRoutes } from './AppRoutes';
import logo from './logo.svg';
import './App.css';

const App: React.FC = () => (
    <BrowserRouter>
      <AppRoutes/>
    </BrowserRouter>
);

export default App;
