# Keepzo PC Backup - Backend Implementation Summary

## Overview

Backend tarafında Keepzo PC Backup için gerekli tüm temel altyapı başarıyla oluşturuldu. MinIO object storage entegrasyonu, database entity'leri ve MinIO storage servisi tamamlandı.

## Implementation Date

December 4, 2024

## Completed Components

### 1. Docker & Infrastructure ( Complete)

#### **MinIO Docker Service** - `_devops/docker/compose/common.yml`
- MinIO container eklendi (latest image)
- API Port: 9000, Console Port: 9001
- Health check yapılandırması
- Volume: `minio_data`
- Network: `maden-network` with alias `minio-container`

#### **Environment Variables** - `common.yml`
```yaml
MINIO_ENDPOINT: minio:9000
MINIO_ACCESS_KEY: ${MINIO_ROOT_USER}
MINIO_SECRET_KEY: ${MINIO_ROOT_PASSWORD}
MINIO_USE_SSL: "false"
MINIO_BUCKET_NAME: "keepzo-backups"
```

#### **Environment Files Updated**
- `_devops/docker/env/dev.env`: Development MinIO credentials
- `_devops/docker/env/prod.env`: Production MinIO credentials (requires password change)
- `KEEPZO_FILE_STORAGE_PATH` added to both environments

### 2. Domain Entities ( Complete)

All entities created in `BusinessModules/Keepzo/BusinessModules.Keepzo.Domain/Entities/`:

#### **KeepzoBackupJob.cs**
- Backup job tracking (Full, Incremental, Differential)
- Status: Pending, Running, Completed, Failed, Cancelled
- Statistics: TotalFiles, TotalBytes, ProcessedFiles, FailedFiles, SkippedFiles
- Timestamps: StartedAt, CompletedAt
- Navigation: License, BackupFiles collection

#### **KeepzoBackupFile.cs**
- File metadata: FullPath, RelativePath, FileName, FileSizeBytes
- SHA-256 FileHash for integrity
- FileModifiedAt timestamp
- StorageUrl in MinIO
- ChunkCount for large files
- UploadStatus: Pending, Uploading, Completed, Failed, Skipped
- Navigation: BackupJob, License, Chunks collection

#### **KeepzoFileChunk.cs**
- Chunk tracking: ChunkIndex, ChunkSizeBytes
- SHA-256 ChunkHash for verification
- StorageUrl in MinIO
- ChunkUploadStatus: Pending, Uploading, Completed, Failed
- Navigation: BackupFile

#### **KeepzoBackupSource.cs**
- Source configuration: Path, IncludePatterns, ExcludePatterns
- IncludeSubdirectories flag
- MaxFileSizeBytes limit
- IsEnabled, Priority fields
- Navigation: License

### 3. Entity Configurations ( Complete)

All EF Core configurations created in `BusinessModules/Keepzo/BusinessModules.Keepzo.Infrastructure/RelationalDB/Configurations/`:

#### **KeepzoBackupJobConfiguration.cs**
- Table: `KeepzoBackupJobs`
- Indexes: LicenseId, Status, StartedAt, (DeviceId + StartedAt)
- Foreign Keys: License (Restrict), BackupFiles (Cascade)

#### **KeepzoBackupFileConfiguration.cs**
- Table: `KeepzoBackupFiles`
- String limits: FullPath/RelativePath (1000), FileName (255), FileHash (64)
- Indexes: BackupJobId, LicenseId, FileHash, (LicenseId + RelativePath), UploadStatus
- Foreign Keys: BackupJob (Cascade), License (Restrict), Chunks (Cascade)

#### **KeepzoFileChunkConfiguration.cs**
- Table: `KeepzoFileChunks`
- Unique Index: (BackupFileId + ChunkIndex)
- Indexes: BackupFileId, ChunkHash, Status
- Foreign Key: BackupFile (Cascade)

#### **KeepzoBackupSourceConfiguration.cs**
- Table: `KeepzoBackupSources`
- Unique Index: (LicenseId + Path)
- Default values: IncludePatterns ("*.*"), ExcludePatterns
- Foreign Key: License (Cascade)

### 4. DbContext Updates ( Complete)

#### **IKeepzoModuleDbContext.cs** - Interface updated with:
```csharp
DbSet<KeepzoBackupJob> BackupJobs { get; set; }
DbSet<KeepzoBackupFile> BackupFiles { get; set; }
DbSet<KeepzoFileChunk> FileChunks { get; set; }
DbSet<KeepzoBackupSource> BackupSources { get; set; }
```

#### **KeepzoModuleDbContext.cs** - Implementation updated with:
- DbSet properties for all 4 new entities
- ApplyConfiguration calls for all 4 new configurations
- Table prefix logic already handles new tables

#### **ApplicationDbContext.cs** (Migration) - Updated with:
- DbSet properties for PC Backup entities
- ApplyConfiguration calls in OnModelCreating

### 5. MinIO Storage Service ( Complete)

#### **MinioStorageService.cs** - `BusinessModules.Keepzo.Infrastructure/Services/`

**Interface (IMinioStorageService):**
- `EnsureBucketExistsAsync()` - Bucket oluşturma/kontrol
- `UploadFileAsync()` - Generic file upload
- `UploadChunkAsync()` - Chunk-specific upload with path: `{licenseId}/{fileId}/chunk_{index}`
- `DownloadFileAsync()` - File download to stream
- `DeleteFileAsync()` - File deletion
- `GetPresignedUrlAsync()` - Pre-signed URL generation (1 hour default)

**Implementation:**
- Configuration-based initialization (MinIO:Endpoint, AccessKey, SecretKey, UseSSL, BucketName)
- Automatic bucket creation if not exists
- Comprehensive error logging
- Default bucket: `keepzo-backups`
- Chunk path format: `{licenseId}/{fileId}/chunk_{chunkIndex:D6}`

### 6. NuGet Package ( Complete)

**BusinessModules.Keepzo.Infrastructure.csproj:**
```xml
<PackageReference Include="Minio" Version="6.0.1" />
```

## File Structure

```
backend/
├── _devops/docker/
│   ├── compose/
│   │   └── common.yml (MinIO service added)
│   └── env/
│       ├── dev.env (MinIO config added)
│       └── prod.env (MinIO config added)
│
├── BusinessModules/Keepzo/
│   ├── BusinessModules.Keepzo.Domain/
│   │   └── Entities/
│   │       ├── KeepzoBackupJob.cs (new)
│   │       ├── KeepzoBackupFile.cs (new)
│   │       ├── KeepzoFileChunk.cs (new)
│   │       └── KeepzoBackupSource.cs (new)
│   │
│   └── BusinessModules.Keepzo.Infrastructure/
│       ├── BusinessModules.Keepzo.Infrastructure.csproj (Minio 6.0.1 added)
│       ├── Services/
│       │   └── MinioStorageService.cs (new)
│       └── RelationalDB/
│           ├── IKeepzoModuleDbContext.cs (updated)
│           ├── KeepzoModuleDbContext.cs (updated)
│           └── Configurations/
│               ├── KeepzoBackupJobConfiguration.cs (new)
│               ├── KeepzoBackupFileConfiguration.cs (new)
│               ├── KeepzoFileChunkConfiguration.cs (new)
│               └── KeepzoBackupSourceConfiguration.cs (new)
│
└── Jobs/RelationalDB/MigrationJob/Database/
    └── ApplicationDbContext.cs (updated)
```

## Database Schema

### Table: Keepzo_KeepzoBackupJobs
```sql
- Id (Guid, PK)
- LicenseId (Guid, FK -> KeepzoLicenses, Index)
- DeviceId (nvarchar(100), Index)
- DeviceName (nvarchar(200))
- Type (int) -- 0=Full, 1=Incremental, 2=Differential
- Status (int, Index) -- 0=Pending, 1=Running, 2=Completed, 3=Failed, 4=Cancelled
- StartedAt (datetime2, Index)
- CompletedAt (datetime2)
- TotalFiles (int)
- TotalBytes (bigint)
- ProcessedFiles (int)
- ProcessedBytes (bigint)
- FailedFiles (int)
- SkippedFiles (int)
- ErrorMessage (nvarchar(2000))
- CreatedAt, UpdatedAt (BaseEntity)
```

### Table: Keepzo_KeepzoBackupFiles
```sql
- Id (Guid, PK)
- BackupJobId (Guid, FK -> BackupJobs, Cascade, Index)
- LicenseId (Guid, FK -> KeepzoLicenses, Restrict, Index)
- FullPath (nvarchar(1000))
- RelativePath (nvarchar(1000), Index with LicenseId)
- FileName (nvarchar(255))
- FileSizeBytes (bigint)
- FileHash (nvarchar(64), Index) -- SHA-256
- FileModifiedAt (datetime2)
- IsDirectory (bit)
- FileAttributes (int)
- StorageUrl (nvarchar(1000))
- ChunkCount (int)
- UploadStatus (int, Index) -- 0=Pending, 1=Uploading, 2=Completed, 3=Failed, 4=Skipped
- ErrorMessage (nvarchar(2000))
- UploadedAt (datetime2)
- CreatedAt, UpdatedAt (BaseEntity)
```

### Table: Keepzo_KeepzoFileChunks
```sql
- Id (Guid, PK)
- BackupFileId (Guid, FK -> BackupFiles, Cascade, Index)
- ChunkIndex (int, Unique with BackupFileId)
- ChunkSizeBytes (bigint)
- ChunkHash (nvarchar(64), Index) -- SHA-256
- StorageUrl (nvarchar(1000))
- Status (int, Index) -- 0=Pending, 1=Uploading, 2=Completed, 3=Failed
- UploadedAt (datetime2)
- CreatedAt, UpdatedAt (BaseEntity)
```

### Table: Keepzo_KeepzoBackupSources
```sql
- Id (Guid, PK)
- LicenseId (Guid, FK -> KeepzoLicenses, Cascade, Index)
- Path (nvarchar(500), Unique with LicenseId)
- IncludePatterns (nvarchar(500), Default: "*.*")
- ExcludePatterns (nvarchar(500), Default: "*.tmp,*.log,Thumbs.db,desktop.ini")
- IncludeSubdirectories (bit, Default: true)
- MaxFileSizeBytes (bigint, Default: 0)
- IsEnabled (bit, Index, Default: true)
- Priority (int, Default: 100)
- CreatedAt, UpdatedAt (BaseEntity)
```

## MinIO Configuration

### Docker Container
- Image: `minio/minio:latest`
- Command: `server /data --console-address ":9001"`
- API Port: 9000 (internal), mapped to host
- Console Port: 9001 (internal), mapped to 127.0.0.1 only
- Volume: `minio_data:/data`
- Network: `maden-network`

### Environment Variables (Container)
```env
MINIO_ROOT_USER=madenadmin
MINIO_ROOT_PASSWORD=MadenSecure123! (dev) / PRODUCTION_SECURE_PASSWORD_CHANGE_THIS! (prod)
```

### .NET Configuration Keys
```
MinIO:Endpoint = "minio:9000"
MinIO:AccessKey = "madenadmin"
MinIO:SecretKey = "password"
MinIO:UseSSL = "false"
MinIO:BucketName = "keepzo-backups"
```

## Next Steps (Pending Implementation)

The following components still need to be implemented:

### 1. API Endpoints (Application Layer)
Location: `BusinessModules/Keepzo/BusinessModules.Keepzo.Application/RequestHandlers/`

**Required Endpoints:**

#### `/api/keepzo/backupjobs/start` - POST
- **Handler:** `BackupJobs/Commands/Start/`
- **Request:** LicenseKey, DeviceId, DeviceName, BackupType, EstimatedFileCount, EstimatedTotalBytes
- **Response:** JobId, Status, StartedAt
- **Logic:**
  - Validate license (active check)
  - Check for running job on device
  - Create BackupJob with status=Running
  - Return job details

#### `/api/keepzo/chunks/upload` - POST
- **Handler:** `Chunks/Commands/Upload/`
- **Request:** Multipart form data with:
  - LicenseKey, JobId, RelativePath, FileHash, FileSizeBytes, FileModifiedAt
  - ChunkIndex, TotalChunks, ChunkHash
  - ChunkData (IFormFile)
- **Response:** Success, FileId, ChunkId, ChunkIndex, StorageUrl, IsFileComplete
- **Logic:**
  - Validate license & job
  - Get or create BackupFile
  - Verify chunk hash
  - Upload chunk to MinIO via MinioStorageService.UploadChunkAsync()
  - Create FileChunk record
  - Check if all chunks complete
  - Update BackupFile and BackupJob statistics

#### `/api/keepzo/backupjobs/complete` - POST
- **Handler:** `BackupJobs/Commands/Complete/`
- **Request:** LicenseKey, JobId, Success, ErrorMessage, Statistics
- **Response:** JobId, Status, CompletedAt, Duration
- **Logic:**
  - Validate license & job
  - Update job status (Completed/Failed)
  - Set CompletedAt timestamp
  - Return final statistics

### 2. Dependency Registration
**File:** `BusinessModules.Keepzo.Application/Configuration/ApplicationDependencyProvider.cs`

Add MinIO service:
```csharp
base.Add<IMinioStorageService, MinioStorageService>();
```

### 3. Database Migration
**Commands:**
```bash
cd Jobs/RelationalDB/MigrationJob
dotnet ef migrations add AddPCBackupTables
dotnet ef database update
```

### 4. API Controller Registration
Ensure Keepzo API endpoints are registered in:
- `BusinessModules.Keepzo.Api` project
- API Gateway routing configuration

## Testing Checklist

### Docker & MinIO
- [ ] `docker-compose up -d minio` - MinIO starts successfully
- [ ] Access MinIO console at `http://localhost:9001`
- [ ] Login with credentials from env file
- [ ] Bucket `keepzo-backups` is created automatically

### Database
- [ ] Migration creates all 4 new tables with correct schema
- [ ] Foreign key constraints work correctly
- [ ] Indexes are created properly
- [ ] Table prefix `Keepzo_` is applied

### MinIO Service
- [ ] Bucket creation/check works
- [ ] Chunk upload creates correct path structure
- [ ] File download works
- [ ] Pre-signed URLs generate correctly

### API Endpoints (When Implemented)
- [ ] Start backup job creates record
- [ ] Chunk upload stores in MinIO and database
- [ ] File completion triggers correctly
- [ ] Job completion updates statistics
- [ ] Error handling works for invalid license/job

## Important Notes

1. **Production Security:**
   - Change MinIO password in `prod.env` before deployment
   - Use strong passwords (minimum 16 characters)
   - Consider using secrets management

2. **MinIO Console:**
   - Development: Accessible from all interfaces
   - Production: Bound to 127.0.0.1 (localhost only)
   - Use SSH tunneling for production access

3. **Table Naming:**
   - All tables automatically prefixed with `Keepzo_`
   - Ensures no collisions with other modules

4. **Chunk Storage Path:**
   - Format: `{licenseId}/{fileId}/chunk_{chunkIndex:D6}`
   - Example: `550e8400-e29b-41d4-a716-446655440000/660e8400-e29b-41d4-a716-446655440001/chunk_000000`

5. **Hash Algorithm:**
   - SHA-256 used for both file and chunk hashes
   - 64-character hex string format

## Integration with Agent

The agent-side implementation (already completed in keepzo-agent repository) will interact with these endpoints:

1. Agent calls `/api/keepzo/backupjobs/start` to begin backup
2. Agent scans files and calculates hashes
3. Agent uploads files in chunks via `/api/keepzo/chunks/upload`
4. Agent calls `/api/keepzo/backupjobs/complete` when done

---

**Status:**  Infrastructure Complete
**Remaining:** API Endpoints, Dependency Registration, Migration
**Ready for:** API endpoint implementation and testing
**Estimated Time:** 2-3 hours for API endpoints + testing
