using System.Threading.Tasks;
using MimeKit;

namespace WikiLeaks.Abstract {
    public interface IEmailCache{
        void Delete(int documentNo);
        Task<MimeMessage> GetMimeMessageAsync(int documentNo);
    }
}
