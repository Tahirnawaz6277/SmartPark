using System.Text.Json;

namespace SmartPark.Common.Wrapper
{
    public class ErrorDetails
    {
        public int StatusCode { get; set; }
        public string? Message { get; set; }

        //public string Detail { get; set; } // stack track
        public string? Path { get; set; }
        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
