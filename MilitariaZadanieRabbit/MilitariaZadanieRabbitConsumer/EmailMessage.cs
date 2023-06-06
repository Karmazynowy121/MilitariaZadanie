

namespace Rabbit.Producer.Model
{
    [Serializable]
    public class EmailMessage
    {
        public string To { get; set; }
        public string From { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public EmailType Type { get; set; }
    }
    public enum EmailType
    {
        Smtp,
        //other types...
    }
}
