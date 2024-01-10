import name from "../package.json";

export const settings = {
    appName: name,
    apiBasePath: 'http://localhost:5062'
};

export const sessionStorageKeys = {
    token: 'key',
};

export const routes = {
    home: '/',
    filesTable:'/files-table'
} 

//dev mocks only for demo purposes we never store keys in code
//in prod it would look different in general
//we only need them here to directly call API on local machine
//they should be the same as in 
// 'appsettings.Development.json' of  'Heinekamp.CodingChallenge.FileApi'
export const expectedApiKeys = [
    'dcbb55abc0c447498a26e1a22f4ec81c',
    'd37cc31f6dd2434abd826456ae4b7063'
]

