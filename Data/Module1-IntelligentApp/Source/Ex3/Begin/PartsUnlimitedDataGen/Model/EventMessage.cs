namespace PartsUnlimitedDataGen
{
    public class EventMessage
    {
        public string EventDate { get; set; }
        public string UserId { get; set; }
        public string Type { get; set; }

        public string ProductId { get; set; }
        public int quantity { get; set; }

        public decimal Price { get; set; } 

        public override string ToString()
        {
            if(Type.Equals("checkout"))
                return string.Format("**{0}*{1}*{2}*{3}*{4}*{5}**", EventDate, UserId, Type, ProductId, quantity, Price);
            else
                return string.Format("**{0}*{1}*{2}*{3}**", EventDate, UserId, Type, ProductId);
        }
    }
}
