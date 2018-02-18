using System;
using System.ComponentModel.DataAnnotations;

namespace SandwichDeliveryBot3.CustomClasses
{
    public class Quote
    {
        public string QuoteContent { get; set; }
        public DateTime QuoteDate { get; set; }
        public ulong Creator { get; set; }
        [Key]
        public int QuoteNum { get; set; }

        public Quote()
        { }
    }
}
