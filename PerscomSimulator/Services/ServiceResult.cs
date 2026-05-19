namespace Perscom.Services;

/// <summary>
/// A generic wrapper for service layer operations that encapsulates success/failure state,
/// a human-readable message, and the resulting data payload.
/// </summary>
/// <typeparam name="T">The type of data returned on success.</typeparam>
public class ServiceResult<T>
{
    /// <summary>
    /// Indicates whether the operation completed without errors.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// A human-readable summary of what happened (e.g., "3 Rank(s) created.").
    /// Populated on success; may be null on failure.
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// A human-readable error description. Populated only when <see cref="Success"/> is false.
    /// </summary>
    public string Error { get; set; }

    /// <summary>
    /// The operation's result payload. The shape of <typeparamref name="T"/> is determined
    /// by each service method — it could be a single entity, a summary object, or a list.
    /// </summary>
    public T Data { get; set; }

    /// <summary>
    /// Creates a successful result containing <paramref name="data"/> and an optional message.
    /// </summary>
    public static ServiceResult<T> Ok(T data, string message = null)
        => new() { Success = true, Data = data, Message = message };

    /// <summary>
    /// Creates a failed result with the given error description.
    /// </summary>
    public static ServiceResult<T> Fail(string error)
        => new() { Success = false, Error = error };
}