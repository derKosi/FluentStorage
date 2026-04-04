using FluentStorage.Blobs;
using FluentStorage.ConnectionString;
using FluentStorage.Messaging;

namespace FluentStorage.Gcp.CloudStorage {
	class Module : IExternalModule, IConnectionFactory {
		public IConnectionFactory ConnectionFactory => new Module();

		public IBlobStorage CreateBlobStorage(StorageConnectionString connectionString) {
			if (connectionString.Prefix == "google.storage") {
				connectionString.GetRequired("bucket", true, out string bucketName);
				string base64EncodedJson = connectionString.Get("cred");

				// When cred= is absent or empty, fall back to Application Default Credentials
				// (Workload Identity on Cloud Run, gcloud auth application-default login locally)
				if (string.IsNullOrEmpty(base64EncodedJson))
					return StorageFactory.Blobs.GoogleCloudStorageFromEnvironmentVariable(bucketName);

				return StorageFactory.Blobs.GoogleCloudStorageFromJson(bucketName, base64EncodedJson, true);
			}

			return null;
		}

		public IMessenger CreateMessenger(StorageConnectionString connectionString) => null;
	}
}
