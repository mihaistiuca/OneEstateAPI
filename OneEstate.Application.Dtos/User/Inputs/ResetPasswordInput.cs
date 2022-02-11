
namespace OneEstate.Application.Dtos
{
    public class ResetPasswordInput
    {
        public string ResetToken { get; set; }

        public string NewPassword { get; set; }
    }
}
