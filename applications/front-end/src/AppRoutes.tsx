import React from 'react';
import {Route, Routes} from 'react-router-dom';
import {AuthenticationContainer} from "./containers/AuthenticationContainer";
import {routes} from "./app-settings";
import {FilesTableContainer} from "./containers/FilesTableContainer";

export const AppRoutes: React.FC = () => {
    return (<Routes>
        <Route path={routes.home} element={<AuthenticationContainer/>}/>
        <Route path={routes.filesTable} element={<FilesTableContainer/>}/>
    </Routes>)
};