using System.Threading.Tasks;
using UserManagement.Models.TokenAuth;
using UserManagement.Web.Controllers;
using Shouldly;
using Xunit;

namespace UserManagement.Web.Tests.Controllers
{
    public class HomeController_Tests: UserManagementWebTestBase
    {
        [Fact]
        public async Task Index_Test()
        {
            await AuthenticateAsync(null, new AuthenticateModel
            {
                UserNameOrEmailAddress = "admin",
                Password = "123qwe"
            });

            //Act
            var response = await GetResponseAsStringAsync(
                GetUrl<HomeController>(nameof(HomeController.Index))
            );

            //Assert
            response.ShouldNotBeNullOrEmpty();
        }
    }
}