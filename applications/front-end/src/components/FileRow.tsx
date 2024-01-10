import React from 'react';
import {FileInfoProps} from './FilesTable';
import {DownloadFile} from './DownloadFile';
import {PreviewFile} from './PreviewFile';
import {FileIcon} from './FileIcon';
import Form from 'react-bootstrap/Form';

interface FileRowProps extends FileInfoProps{
    onDownload: () => void
    onToggleSelection: (fileId:string) => void
    selectedFiles: Set<string>
}

export const FileRow: React.FC<FileRowProps> = ({selectedFiles, onToggleSelection, onDownload, ...fileInfo}) => {
    return (
        <tr>
            <td>{<FileIcon fileContentType={fileInfo.fileContentType}/>}</td>
            <td>{fileInfo.thumbnailImage ? 
                <PreviewFile thumbnailImage={fileInfo.thumbnailImage} fileContentType={fileInfo.fileContentType}/> 
                : 'no preview'}</td>
            <td>{fileInfo.fileName}</td>
            <td>{fileInfo.uploadedOn}</td>
            <td>{fileInfo.downloadedCount}</td>
            <td><DownloadFile fileId={fileInfo.fileId} fileName={fileInfo.fileName} onTrigger={onDownload}/></td>
            <td><Form.Check checked={selectedFiles.has(fileInfo.fileId)}
                            onChange={() => onToggleSelection(fileInfo.fileId)} /></td>
        </tr>
    );
};