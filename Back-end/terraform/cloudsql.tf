# Cloud SQL PostgreSQL Instance
resource "google_sql_database_instance" "postgres" {
  name             = "moviehub-postgres-${var.environment}"
  database_version = var.db_version
  region           = var.region

  settings {
    tier              = var.db_tier
    availability_type = "REGIONAL"
    disk_size         = 100
    disk_type         = "PD_SSD"

    backup_configuration {
      enabled                        = true
      start_time                     = "03:00"
      point_in_time_recovery_enabled = true
      transaction_log_retention_days = 7
      backup_retention_settings {
        retained_backups = 30
      }
    }

    ip_configuration {
      ipv4_enabled    = false
      private_network = google_compute_network.vpc.id
      require_ssl     = true
    }

    database_flags {
      name  = "max_connections"
      value = "1000"
    }

    database_flags {
      name  = "shared_buffers"
      value = "2097152" # 2GB in 8KB pages
    }
  }

  deletion_protection = true

  depends_on = [google_service_networking_connection.private_vpc_connection]
}

# Create databases
resource "google_sql_database" "databases" {
  for_each = toset([
    "moviehub_auth",
    "moviehub_user",
    "moviehub_movies",
    "moviehub_upload",
    "moviehub_streaming",
    "moviehub_library",
    "moviehub_torrent",
    "moviehub_notification",
    "moviehub_processing"
  ])

  name     = each.key
  instance = google_sql_database_instance.postgres.name
}

# Create database user
resource "google_sql_user" "postgres_user" {
  name     = "moviehub_app"
  instance = google_sql_database_instance.postgres.name
  password = random_password.db_password.result
}

resource "random_password" "db_password" {
  length  = 32
  special = true
}

# Private VPC connection for Cloud SQL
resource "google_compute_global_address" "private_ip_address" {
  name          = "moviehub-private-ip"
  purpose       = "VPC_PEERING"
  address_type  = "INTERNAL"
  prefix_length = 16
  network       = google_compute_network.vpc.id
}

resource "google_service_networking_connection" "private_vpc_connection" {
  network                 = google_compute_network.vpc.id
  service                 = "servicenetworking.googleapis.com"
  reserved_peering_ranges = [google_compute_global_address.private_ip_address.name]
}
