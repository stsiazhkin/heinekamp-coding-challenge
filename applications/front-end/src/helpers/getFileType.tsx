
export const getFileType = (fileContentType: string):string => {
    if (fileContentType.includes('image/')) {
        return 'image';
    } else if (fileContentType === 'application/pdf') {
        return 'pdf';
    } else if (fileContentType === 'text/plain') {
        return 'text';
    } else if (fileContentType === 'application/vnd.openxmlformats-officedocument.wordprocessingml.document' || fileContentType === 'application/msword') {
        return 'word';
    } else if (fileContentType === 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' || fileContentType === 'application/vnd.ms-excel') {
        return 'excel';
    } else {
        return 'unknown';
    }
}