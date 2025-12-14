-- Create databases for each microservice
CREATE DATABASE moviehub_auth;
CREATE DATABASE moviehub_user;
CREATE DATABASE moviehub_movies;
CREATE DATABASE moviehub_upload;
CREATE DATABASE moviehub_streaming;
CREATE DATABASE moviehub_library;
CREATE DATABASE moviehub_torrent;
CREATE DATABASE moviehub_notification;
CREATE DATABASE moviehub_processing;

-- Grant permissions
GRANT ALL PRIVILEGES ON DATABASE moviehub_auth TO postgres;
GRANT ALL PRIVILEGES ON DATABASE moviehub_user TO postgres;
GRANT ALL PRIVILEGES ON DATABASE moviehub_movies TO postgres;
GRANT ALL PRIVILEGES ON DATABASE moviehub_upload TO postgres;
GRANT ALL PRIVILEGES ON DATABASE moviehub_streaming TO postgres;
GRANT ALL PRIVILEGES ON DATABASE moviehub_library TO postgres;
GRANT ALL PRIVILEGES ON DATABASE moviehub_torrent TO postgres;
GRANT ALL PRIVILEGES ON DATABASE moviehub_notification TO postgres;
GRANT ALL PRIVILEGES ON DATABASE moviehub_processing TO postgres;
