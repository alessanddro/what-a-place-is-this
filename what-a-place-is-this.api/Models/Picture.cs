namespace what_a_place_is_this.api.Models
{
    public class Picture
    {
        public bool Active { get; set; } = false;
        public bool Validated { get; set; } = false;
        public string PostedBy { get; set; }
        public string Path { get; set; }
    }
}
