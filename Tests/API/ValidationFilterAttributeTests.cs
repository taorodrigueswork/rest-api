using API;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Tests.API;

[TestClass]
public class ValidationFilterAttributeTests
{
    [TestMethod]
    public void OnActionExecuting_WithInvalidModelState_SetsUnprocessableEntityResult()
    {
        // Arrange
        var attribute = new ValidationFilterAttribute();

        var httpContext = new DefaultHttpContext();

        var actionContext = new ActionContext(httpContext, new(), new(), new());

        var context = new ActionExecutingContext(
            actionContext,
            new List<IFilterMetadata>(),
            new Dictionary<string, object?>(),
            httpContext);

        context.ModelState.AddModelError("FieldName", "Some error message");

        // Act
        attribute.OnActionExecuting(context);

        // Assert
        Assert.IsInstanceOfType(context.Result, typeof(UnprocessableEntityObjectResult));
    }

    [TestMethod]
    public void OnActionExecuting_WithValidModelState_DoesNotSetResult()
    {
        // Arrange
        var attribute = new ValidationFilterAttribute();

        var httpContext = new DefaultHttpContext();

        var actionContext = new ActionContext(httpContext, new(), new(), new());

        var context = new ActionExecutingContext(
            actionContext,
            new List<IFilterMetadata>(),
            new Dictionary<string, object?>(),
            httpContext);

        //Act
        attribute.OnActionExecuting(context);

        Assert.IsNull(context.Result);
    }

    [TestMethod]
    public void OnActionExecuted_DoesNotSetResult()
    {
        // Arrange
        var attribute = new ValidationFilterAttribute();

        var httpContext = new DefaultHttpContext();

        var actionContext = new ActionContext(httpContext, new(), new(), new());

        var context = new ActionExecutedContext(
                 actionContext,
                 new List<IFilterMetadata>(),
                 null);

        //Act
        attribute.OnActionExecuted(context);

        Assert.IsNull(context.Result);
    }
}
