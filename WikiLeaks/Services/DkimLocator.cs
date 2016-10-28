using System;
using System.ComponentModel.Composition;
using System.Threading;
using MimeKit.Cryptography;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;

namespace WikiLeaks.Services {

    [Export(typeof(IDkimPublicKeyLocator))]
    public class DkimLocator : IDkimPublicKeyLocator {

        public AsymmetricKeyParameter LocatePublicKey(string methods, string domain, string selector, CancellationToken cancellationToken = new CancellationToken()) {

            var dnsLookup = $"{selector}._domainkey.{domain}";

            var publicKey = GetPublicKey(dnsLookup);

            return PublicKeyFactory.CreateKey(Convert.FromBase64String(publicKey));
        }

        public string GetPublicKey(string dnsLookup){
            switch (dnsLookup) {

                // v=DKIM1; k=rsa;
                case "google._domainkey.hillaryclinton.com":
                    return @"MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQCJdAYdE2z61YpUMFqFTFJqlFomm7C4Kk97nzJmR4YZuJ8SUy9CF35UVPQzh3EMLhP+yOqEl29Ax2hA/h7vayr/f/a19x2jrFCwxVry+nACH1FVmIwV3b5FCNEkNeAIqjbY8K9PeTmpqNhWDbvXeKgFbIDwhWq0HP2PbySkOe4tTQIDAQAB";

                // k=rsa;
                case "20120113._domainkey.gmail.com":
                    return @"MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEA1Kd87/UeJjenpabgbFwh+eBCsSTrqmwIYYvywlbhbqoo2DymndFkbjOVIPIldNs/m40KF+yzMn1skyoxcTUGCQs8g3FgD2Ap3ZB5DekAo5wMmk4wimDO+U8QzI3SD07y2+07wlNWwIt8svnxgdxGkVbbhzY8i+RQ9DpSVpPbF7ykQxtKXkv/ahW3KjViiAH+ghvvIhkx4xYSIc9oSwVmAl5OctMEeWUwg8Istjqz8BZeTWbf41fbNhte7Y+YqZOwq1Sd0DbvYAD9NOZK9vlfuac0598HY+vtSBczUiKERHv1yRbcaQtZFh5wtiRrN04BLUTD21MycBX5jYchHjPY/wIDAQAB";

                // k=rsa;
                case "q20140121._domainkey.comcast.net":
                    return @"MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAwuqXbtRtZLMYtQytThkLGpx7SOPdbO8dEKYHHIvcYLd1c7C8tMJjIuxSnNgA2+W6g6WtPJjr/Af2yqmHn3AKOdaPzp+Wx/kDNoGQDyO98OO1/0e+W1MXOWHAkLJe6/eHx7rEp0gNXU1b16WvhiLWQmr3bekPPfJvIOsrW8HeQrA2RX8Eg5a2HAnQ0jfPB1bzpFN8EjWlrP0ISQhC4X2/UQy+3Fi8yLjVzEqiMPhowI4ndC8wWG0jPuPL0X02SPmCar4yjlh7zrd7x9Hix+Eknz1bqQVms15n2iylcd2EluckeMmvAFnIZXiGnckzTVPq4ouOkt6UJZVxcBikzJEFxwIDAQAB";

                // v=DKIM1; r=postmaster; g=*; k=rsa
                case "dccckey._domainkey.dccc.org":
                    return @"MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQCy+xh45ea5Iiwm1T7TzpuA3IRe4imyJC5w+S1yOZjCQglI6KMCIdp5RHe03L44E9AUizK3VPcGHZpVCUn2h3QxCy/7/NlyGpQziRYHfgAjwyekZhF9gzCC4RbRBV2jc5L7fU/glUH9rY7XHXSLQLDfibO0kuGElzJU+Iu2lBWUeQIDAQAB";

                // k=rsa 
                case "1000073432._domainkey.auth.ccsend.com":
                    return @"MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQCPQbUA/GI9q/nSI59sTfgzf8ZFlgdOKZ8IPOmPEUhgbAeqgv02OMiH4WEV5sOXURfUdD+CJ4usxumlo7tseSr3Zvpr98XjDjGnaA4NTG4iRmk7cedcYvv2fbU4iXACS6YI08suYiOuB5ITTi8LTrHHr/tnTia8se7AO+w3b2KvmQIDAQAB";

                // k=rsa 
                case "20120113._domainkey.google.com":
                    return @"MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAp5kQ31/aZDreQqR9/ikNe00ywRvZBFHod6dja+Xdui4C1y8SVrkUMQQLOO49UA+ROm4evxAru5nGPbSl7WJzyGLl0z8Lt+qjGSa3+qxf4ZhDQ2chLS+2g0Nnzi6coUpF8rjuvuWHWXnzpvLxE5TQdfgp8yziNWUqCXG/LBbgeGqCIpaQjlaA6GtPbJbh0jl1NcQLqrOmc2Kj2urNJAW+UPehVGzHal3bCtnNz55sajugRps1rO8lYdPamQjLEJhwaEg6/E50m58BVVdK3KHvQzrQBwfvm99mHLALJqkFHnhyKARLQf8tQMy8wVtIwY2vOUwwJxt3e0KcIX6NtnjSSwIDAQAB";

                // v=DKIM1; g=*; k=rsa
                case "bsdkey._domainkey.bounce.bluestatedigital.com":
                    return @"MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQDE1rpd/RcC3QhMtKTa2waVqzI9+JoItq0qxNPEywBaN7x64YCwoq010CDGKo17bLvqN8rMKI4gUxiHSV8oFFkDXsnHlyhNBKMzy5DZKcrRh7rS/4RnWX+eLl/HqJ3S6nmgAyDqYOk5vPxp0D/nhKUNoePdDw9mftStozxcTfux6QIDAQAB";

                // v=DKIM1; k=rsa; n=sha1
                case "s1024-dkim._domainkey.mail.salsalabs.net":
                    return @"MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQDaDlqt/YggDxTyRfSo8zpfiNJYaP0+TQksbIIWy9uqpoDf8y5TR5B04zvjvxqlV8OF0KNiWPTkmQ3g/NYxPSQtrJOcdtstVvhEtEr9CxHj8ENQvO/7GIhytNJpiwPNQrG9FWuAtsqE9OOX/22lzYAeYNe2GTRMqlKjYsTarVM0lQIDAQAB";

                // v=DKIM1; k=rsa; n=1024
                case "ngpweb3._domainkey.bounce.myngp.com":
                    return @"MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQD+FZRWRvxNzHH8gasWTJi4+bWRyDSMgEI7XOwAzUyrrvwz4QZ4lDtOwQVAmkqxUiyf5YkufT6+5h15wmR0f82JwqwT1vMjOUNS/Kausds5aBJiu2GFsIFrwXBUFf2Hp81yRzWQ56XoP+QTYJDk7Q3NRRGg17QfOZSDfPZCMICFVwIDAQAB";

                case "selector1-gulaw-onmicrosoft-com._domainkey.gulaw.onmicrosoft.com":
                    return @"MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQCyr0dF0wDuUlztKsCjtL65ITEYNaMD3QoSM8JosRlPsYzTnDM2J6/lU2mnjJNeurQIjehLwQT20L0SfyyE5QWIkLoN3wfac12PXN382S79GpYXgtFG39mYCXN5t/hKhud4qG18ZkJmsDlow7TkQo7z3TITP2gmF3BN6/B61SbpWQIDAQAB";

                case "bdk._domainkey.bronto.com":
                    return @"MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQC6Y8MQmVwMUsVyl9zN6P7s1FYDOJVEpjsK/CexZlsDHuchu730LYEH3CC9sSAAR5Bt6Kws1djfYqfXouxzP1IJDVxrzxKQ+f91lS1RYnh+1wX06nYGZYYXcGMfEKKpASRoo1tJyo5Shz6+pnnN93hNWOe5Rh9Umq56DYwKoFnTPQIDAQAB";

                case "ngpweb1._domainkey.bounce.myngp.com":
                    return @"MFwwDQYJKoZIhvcNAQEBBQADSwAwSAJBAP8GA3mvMMWjkJ9CQTE5115ZuwHUwpKaK/34OjPx6sU4+KV7px+9DQf/zf6LByfPkB6TnrjMgYae1CEkfwgGbPMCAwEAAQ==";

                case "20150623._domainkey.mercedesberk-com.20150623.gappssmtp.com":
                    return @"MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEA2UMfREvlgajdSp3jv1tJ9nLpi/mRYnGyKC3inEQ9a7zqUjLq/yXukgpXs9AEHlvBvioxlgAVCPQQsuc1xp9+KXQGgJ8jTsn5OtKm8u+YBCt6OfvpeCpvt0l9JXMMHBNYV4c0XiPE5RHX2ltI0Av20CfEy+vMecpFtVDg4rMngjLws/ro6qT63S20A4zyVs/V19WW5F2Lulgv+l+EJzz9XummIJHOlU5n5ChcWU3Rw5RVGTtNjTZnFUaNXly3fW0ahKcG5Qc3e0Rhztp57JJQTl3OmHiMR5cHsCnrl1VnBi3kaOoQBYsSuBm+KRhMIw/X9wkLY67VLdkrwlX3xxsp6wIDAQAB";

                case "20120806._domainkey.googlegroups.com":
                    return @"MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAzb2fhKQxJYmlF+PNnOExrd8kRMlV2b/GBb1mw4vpTGDVS8wD+6k8TEXSSsaS2B4uOrOfKBWBb6lMVbVmi/zy3Jc+YP5XkEt09UtXm4HWeAqgu0arqCmjH6yhbUGlPipIqVQMmWy5jnWJsHioAAN8G5S5t5qrCRzxv7ntDOOUhwEPCIIrfncOgBzF4XdJPiuanUNOX5Jw5Q2H3wcOmBTKQ3t0ETvPYK/cqpe7rJ+4L06+QKE2kk/WDuHuxtSZbZUo2U6kM56CGxdvBiNRfPLoMFnMddCQqXYJsJZJHfwBnLQxFwbkZS0idkSWLf8AUKbB0lVWQe4+F0M1CeOj8YimmQIDAQAB";

                case "selector1._domainkey.mta-poolbbcs02.cluster3.convio.net":
                    return @"MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQCfRB/KQvEh8nkrIkbyeN5OXCrLUWa8yUk30YZKC/5sobLRo2eSTG4kHK5tBByLiHo4pnk0SNd5WDdX4kQT+O5+NRLdettms3R+PKhTlcKt9bvI3dNK/taV2oaFTV0PJay9pOUSG1hhJ5tae6slI96kW1vWrIDmhIeCCIcV4Tn5bwIDAQAB";

                case "s2048._domainkey.yahoo.com":
                    return @"MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAuoWufgbWw58MczUGbMv176RaxdZGOMkQmn8OOJ/HGoQ6dalSMWiLaj8IMcHC1cubJx2gziAPQHVPtFYayyLA4ayJUSNk10/uqfByiU8qiPCE4JSFrpxflhMIKV4bt+g1uHw7wLzguCf4YAoR6XxUKRsAoHuoF7M+v6bMZ/X1G+viWHkBl4UfgJQ6O8F1ckKKoZ5KqUkJH5pDaqbgs+F3PpyiAUQfB6EEzOA1KMPRWJGpzgPtKoukDcQuKUw9GAul7kSIyEcizqrbaUKNLGAmz0elkqRnzIsVpz6jdT1/YV5Ri6YUOQ5sN5bqNzZ8TxoQlkbVRy6eKOjUnoSSTmSAhwIDAQAB";

                case "k1._domainkey.mail49.wdc01.mcdlv.net":
                    return @"MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQDbNrX2cY/GUKIFx2G/1I00ftdAj713WP9AQ1xir85i89sA2guU0ta4UX1Xzm06XIU6iBP41VwmPwBGRNofhBVR+e6WHUoNyIR4Bn84LVcfZE20rmDeXQblIupNWBqLXM1Q+VieI/eZu/7k9/vOkLSaQQdml4Cv8lb3PcnluMVIhQIDAQAB";

                case "key1024._domainkey.birthdayalarm.com":
                    return @"MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQDlJ84dZvbOtcAMouetNcKUXI/UrzThX7OBt3b480ntOwRkZb8yuVivGnvh0HElq4uxQAGCCNNYkLMW4XkNtNvk3c22Xx++rL4ncDchD0RDVEzPptzUfC9C5wu1TaPAqWvuMRhfkQfJZojsMIJ5NwiDI8faSYByIWqXFyH/KiUf/wIDAQAB";

                // Non existant domains
                case "beta._domainkey.googlegroups.com":
                case "smtpapi._domainkey.email.nationbuilder.com":
                case "gamma._domainkey.gmail.com":
                    throw new InvalidOperationException("The public key doesn't exist");

                default:
                    throw new InvalidOperationException("The DNS hasn't been looked up yet.");
            }
        }
    }
}
