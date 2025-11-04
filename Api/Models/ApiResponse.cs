namespace Api.Models {
    public class ApiResponse<T> {
        public string Status { get; set; } = "success";
        public T? Data { get; set; }
        public string? Message { get; set; }

        public static ApiResponse<T> Success(T data, string? message = null) {
            return new ApiResponse<T> {
                Status = "success",
                Data = data,
                Message = message
            };
        }
        public static ApiResponse<T> Failure(string message) {
            return new ApiResponse<T> {
                Status = "failure",
                Data = default,
                Message = message
            };
        }
    }
}
