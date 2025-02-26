namespace BookingApp.Domain.Results;

public class Error
{
    private const string NOT_FOUND_CODE = "NotFound";
    private const string BAD_REQUEST_CODE = "BadRequest";
    private const string INTERNAL_SERVER_ERROR_CODE = "InternalServerError";
    private const string VALIDATION_ERROR_CODE = "ValidationError";

    public string Code { get; set; }
    public string Message { get; set; }
    public Error(string code, string message)
    {
        Code = code;
        Message = message;
    }

    public static Error None => new (string.Empty, string.Empty);
    public static Error NotFound(string message) => new(NOT_FOUND_CODE, message);
    public static Error BadRequest(string message) => new(BAD_REQUEST_CODE, message);
    public static Error InternalServer(string message) => new(INTERNAL_SERVER_ERROR_CODE, message);
    public static Error ValidationError(string message) => new(VALIDATION_ERROR_CODE, message);
}