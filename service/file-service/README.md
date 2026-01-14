# File Service - Movie Poster Upload Microservice

A microservice for handling movie poster image uploads and serving static files.

## Features

- **Secure File Upload**: JWT authenticated file uploads
- **Image Validation**: Supports JPG, PNG, GIF, WebP formats
- **File Size Limits**: 5MB maximum file size
- **Local Storage**: Files stored in wwwroot/uploads directory
- **Public URLs**: Generates accessible file URLs
- **Static File Serving**: Direct file access via /uploads/{filename}

## API Endpoints

### POST /api/files/upload
Upload a movie poster image.

**Authentication**: Required (JWT Bearer token)

**Content-Type**: multipart/form-data

**Request Body**:
- `file`: Image file (JPG, PNG, GIF, WebP)

**Response** (200 OK):
```json
{
  "fileName": "a1b2c3d4-e5f6-7890-abcd-ef1234567890.jpg",
  "fileUrl": "https://api.example.com/uploads/a1b2c3d4-e5f6-7890-abcd-ef1234567890.jpg",
  "fileSize": 245760,
  "contentType": "image/jpeg",
  "uploadedAt": "2026-01-14T16:41:00Z"
}
```

### GET /uploads/{filename}
Serve uploaded files directly.

**Authentication**: Not required (public access)

**Example**: `GET /uploads/a1b2c3d4-e5f6-7890-abcd-ef1234567890.jpg`

## Security

- JWT token validation (same as other microservices)
- File type validation (images only)
- File size limits (5MB max)
- Secure filename generation (GUID-based)

## Configuration

```json
{
  "Jwt": {
    "SecretKey": "your-super-secret-key-here-make-it-long-and-secure",
    "Issuer": "AuthService",
    "Audience": "MovieService"
  }
}
```

## File Storage

- **Directory**: wwwroot/uploads/
- **Naming**: GUID-based unique filenames
- **Access**: Public via /uploads/ route
- **Cleanup**: Manual cleanup required (files persist until deleted)

## Usage Example

```bash
# Upload a file
curl -X POST "https://api.example.com/api/files/upload" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -F "file=@poster.jpg"

# Access uploaded file
curl "https://api.example.com/uploads/a1b2c3d4-e5f6-7890-abcd-ef1234567890.jpg"
```