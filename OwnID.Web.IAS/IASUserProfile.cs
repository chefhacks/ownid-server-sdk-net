using OwnID.Attributes;
using OwnID.Extensibility.Configuration.Profile;
using System.ComponentModel.DataAnnotations;

namespace OwnID.Web.IAS
{
    class IASUserProfile : IIASUserProfile
    {
        [OwnIdField(Constants.DefaultEmailLabel, Constants.DefaultEmailLabel)]
        [OwnIdFieldType(ProfileFieldType.Email)]
        [Required]
        [MaxLength(200)]
        public string Email { get; set; }

        [OwnIdField(Constants.DefaultFirstNameLabel, Constants.DefaultFirstNameLabel)]
        [MaxLength(200)]
        public string FirstName { get; set; }

        [OwnIdField(Constants.DefaultLastNameLabel, Constants.DefaultLastNameLabel)]
        [MaxLength(200)]
        public string LastName { get; set; }
    }
}
