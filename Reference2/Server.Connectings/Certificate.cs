using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Operators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Prng;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
using Org.BouncyCastle.X509.Extension;

namespace Server.Connectings;

internal class Certificate
{
	public const string certificate_name = "ServerCertificate.p12";

	public static X509Certificate2 certificate { get; private set; }

	public static bool Imported { get; private set; }

	public static void Import()
	{
		if (File.Exists("ServerCertificate.p12"))
		{
			certificate = new X509Certificate2("ServerCertificate.p12");
			Imported = true;
		}
	}

	public static X509Certificate2 CreateCertificateAuthority(string caName)
	{
		int strength = 4096;
		SecureRandom random = new SecureRandom(new CryptoApiRandomGenerator());
		RsaKeyPairGenerator rsaKeyPairGenerator = new RsaKeyPairGenerator();
		rsaKeyPairGenerator.Init(new KeyGenerationParameters(random, strength));
		AsymmetricCipherKeyPair asymmetricCipherKeyPair = rsaKeyPairGenerator.GenerateKeyPair();
		X509V3CertificateGenerator x509V3CertificateGenerator = new X509V3CertificateGenerator();
		X509Name x509Name = new X509Name("CN=" + caName);
		BigInteger serialNumber = BigInteger.ProbablePrime(120, random);
		x509V3CertificateGenerator.SetSerialNumber(serialNumber);
		x509V3CertificateGenerator.SetSubjectDN(x509Name);
		x509V3CertificateGenerator.SetIssuerDN(x509Name);
		x509V3CertificateGenerator.SetNotAfter(DateTime.UtcNow.AddYears(10));
		x509V3CertificateGenerator.SetNotBefore(DateTime.UtcNow.Subtract(new TimeSpan(2, 0, 0, 0)));
		x509V3CertificateGenerator.SetPublicKey(asymmetricCipherKeyPair.Public);
		x509V3CertificateGenerator.AddExtension(X509Extensions.SubjectKeyIdentifier, critical: false, new SubjectKeyIdentifierStructure(asymmetricCipherKeyPair.Public));
		x509V3CertificateGenerator.AddExtension(X509Extensions.BasicConstraints, critical: true, new BasicConstraints(cA: true));
		ISignatureFactory signatureCalculatorFactory = new Asn1SignatureFactory("SHA512WITHRSA", asymmetricCipherKeyPair.Private, random);
		X509Certificate2 x509Certificate = new X509Certificate2(DotNetUtilities.ToX509Certificate(x509V3CertificateGenerator.Generate(signatureCalculatorFactory)));
		x509Certificate.PrivateKey = DotNetUtilities.ToRSA(asymmetricCipherKeyPair.Private as RsaPrivateCrtKeyParameters);
		File.WriteAllBytes("ServerCertificate.p12", x509Certificate.Export(X509ContentType.Pfx));
		Imported = true;
		return x509Certificate;
	}
}
