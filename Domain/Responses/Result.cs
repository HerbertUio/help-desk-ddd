using System.Net;

namespace Domain.Responses;

public class Result<T>
{
    public T? Data { get; private set; }
    public bool IsSuccess { get; private set; }
    public string Message { get; private set; }
    public List<string> Errors { get; private set; }

    // Propiedad interna opcional para que la API pueda usarla
    // Si quieres independencia TOTAL, podrías eliminar esta propiedad y los constructores/métodos que la usan.
    // Por ahora, la mantenemos para flexibilidad de la capa API.
    public HttpStatusCode StatusCode { get; private set; }

    private Result(T? data, bool isSuccess, HttpStatusCode code, string message, List<string>? errors)
    {
        Data = data;
        IsSuccess = isSuccess;
        StatusCode = code;
        Message = message ?? string.Empty;
        Errors = errors ?? new List<string>();
    }

    public static Result<T> Success(T data, string message)
    {
        return new Result<T>(data, true, HttpStatusCode.OK, message, null);
    }

    public static Result<T> Failure(List<string> errors, string message)
    {
        if (errors == null || !errors.Any())
        {
            errors = new List<string> { message ?? "Ocurrió un error desconocido." };
        }
        return new Result<T>(default, false, HttpStatusCode.BadRequest, message, errors);
    }

    public static Result<T> Failure(string error, string message)
    {
        return new Result<T>(default, false, HttpStatusCode.BadRequest, message, new List<string> { error });
    }

    // --- Métodos Adicionales (Opcional - Usados por API o internamente, no por Application Core) ---

    public static Result<T> Success(T data, HttpStatusCode code, string message)
    {
         return new Result<T>(data, true, code, message, null);
    }

     public static Result<T> Failure(List<string> errors, HttpStatusCode code, string message)
    {
        if (errors == null || !errors.Any())
        {
            errors = new List<string> { message ?? "Ocurrió un error desconocido." };
        }
        return new Result<T>(default, false, code, message, errors);
    }

     public static Result<T> Failure(string error, HttpStatusCode code, string message)
    {
        return new Result<T>(default, false, code, message, new List<string> { error });
    }
}

public static class Result
{
    public static Result<bool> Success(string message) => Result<bool>.Success(true, message);
    public static Result<bool> Failure(List<string> errors, string message) => Result<bool>.Failure(errors, message);
    public static Result<bool> Failure(string error, string message) => Result<bool>.Failure(error, message);

     // Versiones con HttpStatusCode (Opcional - para API)
     public static Result<bool> Success(HttpStatusCode code, string message) => Result<bool>.Success(true, code, message);
     public static Result<bool> Failure(List<string> errors, HttpStatusCode code, string message) => Result<bool>.Failure(errors, code, message);
     public static Result<bool> Failure(string error, HttpStatusCode code, string message) => Result<bool>.Failure(error, code, message);
}