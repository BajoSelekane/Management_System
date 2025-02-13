namespace BaseLibrary.Entities
{
    public class User : BaseEntity
    {
        public string? UserName { get; set; }
        public string? EmailAddress { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }

        

    }

}