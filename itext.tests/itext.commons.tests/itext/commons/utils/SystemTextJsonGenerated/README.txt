This folder contains auto generated code by System.Text.Json library. 
Generated code was embedded with small adjustements to let .NET 3.1 compile it.

To re-create files do the following (was done in Rider):
1. Make sure that .NET 6+ is used in your IDE.
2. Remove old auto generated code.
3. Add JsonSerializable annotations to classes which extends JsonSerializerContext.
4. Run locally tests to make sure that annotations were added correctly.
Example for LicenseFile from licensekey
		[JsonSerializable(typeof(LicenseFile[]))]
		[JsonSerializable(typeof(LicenseFile))]
		[JsonSerializable(typeof(Licensee))]
		[JsonSerializable(typeof(OnExpirationStrategy))]
		[JsonSerializable(typeof(Platform))]
		[JsonSerializable(typeof(LicenseType))]
		[JsonSerializable(typeof(DeploymentType))]
		[JsonSerializable(typeof(ProductVersion))]
		[JsonSerializable(typeof(IDictionary<String, Limit>))]
		[JsonSerializable(typeof(EventReporting))]
		[JsonSerializable(typeof(IDictionary<String, String>))]
		[JsonSerializable(typeof(SendStatistics))]
		public partial class LicenseFileSerializationContext : JsonSerializerContext
		{
		}
5. Open each usage of all `...SerializationContext` classes to force IDE locally generate code.
6. In IDE open a folder of any file where `...SerializationContext` is used, you should see all opened on previous step generated classes.
7. Copy paste generated `.cs` files to that folder.
8. Remove JsonSerializable from 3rd step.
9. In each file, remove all `static` excpet comments and method declarations to achieve .NET 3.1 compatibility.
10. Code should be compilable on .NET 3.1.