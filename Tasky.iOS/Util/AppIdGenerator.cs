using System;
using MonoTouch.UIKit;

namespace Tasky.Util
{
	public class AppIdGenerator : IAppIdGenerator
	{
		public string GenerateAppId(bool usingPhoneId = false, string prefix = null, string suffix = null)
		{
			var appId = "";

			if (!string.IsNullOrEmpty(prefix))
				appId += prefix;

			appId += Guid.NewGuid().ToString();

			if (usingPhoneId)
				appId += PhoneId;

			if (!string.IsNullOrEmpty(suffix))
				appId += suffix;

			return appId;
		}

		public string PhoneId
		{
			get
			{
				// iOS 6 and up
				return UIDevice.CurrentDevice.IdentifierForVendor.AsString();
			}
		}

		public string PhoneModel
		{
			get
			{
				return UIDevice.CurrentDevice.Model;
			}
		}

		public string OsVersion
		{
			get
			{
				return Constants.DeviceType.iOS + " " + UIDevice.CurrentDevice.SystemVersion;
			}
		}

		public string Platform { get { return Constants.DeviceType.iOS; } }
	}
}


