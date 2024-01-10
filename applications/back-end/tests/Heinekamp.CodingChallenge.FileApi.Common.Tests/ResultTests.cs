using FluentAssertions;
using Xunit;

namespace Heinekamp.CodingChallenge.FileApi.Common.Tests;

public class ResultTests
{
    [Fact]
    public void Given_Success_Result_Should_NotHaveErrorMessageOrException()
    {
        var result = Result.Success();

        result.IsSuccess.Should().BeTrue();
        result.ErrorMessage.Should().BeNull();
        result.Exception.Should().BeNull();
    }
    
    [Fact]
    public void Given_FailWithErrorMessage_Result_Should_HaveErrorMessage()
    {
        var errorMessage = "Error message";
        var result = Result.Fail(errorMessage);

        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Be(errorMessage);
        result.Exception.Should().BeNull();
    }
    
    [Fact]
    public void Given_FailWithException_Result_Should_HaveException()
    {
        var errorMessage = "Error message";
        var exceptionMessage = "Exception message";
        var exception = new Exception(exceptionMessage);
        var result = Result.Fail(errorMessage, exception);

        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Be(errorMessage);
        result.Exception.Should().NotBeNull();
        result.Exception.Should().Be(exception);
    }
    
    [Fact]
    public void Given_SuccessWithValue_Result_Should_ReturnValue()
    {
        var value = "value";
        var result = Result<string>.Success(value);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(value);
    }
    
    [Fact]
    public void Given_FailWithValue_Result_Should_ThrowError()
    {
        var errorMessage = "Error message";
        var value = "value";
        var result = Result<string>.Fail(errorMessage, value: value);

        var action = () => { var resultValue = result.Value; };
        
        action.Should().Throw<ArgumentException>()
            .WithMessage("Failed Result does not have a valid Value");
        result.IsSuccess.Should().BeFalse();
    }   
}