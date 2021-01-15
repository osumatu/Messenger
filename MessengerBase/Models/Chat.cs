namespace MessengerBase.Models
{
    public class Chat
    {
        public User Client1 { get; set; }
        public User Client2 { get; set; }
        public string ChatHistory { get; set; }
        public bool IsActive { get; set; }
        public Chat() { }
        public Chat(User client1, User client2)
        {
            this.Client1 = client1;
            this.Client2 = client2;
            this.ChatHistory = string.Empty;
            this.IsActive = false;
        }
    }
}
