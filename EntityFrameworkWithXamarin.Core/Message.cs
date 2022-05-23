using System;
using System.Collections.Generic;
using System.Text;

namespace EntityFrameworkWithXamarin.Core
{
    public class Message
    {
        public int Id { get; set; }
        public long ChatId { get; set; }
        public string Name { get; set; }
        public long MessageId { get; set; }

    }
}
