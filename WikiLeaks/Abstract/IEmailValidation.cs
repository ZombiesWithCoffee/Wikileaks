using MimeKit;

namespace WikiLeaks.Abstract{
    public interface IEmailValidation{
        bool? ValidateSource(MimeMessage message);
    }
}