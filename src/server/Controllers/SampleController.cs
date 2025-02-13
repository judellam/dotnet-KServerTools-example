using System.Diagnostics;
using KServerTools.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using server.Models;

namespace server.Controllers;

[ApiController]
[Route("sample")]
public class SampleController(IJsonLogger logger) : ControllerBase {
    private readonly IJsonLogger logger = logger;

    /// <summary>
    /// Does nothing fancy, just requires a valid JWT token.
    /// </summary>
    [HttpGet]
    [Authorize]
    public ActionResult<SampleResponse> Get() {
        Stopwatch stopwatch = Stopwatch.StartNew();
        try {
            return Ok(new SampleResponse("Much success, very nice!"));
        } catch (Exception ex) {
            return BadRequest(new SampleResponse(ex.Message));
        } finally {
            this.logger.Info("End sample request", stopwatch.ElapsedMilliseconds);
        }
    }
}