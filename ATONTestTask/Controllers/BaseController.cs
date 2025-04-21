using ATONTestTask.Filters;
using Microsoft.AspNetCore.Mvc;

namespace ATONTestTask.Controllers
{
    [TypeFilter<ApiExceptionFilter>]
    public class BaseController : ControllerBase
    {

    }
}
