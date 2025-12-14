output "gke_cluster_name" {
  value       = google_container_cluster.primary.name
  description = "GKE Cluster Name"
}

output "gke_cluster_endpoint" {
  value       = google_container_cluster.primary.endpoint
  description = "GKE Cluster Endpoint"
  sensitive   = true
}

output "cloudsql_connection_name" {
  value       = google_sql_database_instance.postgres.connection_name
  description = "Cloud SQL Connection Name"
}

output "cloudsql_private_ip" {
  value       = google_sql_database_instance.postgres.private_ip_address
  description = "Cloud SQL Private IP"
}

output "db_password" {
  value       = random_password.db_password.result
  description = "Database Password"
  sensitive   = true
}

output "uploads_bucket" {
  value       = google_storage_bucket.uploads.name
  description = "Uploads Bucket Name"
}

output "streams_bucket" {
  value       = google_storage_bucket.streams.name
  description = "Streams Bucket Name"
}

output "torrents_bucket" {
  value       = google_storage_bucket.torrents.name
  description = "Torrents Bucket Name"
}

output "gcs_service_account_email" {
  value       = google_service_account.gcs_service_account.email
  description = "GCS Service Account Email"
}

output "gcs_service_account_key" {
  value       = google_service_account_key.gcs_key.private_key
  description = "GCS Service Account Private Key"
  sensitive   = true
}
