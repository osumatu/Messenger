namespace MessengerBase.Models
{
    public class Chat
    {
        public User client1 { get; set; }
        public User client2 { get; set; }
        public string chatHistory { get; set; }
        public bool isActive { get; set; }
        public Chat() { }
        public Chat(User client1, User client2)
        {
            this.client1 = client1;
            this.client2 = client2;
            this.chatHistory = string.Empty;
            this.isActive = false;
        }
    }
}
