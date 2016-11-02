using System.Collections.Generic;
using System.Threading.Tasks;
using WikiLeaks.Models;

namespace WikiLeaks.Abstract {
    public interface IAttachmentHistory {
        List<Document> Initialize();
        Task<List<Document>> RefreshAsync();
    }
}
