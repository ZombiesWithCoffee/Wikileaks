using MimeKit;
using WikiLeaks.Enums;

namespace WikiLeaks.Abstract{
    public interface IEmailValidation{
        SignatureValidation ValidateSource(MimeMessage message);
    }
}