#!/bin/bash

echo "Initializing LocalStack services..."

awslocal --endpoint-url=http://localhost:4566 s3 mb s3://files-bucket

CORS_POLICY=$(cat <<EOF
{
  "CORSRules": [
    {
      "AllowedHeaders": ["*"],
      "AllowedMethods": ["GET", "POST", "PUT"],
      "AllowedOrigins": ["*"],
      "ExposeHeaders": []
    }
  ]
}
EOF
)
  
awslocal --endpoint-url=http://localhost:4566 s3api put-bucket-cors --bucket files-bucket --cors-configuration "$CORS_POLICY"

echo "Finished LocalStack services initialisation."

