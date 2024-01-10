import React, {ChangeEvent, useRef} from 'react';
import {useRequest} from '../hooks/useRequest';
import Button from 'react-bootstrap/Button';

export const UploadFile: React.FC<{onUpload:() => void}> = (onUpload) => {
    const postFile = useRequest(
        `/files/`,
        {
            method: 'POST',
            headers: {},
        },
    );
    const fileInputRef = useRef<HTMLInputElement>(null);
    
    const handleFileChange = async (event: ChangeEvent<HTMLInputElement>) => {
        if (event.target.files) {
            await handleUpload(event.target.files[0]);
        }
    };

    const handleUpload = async (file: any) => {
        if (file) {
            try {
                const formData = new FormData();
                formData.append('file', file);

                const uploadFile = async  () => {
                    const response = await postFile({ body: formData });
                };

                await uploadFile();
                
                console.log('File uploaded successfully');

                onUpload.onUpload();
            } catch (error) {
                console.error('Error uploading file:', error);
                alert('Error uploading file:');
            }
        }
    };
    
    const handleUploadButtonClick = () => {
        if(fileInputRef.current){
            fileInputRef.current.click();    
        }
    }

    return (
        <div>
            <input type='file' onChange={handleFileChange} ref={fileInputRef} className={'hidden'} />
            <Button variant='primary' size='lg' onClick={handleUploadButtonClick}>Upload File</Button>
        </div>
    );
};