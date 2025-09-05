namespace SmartPark.Dtos.UserDtos
{
    public record UpdateUserRequest
    {
        public string Name { get; set; } = null!;

        public string Address { get; set; }

        public string PhoneNumber { get; set; }

        public string City { get; set; }

        public string Email { get; set; } = null!;

    }
}
