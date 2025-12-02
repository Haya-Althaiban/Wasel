namespace Wasel.ViewModels.SellerVMs.MessageVMs
{
    public class SellerMessageItemViewModel
    {
        public int MessageNum { get; set; }
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public string SenderName { get; set; }
        public string ReceiverName { get; set; }
        public string MessageText { get; set; }
        public DateTime SentDate { get; set; }
        public bool IsRead { get; set; }
        public bool IsSentByMe { get; set; }
    }
}
