using System.Diagnostics;
using KServerTools.Common;
using Microsoft.AspNetCore.Mvc;
using server.Components;
using server.Models;

namespace server.Controllers;

[ApiController]
[Route("user")]
public class UserController(IUserComponent userComponent, IJsonLogger logger) : ControllerBase {
    private readonly IUserComponent userComponent = userComponent;
    private readonly IJsonLogger logger = logger;

    [HttpPost]
    [Route("login")]
    public async Task<ActionResult<UserLoginResponse>> Login([FromBody] User user, CancellationToken cancellationToken) {
        Stopwatch stopwatch = Stopwatch.StartNew();
        try {
            this.logger.Info($"User login attempt: {user.Username}");
            UserLoginResponse userLoginResponse = await this.userComponent.Login(user, cancellationToken);
            return Ok(userLoginResponse);
        } finally {
            this.logger.Info("End login request", stopwatch.ElapsedMilliseconds);
        }
    }

    [HttpPost]
    [Route("register")]
    public async Task<ActionResult<RegisterUser>> Register([FromBody] RegisterUser user, CancellationToken cancellationToken) {
        Stopwatch stopwatch = Stopwatch.StartNew();
        try {
            this.logger.Info($"User registration attempt: {user.Username}");
            RegisterUser registeredUser = await this.userComponent.Register(user, cancellationToken);
            return Ok(registeredUser);
        } finally {
            this.logger.Info("End login request", stopwatch.ElapsedMilliseconds);
        }
    }
}