import React, {useState} from 'react';
import { useNavigate } from 'react-router-dom';
import { routes, sessionStorageKeys} from '../app-settings';
import Button from "react-bootstrap/Button";
import {shouldAuthenticate} from "../helpers/authenticationHelper";

// dummy container
//to set api key which will be used in each request to backend API
export const AuthenticationContainer: React.FC = () => {
    const [key, setKey] = useState<string>('');
    const navigate = useNavigate();

    const handleLogin = () => {
        if(shouldAuthenticate(key)){
            sessionStorage.setItem(sessionStorageKeys.token, key);
            navigate(routes.filesTable);    
        } else {
            alert('Invalid API key');   
        }
    };
    
    const handleKeyChange = (event:React.ChangeEvent<HTMLInputElement>) => {
        setKey(event.target.value)
    }

    return (
        <div className={'center-screen'}>
            <input
                type="text"
                value={key}
                onChange={handleKeyChange}
                placeholder="Enter your API Key"/>
            <Button onClick={handleLogin}>Login</Button>
        </div>
    );
};