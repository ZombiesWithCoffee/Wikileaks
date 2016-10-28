using System;
using System.Threading;
using MimeKit.Cryptography;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;

namespace WikiLeaks.Services {

    public class DkimLocator : IDkimPublicKeyLocator {

        public AsymmetricKeyParameter LocatePublicKey(string methods, string domain, string selector, CancellationToken cancellationToken = new CancellationToken()) {

            var dnsLookup = $"{selector}._domainkey.{domain}";

            var publicKey = GetPublicKey(dnsLookup);

            if (publicKey == null)
                return null;

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
                    return "MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQD+FZRWRvxNzHH8gasWTJi4+bWRyDSMgEI7XOwAzUyrrvwz4QZ4lDtOwQVAmkqxUiyf5YkufT6+5h15wmR0f82JwqwT1vMjOUNS/Kausds5aBJiu2GFsIFrwXBUFf2Hp81yRzWQ56XoP+QTYJDk7Q3NRRGg17QfOZSDfPZCMICFVwIDAQAB";

                // Non existant domains
                case "beta._domainkey.googlegroups.com":
                case "smtpapi._domainkey.email.nationbuilder.com":
                case "gamma._domainkey.gmail.com":
                    return null;

                default:
                    return null;
            }
        }
    }
}
