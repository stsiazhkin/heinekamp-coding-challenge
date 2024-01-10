import {expectedApiKeys} from "../app-settings";

export const shouldAuthenticate = (apiKey: string):boolean => {
    return expectedApiKeys.includes(apiKey);
};

