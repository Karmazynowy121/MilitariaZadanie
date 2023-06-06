using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rabbit.Consumer.Model
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
