using System.Threading.Tasks;
using MimeKit;

namespace WikiLeaks.Abstract {

    public interface IEmailOnline {
        Task<MimeMessage> GetMimeMessageAsync(int documentNo);
    }
}
