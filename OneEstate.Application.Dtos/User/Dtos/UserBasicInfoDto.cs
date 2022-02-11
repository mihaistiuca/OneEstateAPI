
namespace OneEstate.Application.Dtos
{
    public class UserBasicInfoDto
    {
        public string Id { get; set; }

        public string Email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string ProfileImageUrl { get; set; }


        public string Token { get; set; }

        public bool IsAdmin { get; set; }


        public string PhoneNumber { get; set; }

        // validated/notvalidated/inprogress/failed
        public string PhoneNumberValidationStatus { get; set; }

        // validated/notvalidated/inprogress/failed
        public string UserIdValidationStatus { get; set; }
    }
}
