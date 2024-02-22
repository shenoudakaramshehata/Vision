using Newtonsoft.Json;

namespace Vision.Models
{
    public class ReelResult
    {
            public long ClassifiedAdId { get; set; }
            public DateTime PublishDate { get; set; }
            public bool IsActive { get; set; }
            public int ClassifiedAdsCategoryId { get; set; }
            public string Description { get; set; }
            public string MainPic { get; set; }
            public string PhoneNumber { get; set; }
            public string UseId { get; set; }
            public string Reel { get; set; }
            public string UserName { get; set; }
            public string FullName { get; set; }
            public string ProfilePicture { get; set; }
    }
}