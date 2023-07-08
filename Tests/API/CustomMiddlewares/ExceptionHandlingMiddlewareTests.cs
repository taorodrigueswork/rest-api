using ExceptionHandling.CustomMiddleware;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Text.Json;

namespace Tests.API.CustomMiddlewares
{
    [TestClass]
    public class ExceptionHandlingMiddlewareTests
    {
        private ExceptionHandlingMiddleware _middleware;
        private HttpContext _httpContext;
        private Mock<RequestDelegate> _nextDelegateMock;
        private Mock<ILogger<ExceptionHandlingMiddleware>> _loggerMock;

        [TestInitialize]
        public void Setup()
        {
            _nextDelegateMock = new Mock<RequestDelegate>();
            _loggerMock = new Mock<ILogger<ExceptionHandlingMiddleware>>();
            _middleware = new ExceptionHandlingMiddleware(_nextDelegateMock.Object, _loggerMock.Object);
            _httpContext = new DefaultHttpContext();
            _httpContext.Response.Body = new MemoryStream();
        }

        [TestMethod]
        public async Task InvokeAsync_ShouldCallNextDelegate()
        {
            // Arrange

            // Act
            await _middleware.InvokeAsync(_httpContext);

            // Assert
            _nextDelegateMock.Verify(next => next.Invoke(_httpContext), Times.Once);
        }

        [TestMethod]
        public async Task InvokeAsync_ShouldHandleApplicationException_WithInvalidToken()
        {
            // Arrange
            var exception = new ApplicationException("Invalid Token");
            _nextDelegateMock.Setup(next => next.Invoke(_httpContext)).Throws(exception);

            // Act
            await _middleware.InvokeAsync(_httpContext);

            // Assert
            AssertErrorResponse(HttpStatusCode.Forbidden, exception.Message);
        }

        [TestMethod]
        public async Task InvokeAsync_ShouldHandleApplicationException_WithoutInvalidToken()
        {
            // Arrange
            var exception = new ApplicationException("Some error");
            _nextDelegateMock.Setup(next => next.Invoke(_httpContext)).Throws(exception);

            // Act
            await _middleware.InvokeAsync(_httpContext);

            // Assert
            AssertErrorResponse(HttpStatusCode.BadRequest, exception.Message);
        }

        [TestMethod]
        public async Task InvokeAsync_ShouldHandleArgumentNullException_WithNotFound()
        {
            // Arrange
            var exception = new ArgumentNullException("Some argument not found");
            _nextDelegateMock.Setup(next => next.Invoke(_httpContext)).Throws(exception);

            // Act
            await _middleware.InvokeAsync(_httpContext);

            // Assert
            AssertErrorResponse(HttpStatusCode.NotFound, exception.Message);
        }

        [TestMethod]
        public async Task InvokeAsync_ShouldHandleArgumentNullException_WithoutNotFound()
        {
            // Arrange
            var exception = new ArgumentNullException("Some argument");
            _nextDelegateMock.Setup(next => next.Invoke(_httpContext)).Throws(exception);

            // Act
            await _middleware.InvokeAsync(_httpContext);

            // Assert
            AssertErrorResponse(HttpStatusCode.BadRequest, exception.Message);
        }

        [TestMethod]
        public async Task InvokeAsync_ShouldHandleOtherExceptions()
        {
            // Arrange
            var exception = new Exception("Some error");
            _nextDelegateMock.Setup(next => next.Invoke(_httpContext)).Throws(exception);

            // Act
            await _middleware.InvokeAsync(_httpContext);

            // Assert
            AssertErrorResponse(HttpStatusCode.InternalServerError, exception.Message);
        }

        private void AssertErrorResponse(HttpStatusCode expectedStatusCode, string expectedErrorMessage)
        {
            _httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
            using var reader = new StreamReader(_httpContext.Response.Body);
            var responseContent = reader.ReadToEnd();
            var jsonResponse = JsonSerializer.Deserialize<string>(responseContent);

            Assert.AreEqual(expectedStatusCode, (HttpStatusCode)_httpContext.Response.StatusCode);
            Assert.AreEqual($@"{expectedErrorMessage}", jsonResponse);
        }
    }
}