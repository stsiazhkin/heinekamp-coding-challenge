import React from "react";
import {sessionStorageKeys} from "../app-settings";
import {shouldAuthenticate} from "../helpers/authenticationHelper";
import {FilesTable} from "../components/FilesTable";
import {Navigate} from "react-router-dom";

export const FilesTableContainer: React.FC  = () => {
    const token = sessionStorage.getItem(sessionStorageKeys.token);
    
    if(token && shouldAuthenticate(token))
    {
        return (<FilesTable/>);
    }

    return <Navigate to="/" replace />;
};