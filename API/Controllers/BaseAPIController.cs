using System;
using API.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ServiceFilter(typeof(LogUserActivity))] //this attribute is used to log the user activity
[ApiController]
[Route("api/[controller]")]
public class BaseAPIController : ControllerBase
{

}
