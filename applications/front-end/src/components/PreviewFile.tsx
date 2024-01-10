import React from 'react';
import {getFileType} from '../helpers/getFileType';

interface PreviewFileProps{
    thumbnailImage: string
    fileContentType: string
}

export const PreviewFile: React.FC<PreviewFileProps> = (file) => {

    const fileType = getFileType(file.fileContentType);
    
    return (
        <>
            {fileType === 'image' && <div className={'preview-image-wrapper'}><img src={getFileUrl(file)} alt='Preview' className={'preview-image'}/></div>}
            {fileType === 'pdf' && <div className={'preview-image-wrapper'}><embed src={getFileUrl(file)} type='application/pdf' className={'preview-image'} /></div>}
            {fileType === 'text' && <div className={'preview-image-wrapper'}><embed src={getFileUrl(file)} type='text/plain' className={'preview-image'}/></div>}
            {fileType === 'word' && <div className={'preview-image-wrapper'}>Word Document</div>}
            {fileType === 'excel' && <div className={'preview-image-wrapper'}>Excel Spreadsheet</div>}
            {fileType === 'unknown' && <div className={'preview-image-wrapper'}>No Preview</div>}
        </>
    );
}

function getFileUrl(file:PreviewFileProps):string {
    const byteCharacters = atob(file.thumbnailImage);
    const byteNumbers = new Array(byteCharacters.length);
    for (let i = 0; i < byteCharacters.length; i++) {
        byteNumbers[i] = byteCharacters.charCodeAt(i);
    }
    const byteArray = new Uint8Array(byteNumbers);

    const blob: Blob = new Blob([byteArray], { type: file.fileContentType });

    return URL.createObjectURL(blob);
}



