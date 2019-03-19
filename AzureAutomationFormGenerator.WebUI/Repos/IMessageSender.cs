using System.Threading.Tasks;

namespace AzureAutomationFormGenerator.WebUI.Repos
{
    public interface IMessageSender
    {
        Task SendErrorMessage(string message);
        /// <summary>
        /// Set Job Message value on AzureRunbookForm view instantly using SignalR
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        Task SendMessage(string message);

        /// <summary>
        /// Set Job Status value on AzureRunbookForm view instantly using SignalR
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        Task SendStatus(string status);
    }
}