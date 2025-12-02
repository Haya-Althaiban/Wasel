using System.ComponentModel.DataAnnotations;
using Wasel.Models;

namespace Wasel.ViewModels.MessageVMs
{
    #region Message Index

    public class MessageIndexViewModel
    {
        public List<ConversationViewModel> Conversations { get; set; } = new List<ConversationViewModel>();
        public List<MessageItemViewModel> Messages { get; set; } = new List<MessageItemViewModel>();
        public User SelectedConversation { get; set; }
        public int? SelectedConversationId { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public bool HasPreviousPage { get; set; }
        public bool HasNextPage { get; set; }
    }

    public class ConversationViewModel
    {
        public int PartnerId { get; set; }
        public User Partner { get; set; }
        public Wasel.Models.Message LastMessage { get; set; }
        public int MessageCount { get; set; }
        public int UnreadCount { get; set; }
        public DateTime LastMessageDate { get; set; }
    }

    public class MessageItemViewModel
    {
        public int MessageNum { get; set; }
        public string MessageText { get; set; }
        public DateTime SentDate { get; set; }
        public bool IsRead { get; set; }
        public DateTime? ReadAt { get; set; }
        public bool IsSentByCurrentUser { get; set; }
        public string SenderName { get; set; }
        public string ReceiverName { get; set; }
    }

    #endregion

    #region Send Message

    public class SendMessageViewModel
    {
        [Required(ErrorMessage = "Receiver is required")]
        [Display(Name = "Send To")]
        public int ReceiverId { get; set; }

        [Required(ErrorMessage = "Message text is required")]
        [StringLength(1000, MinimumLength = 1, ErrorMessage = "Message must be between 1 and 1000 characters")]
        [Display(Name = "Message")]
        public string MessageText { get; set; }
    }

    #endregion
}
