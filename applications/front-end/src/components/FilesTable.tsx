import React, {useEffect, useState} from "react";
import {FileRow} from "./FileRow";
import {useRequest} from "../hooks/useRequest";
import {UploadFile} from "./UploadFile";
import {Table} from "react-bootstrap";
import {ShareDownloadLink} from "./ShareDownloadLink";
import Button from "react-bootstrap/Button";
import {useNavigate} from "react-router-dom";
import {routes, sessionStorageKeys} from "../app-settings";

interface FilesTableResponse{
    items: FileInfoProps[]
}

export interface FileInfoProps {    
    fileId: string,
    fileName: string,
    downloadedCount: number,
    uploadedOn: string,
    thumbnailImage?: string | null
    fileContentType: string
}

export const FilesTable: React.FC = () => {
    const [ selectedFiles, setSelectedFiles ] = useState<Set<string>>(new Set());
    const [files, setData] = useState<FileInfoProps[]>([]);

    useEffect(() => {
        (async  () => {
            await stateUpdate();
        })()
    }, []);

    const navigate = useNavigate();
    
    const onChange = async () => {
        await stateUpdate();
    }
    
    const getFilesInfo = useRequest(
        `/files/`,
        {
            method: 'GET',
            headers: {},
        },
    );
    
    const stateUpdate = async () => {
        const response: FilesTableResponse = await getFilesInfo();
        setData(response.items);
    }

    const toggleSelection = (fileId:string) => {
        setSelectedFiles(prevSelected => {
            const newSelected = new Set(prevSelected);
            if (newSelected.has(fileId)) {
                newSelected.delete(fileId);
            } else {
                newSelected.add(fileId);
            }
            return newSelected;
        });
    };
    
    const rows: any[] = [];
    files.forEach((file) => {
        rows.push(
            <FileRow 
                key = {file.fileId} 
                {...file} 
                onDownload={onChange} 
                onToggleSelection={toggleSelection} 
                selectedFiles={selectedFiles}/>
        );
    });
    
    const resetSelectedFiles = () => {
        setSelectedFiles(new Set())
    }
    
    const handleLogout = () => {
        setData([]);
        sessionStorage.setItem(sessionStorageKeys.token, '');
        navigate(routes.home);
    };
    
    return (
        <div>
            <Button variant="danger" onClick={handleLogout}>Log out</Button>
            <Table bordered striped hover>
                <thead>
                <tr>
                    <th>Icon</th>
                    <th>Preview</th>
                    <th>Name</th>
                    <th>Uploaded</th>
                    <th>Downloaded</th>
                    <th></th>
                    <th></th>
                </tr>
                </thead>
                <tbody>
                    {rows}
                    <tr>
                        <td colSpan={5}></td>
                        <td><UploadFile onUpload={onChange}/></td>
                        <td>
                            <ShareDownloadLink 
                            resetSelectedFiles={resetSelectedFiles} 
                            selectedFiles={selectedFiles} 
                            onDownload={onChange}/>
                        </td>
                    </tr>
                </tbody>
            </Table>
        </div>
    );
};