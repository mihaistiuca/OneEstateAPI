using OneEstate.Domain.Entities.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace OneEstate.Domain.Entities
{
    public class User : EntityBase
    {
        public string Email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public byte[] PasswordHash { get; set; }

        public byte[] PasswordSalt { get; set; }


        public DateTime ActivationDate { get; set; }

        public string ActivationCode { get; set; }

        // for reset password
        public DateTime ResetDate { get; set; }

        public string ResetToken { get; set; }

        public bool IsResetTokenActive { get; set; }


        public bool IsActive { get; set; }


        public List<string> InvestmentsIds { get; set; } = new List<string>();

        // ---------------------------------
        // to be validated info

        public string PhoneNumber { get; set; }

        // validated/notvalidated/inprogress/failed
        public string PhoneNumberValidationStatus { get; set; }

        public string IdImageId { get; set; }

        // validated/notvalidated/inprogress/failed
        public string UserIdValidationStatus { get; set; }

        // ---------------------------------

    }
}
