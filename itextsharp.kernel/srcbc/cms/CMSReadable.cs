using System;
using System.IO;

namespace Org.BouncyCastle.Cms
{
	internal interface CmsReadable
	{
		Stream GetInputStream();
	}
}
