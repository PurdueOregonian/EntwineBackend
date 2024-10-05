using Friends5___Backend.Authentication;
using Friends5___Backend.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace Friends5___Backend_Tests
{
    public class AuthServiceTests
    {
        private readonly Mock<IConfiguration> _configuration = new();
        private readonly Mock<UserManager<IdentityUser>> _userManager = new Mock<UserManager<IdentityUser>>(
            new Mock<IUserStore<IdentityUser>>().Object,
            new Mock<IOptions<IdentityOptions>>().Object,
            new Mock<IPasswordHasher<IdentityUser>>().Object,
            new IUserValidator<IdentityUser>[0],
            new IPasswordValidator<IdentityUser>[0],
            new Mock<ILookupNormalizer>().Object,
            new Mock<IdentityErrorDescriber>().Object,
            new Mock<IServiceProvider>().Object,
            new Mock<ILogger<UserManager<IdentityUser>>>().Object);
        private readonly AuthService _authService;
        private readonly ValidLoginInfo loginInfo = new ValidLoginInfo { Username = "legit", Password = "legitPassword" };
        private readonly TokenBlacklist tokenBlacklist = new TokenBlacklist();

        public AuthServiceTests() {
            var inMemorySettings = new Dictionary<string, string?> {
                {"Jwt:Key", "SomeFakeKey"}
            };
            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            _authService = new AuthService(configuration, _userManager.Object, tokenBlacklist);
        }

        [Fact]
        public async Task RegisterUser()
        {
            _userManager.Setup(u => u.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>())).Returns(Task.FromResult(IdentityResult.Success));

            var result = await _authService.RegisterUser(loginInfo);

            Assert.True(result.Succeeded);
            _userManager.Verify(u => u.CreateAsync(It.Is<IdentityUser>(u => u.UserName == loginInfo.Username), loginInfo.Password));
        }

        [Fact]
        public async Task RegisterUser_Fails()
        {
            _userManager.Setup(u => u.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>())).Returns(Task.FromResult(IdentityResult.Failed()));

            var result = await _authService.RegisterUser(loginInfo);

            Assert.False(result.Succeeded);
        }

        [Fact]
        public async Task RegisterUser_Exception()
        {
            _userManager.Setup(u => u.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>())).Throws(new Exception());

            var result = await _authService.RegisterUser(loginInfo);

            Assert.False(result.Succeeded);
        }

        [Fact]
        public async Task Login()
        {
            var user = new IdentityUser();
            _userManager.Setup(u => u.FindByNameAsync(It.IsAny<string>())).Returns(Task.FromResult(user)!);
            _userManager.Setup(u => u.CheckPasswordAsync(user, It.IsAny<string>())).Returns(Task.FromResult(true));

            var result = await _authService.Login(loginInfo);

            Assert.True(result.Success);
        }

        [Fact]
        public async Task Login_NoSuchUser()
        {
            _userManager.Setup(u => u.FindByNameAsync(It.IsAny<string>())).Returns(Task.FromResult<IdentityUser?>(null));
            _userManager.Setup(u => u.CheckPasswordAsync(It.IsAny<IdentityUser>(), It.IsAny<string>())).Returns(Task.FromResult(true));

            var result = await _authService.Login(loginInfo);

            Assert.False(result.Success);
        }

        [Fact]
        public async Task Login_WrongPassword()
        {
            var user = new IdentityUser();
            _userManager.Setup(u => u.FindByNameAsync(It.IsAny<string>())).Returns(Task.FromResult(user)!);
            _userManager.Setup(u => u.CheckPasswordAsync(user, It.IsAny<string>())).Returns(Task.FromResult(false));

            var result = await _authService.Login(loginInfo);

            Assert.False(result.Success);
        }

        [Fact]
        public async Task Login_Exception()
        {
            var user = new IdentityUser();
            _userManager.Setup(u => u.FindByNameAsync(It.IsAny<string>())).Throws(new Exception());
            _userManager.Setup(u => u.CheckPasswordAsync(user, It.IsAny<string>())).Returns(Task.FromResult(false));

            var result = await _authService.Login(loginInfo);

            Assert.False(result.Success);
        }
    }
}
