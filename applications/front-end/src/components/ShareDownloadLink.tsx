import React, {useState} from "react";
import Button from "react-bootstrap/Button";
import {useRequest} from "../hooks/useRequest";

interface ShareDownloadLinkProps{
    selectedFiles: Set<string>,
    resetSelectedFiles: () => void
    onDownload: () => void
}

export const ShareDownloadLink: React.FC<ShareDownloadLinkProps> = (selectedFiles) => {
    const [ showLink, setShowLink ] = useState<boolean>(false);
    const [ downLoadLink, setDownLoadLink ] = useState<string>('');
        
    const queryString = Array.from(selectedFiles.selectedFiles)
        .map(value => `fileIds=${encodeURIComponent(value)}`).join('&');
    
    const getFilesDownloadUrl = useRequest(
        `/files/bulk-download/?${queryString}`,
        {
            method: 'GET',
            headers: {},
        },
    ); 
    
    const handleGetDownloadLink = async () => {
        try {
            const downloadUrl = await getFilesDownloadUrl();

            selectedFiles.resetSelectedFiles();

            selectedFiles.onDownload();

            setDownLoadLink(downloadUrl);
            setShowLink(true);
        }
        catch (error){
            alert(error);
        }
    };
    
    
    return (
        <>
            <Button onClick={handleGetDownloadLink} size="lg">Get download link</Button>
            <br/>
            {
                showLink ?
                    <>
                        <a href={downLoadLink}>Here is the shareable download link</a>
                        <br/>
                        <span>Pick files above to include into a new download link</span>
                    </> :
                    <span>Pick files above to include</span>
            }
        </>
    );
}