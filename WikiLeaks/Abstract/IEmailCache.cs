using System.Threading.Tasks;
using MimeKit;

namespace WikiLeaks.Abstract {
    public interface IEmailCache {
        Task<MimeMessage> GetMimeMessageAsync(int documentNo);
    }
}
