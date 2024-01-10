import React from 'react';
import {useRequest} from "../hooks/useRequest";
import Button from "react-bootstrap/Button";

export const DownloadFile: React.FC<{ fileId: string, fileName: string, onTrigger: () => void }> = (file) => {
    
    const getFileDownloadUrl = useRequest(
        `/files/${file.fileId}`,
        {
            method: 'GET',
            headers: {},
        },
    );

    const handleDownload = async () => {
        const url = await getFileDownloadUrl();
        
        fetch(url)
            .then(response => response.blob())
            .then(blob => {
                const url = window.URL.createObjectURL(blob);
                const a = document.createElement('a');
                a.href = url;
                a.download = file.fileName;
                document.body.appendChild(a);
                a.click();
                a.remove();
                window.URL.revokeObjectURL(url);
            })
            .catch((error) => {
                console.log(error);
                alert('Problem downloading the file')
            });
        
        file.onTrigger();
    };
    
    return (
        <Button variant="success" onClick={handleDownload}>Download File</Button>
    );
};