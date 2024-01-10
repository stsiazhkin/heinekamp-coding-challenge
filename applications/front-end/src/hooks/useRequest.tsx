import {useRef} from 'react';

import { settings, sessionStorageKeys } from '../app-settings';

export const useRequest = (url?: string, options?: RequestInit) => {
    const tokenRef = useRef<string | null>(null);

    return async (requestParams?: { [key: string]: any }) => {
        const defaultParams: RequestInit = {
            headers: {'Content-Type': 'application/json'},
        };

        const params = {...defaultParams, ...options, ...requestParams};

        if (!tokenRef.current) {
            tokenRef.current = sessionStorage.getItem(sessionStorageKeys.token)!;
        }

        params.headers = {
            'x-api-key': tokenRef.current,
            ...params.headers
        }

        const requestUrl = url || requestParams?.url;

        const fullUrl = `${settings.apiBasePath}${requestUrl}`;

        return fetch(fullUrl, params).then(async (response) => {
            const responseBody = response.headers.get('Content-Type')?.includes('application/json')
                ? await response.json()
                : await response.text();

            if (!response.ok) {
                if (response.status === 400) {
                    throw Error("Invalid request");
                }

                if (response.status === 500) {
                    throw Error("Server error");
                }
            }

            return responseBody;
        });
    };
};