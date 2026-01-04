-- Create UserProfiles table
CREATE TABLE IF NOT EXISTS "UserProfiles" (
    "Id" uuid NOT NULL,
    "UserId" uuid NOT NULL,
    "Email" varchar(255) NOT NULL,
    "DisplayName" varchar(100) NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_UserProfiles" PRIMARY KEY ("Id")
);

-- Create unique index on UserId
CREATE UNIQUE INDEX IF NOT EXISTS "IX_UserProfiles_UserId" ON "UserProfiles" ("UserId");

-- Create __EFMigrationsHistory table
CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" varchar(150) NOT NULL,
    "ProductVersion" varchar(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

-- Insert migration record
INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20251225072617_InitialCreate', '8.0.0')
ON CONFLICT DO NOTHING;
