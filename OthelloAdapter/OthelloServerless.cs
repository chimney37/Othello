using System;

namespace OthelloAWSServerless
{
    public class OthelloGameRepresentation
    {
        public string Id { get; set; }
        public string OthelloGameStrRepresentation { get; set; }
        public DateTime CreatedTimestamp { get; set; }
    }
}
