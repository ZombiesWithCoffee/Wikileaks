using System;
using System.ComponentModel.Composition;
using System.Net.Http;
using System.Threading.Tasks;
using MimeKit;
using WikiLeaks.Abstract;
using WikiLeaks.Properties;

namespace WikiLeaks.Services {

    [Export(typeof(IEmailOnline))]
    public class EmailOnline : IEmailOnline {

        public async Task<MimeMessage> GetMimeMessageAsync(int documentNo) {

            string messageUrl = $"https://wikileaks.org/{Settings.Default.Repository}/get/{documentNo}";

            using (var client = new HttpClient()) {
                using (var response = await client.GetAsync(new Uri(messageUrl))) {
                    var stream = await response.Content.ReadAsStreamAsync();

                    var message = MimeMessage.Load(stream);

                    if (message.TextBody != null && message.TextBody.Contains("<p>Bad request or the file you have requested does not exist.</p>"))
                        return null;

                    return message;
                }
            }
        }
    }
}
