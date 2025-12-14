# GCS Buckets
resource "google_storage_bucket" "uploads" {
  name          = "${var.project_id}-uploads"
  location      = var.region
  force_destroy = false

  uniform_bucket_level_access = true

  versioning {
    enabled = true
  }

  lifecycle_rule {
    condition {
      age = 90
    }
    action {
      type = "Delete"
    }
  }

  cors {
    origin          = ["*"]
    method          = ["GET", "HEAD", "PUT", "POST", "DELETE"]
    response_header = ["*"]
    max_age_seconds = 3600
  }
}

resource "google_storage_bucket" "streams" {
  name          = "${var.project_id}-streams"
  location      = var.region
  force_destroy = false

  uniform_bucket_level_access = true

  versioning {
    enabled = false
  }

  lifecycle_rule {
    condition {
      age = 365
    }
    action {
      type          = "SetStorageClass"
      storage_class = "NEARLINE"
    }
  }

  cors {
    origin          = ["*"]
    method          = ["GET", "HEAD"]
    response_header = ["*"]
    max_age_seconds = 3600
  }
}

resource "google_storage_bucket" "torrents" {
  name          = "${var.project_id}-torrents"
  location      = var.region
  force_destroy = false

  uniform_bucket_level_access = true

  versioning {
    enabled = false
  }

  cors {
    origin          = ["*"]
    method          = ["GET", "HEAD"]
    response_header = ["*"]
    max_age_seconds = 3600
  }
}

# Service Account for GCS access
resource "google_service_account" "gcs_service_account" {
  account_id   = "moviehub-gcs-sa"
  display_name = "MovieHub GCS Service Account"
}

resource "google_storage_bucket_iam_member" "uploads_admin" {
  bucket = google_storage_bucket.uploads.name
  role   = "roles/storage.objectAdmin"
  member = "serviceAccount:${google_service_account.gcs_service_account.email}"
}

resource "google_storage_bucket_iam_member" "streams_admin" {
  bucket = google_storage_bucket.streams.name
  role   = "roles/storage.objectAdmin"
  member = "serviceAccount:${google_service_account.gcs_service_account.email}"
}

resource "google_storage_bucket_iam_member" "torrents_admin" {
  bucket = google_storage_bucket.torrents.name
  role   = "roles/storage.objectAdmin"
  member = "serviceAccount:${google_service_account.gcs_service_account.email}"
}

# Create service account key
resource "google_service_account_key" "gcs_key" {
  service_account_id = google_service_account.gcs_service_account.name
}
