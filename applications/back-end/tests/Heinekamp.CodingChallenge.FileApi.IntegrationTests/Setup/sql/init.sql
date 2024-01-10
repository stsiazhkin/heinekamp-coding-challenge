CREATE TABLE files_information (
       file_id UUID PRIMARY KEY,
       file_name VARCHAR(255),
       downloaded_count BIGINT,
       uploaded_on TIMESTAMPTZ,
       uploaded_by VARCHAR(255),
       thumbnail_image BYTEA,
       file_content_type VARCHAR(255)
);