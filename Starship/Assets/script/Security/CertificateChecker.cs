using UnityEngine;

public static class EncryptedStrings
{
	// 8488d0b0a160552cd4bccec2f5472d88bfef14da5c0a7b92a546dcc38e795c55822cff5b736f09e7c1122f5a1c2d88de8a8fa4eb01f28ac9311074788e710504f2271ce62db499e5b09ffdbc2856c1a627f3b341b1a6b02ab15a9f30d8b96266f680bccf9ab07a4c60f5119570e4d7627496f4038cdc40197b368bfbc2ad2a3f
	public static readonly string Rsa = "A#AAeUTUMxfUdd >e#T>>v> Ld#' eAATLvLx#eMd>UM'T5 Md#fe>>!Av'5d>ddA  >LLdT'!fLU5v'>xx  LdMx> eAAevAMALM#vTUxL AM>5!xxU'#'AAv'xUdU#L  'x>vf eT#55vdTU5LLeT> Adf>xMf 'L!T!#xTxMfTU MTxdM5L!UeAT5f ffLfAUT>>L5MTU'M#>fULdxx5d'Uv#e'f '#5fL#U!A>e>#Ux5'T!fATLT> Me M!L";
	// com.unity3d.player.UnityPlayer
	public static readonly string UnityPlayerClass = ">hb3&q=W(!e3PIM(v?3_q=W(rIM(v?";
	// currentActivity
	public static readonly string CurrentActivity = ">&??vqW+>W=:=W(";
	// getPackageManager
	public static readonly string GetPackageManager = "wvWrM>jMwv8MqMwv?";
	// getPackageName
	public static readonly string GetPackageName = "wvWrM>jMwv[Mbv";
	// getPackageInfo
	public static readonly string GetPackageInfo = "wvWrM>jMwv^qLh";
	// signatures
	public static readonly string Signatures = "}=wqMW&?v}";
	// toByteArray
	public static readonly string ToByteArray = "Wh9(Wv+??M(";
	// java.io.ByteArrayInputStream
	public static readonly string ByteArrayInputStreamClass = ",M:M3=h39(Wv+??M(^qP&W*W?vMb";
	// java.security.cert.CertificateFactory
	public static readonly string CertificateFactoryClass = ",M:M3}v>&?=W(3>v?W3cv?W=L=>MWvSM>Wh?(";
	// getInstance
	public static readonly string GetInstance = "wvW^q}WMq>v";
	// X509
	public static readonly string X509 = "odU5";
	// generateCertificate
	public static readonly string GenerateCertificate = "wvqv?MWvcv?W=L=>MWv";
	// getPublicKey
	public static readonly string GetPublicKey = "wvWr&TI=>%v(";
	// toString
	public static readonly string ObjectToString = "Wh*W?=qw";
	// Invader_pirate
	public static readonly string invaderPirate = "^q:Mev?KP=?MWv";
}

public static class CertificateChecker
{
	public static bool IsValid
	{
		get
		{
            if (_checkResult < 0)
				VerifyCertificate();

			return _checkResult > 0;
		}
	}

	private static void VerifyCertificate()
	{
#if UNITY_ANDROID && !UNITY_EDITOR
		try
		{
			var unityPlayerJavaClass = new JavaClassWrapper(EncryptedStrings.UnityPlayerClass);
			var currentActivity = unityPlayerJavaClass.GetStaticObject(EncryptedStrings.CurrentActivity);
			var packageManager = currentActivity.CallForObject(EncryptedStrings.GetPackageManager);
			var packageName = currentActivity.CallForString(EncryptedStrings.GetPackageName);
			var packageInfo = packageManager.CallForObject(EncryptedStrings.GetPackageInfo, packageName, 0x40);
			var signatures = packageInfo.GetArray(EncryptedStrings.Signatures);
			var signature = signatures[0].Call<byte[]>(EncryptedStrings.ToByteArray);
			var byteArrayInputStream = new JavaObjectWrapper(EncryptedStrings.ByteArrayInputStreamClass, signature);			
			var certificateFactoryJavaClass = new JavaClassWrapper(EncryptedStrings.CertificateFactoryClass);
			var certificateFactory = certificateFactoryJavaClass.CallStaticObject(EncryptedStrings.GetInstance, EncryptedStrings.X509.Decrypt());
			var x509certificate = certificateFactory.CallForObject(EncryptedStrings.GenerateCertificate, byteArrayInputStream.RawObject);
			var publicKey = x509certificate.CallForObject(EncryptedStrings.GetPublicKey);
			var key = publicKey.CallForString(EncryptedStrings.ObjectToString);
			_checkResult = key.Contains(EncryptedStrings.Rsa.Decrypt()) ? 1 : 0;
		}
		catch (System.Exception e)
		{
			Debug.LogException(e);
			_checkResult = 0;
		}
#else
	    _checkResult = 1;
#endif
	}

    private static int _checkResult = -1;
}
