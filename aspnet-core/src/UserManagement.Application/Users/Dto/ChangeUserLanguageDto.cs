using System.ComponentModel.DataAnnotations;

namespace UserManagement.Users.Dto
{
    public class ChangeUserLanguageDto
    {
        [Required]
        public string LanguageName { get; set; }
    }
}