using System;
using Android.OS;
using Android.Provider;
using Android.App;

namespace Tasky.Util
{
	public class AppIdGenerator: IAppIdGenerator
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
				var serial = "";
				try
				{
					// Android 2.3 and up (API 10)
					serial = Build.Fingerprint;    
				}
				catch(Exception) {}

				var androidId = "";
				try
				{
					// Not 100% reliable on 2.2 (API 8)
					androidId = Settings.Secure.GetString(Application.Context.ContentResolver, Settings.Secure.AndroidId);
				}
				catch(Exception) {}

				return serial + androidId;
			}
		}

		public string PhoneModel
		{
			get
			{
				return Build.Model;
			}
		}

		public string OsVersion
		{
			get
			{
				return Constants.DeviceType.Android + " " + Build.VERSION.Release;
			}
		}

		public string Platform { get { return Constants.DeviceType.Android; } }
	}
}


