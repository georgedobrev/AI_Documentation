using DocuAurora.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace DocuAurora.Services.Data.Tests
{
    public class AuthServiceTest : BaseServiceTests
    {
        private Mock<UserManager<ApplicationUser>> _mockUserManager;
        private Mock<IConfiguration> _mockConfiguration;

        public AuthServiceTest()
        {
            _mockUserManager = new Mock<UserManager<ApplicationUser>>(
                new Mock<IUserStore<ApplicationUser>>().Object,
                null, null, null, null, null, null, null, null
            );

            _mockConfiguration = new Mock<IConfiguration>();

            _mockConfiguration.Setup(c => c[It.Is<string>(s => s == "JwtSettings:SecretKey")]).Returns("BestSecretTestSecretKey");
            _mockConfiguration.Setup(c => c[It.Is<string>(s => s == "JwtSettings:Issuer")]).Returns("BestSecretTestIssuer");
            _mockConfiguration.Setup(c => c[It.Is<string>(s => s == "JwtSettings:Audience")]).Returns("BestSecretyourTestAudience");
        }

        [Fact]
        public async Task Test_CreateRefreshToken()
        {
            var user = new ApplicationUser { Id = Guid.NewGuid().ToString() };
            _mockUserManager
                .Setup(um => um.SetAuthenticationTokenAsync(
                    It.IsAny<ApplicationUser>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>())
                )
                .ReturnsAsync(IdentityResult.Success);

            var authService = new AuthService(_mockUserManager.Object, _mockConfiguration.Object);
            var token = await authService.CreateRefreshToken(user);

            Assert.NotNull(token);
            _mockUserManager.Verify(um => um.SetAuthenticationTokenAsync(
                It.IsAny<ApplicationUser>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()), Times.Once);
        }

    }
}
