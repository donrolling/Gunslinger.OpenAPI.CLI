namespace Domain
{
	public class OperationResult<T> : OperationResult
	{
		/// <summary>
		/// Contains the results of a successful call. Will be null in many failure scenarios.
		/// </summary>
		public T Result { get; internal set; }

		public OperationResult()
		{ }


		public OperationResult(bool success, string message, T result)
		{
			Success = success;
			Message = message;
			Result = result;
		}

		/// <summary>
		/// This ctor is for copying a non-typed op result to a typed one when you have the result.
		/// </summary>
		/// <param name="operationResult"></param>
		public OperationResult(OperationResult operationResult, T result)
		{
			foreach (var properties in operationResult.GetType().GetProperties())
			{
				properties.SetValue(this, properties.GetValue(operationResult));
			}
			Result = result;
		}

		/// <summary>
		/// This ctor is for copying a non-typed op result to a typed one when you don't have a result.
		/// </summary>
		/// <param name="operationResult"></param>
		public OperationResult(OperationResult operationResult)
		{
			foreach (var properties in operationResult.GetType().GetProperties())
			{
				properties.SetValue(this, properties.GetValue(operationResult));
			}
		}
	}

	public class OperationResult
	{
		public bool Failed { get => !Success; }

		public string Message { get; internal set; }

		public bool Success { get; internal set; }

		public OperationResult()
		{
		}

		public OperationResult(bool success, string message)
		{
			Success = success;
			Message = message;
		}

		/// <summary>
		/// A shortcut to a successful transaction.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="result"></param>
		/// <returns></returns>
		public static OperationResult Ok()
		{
			return new OperationResult
			{
				Success = true
			};
		}

		/// <summary>
		/// A shortcut to a successful transaction when you have a result object.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="result"></param>
		/// <returns></returns>
		public static OperationResult<T> Ok<T>(T result)
		{
			return new OperationResult<T>
			{
				Result = result,
				Success = true
			};
		}

		/// <summary>
		/// A shortcut to a failed transaction. Please provide a meaningful description for the failure.
		/// </summary>
		/// <param name="message"></param>
		/// <returns></returns>
		public static OperationResult Fail(string message)
		{
			return new OperationResult
			{
				Message = message,
				Success = false
			};
		}

		/// <summary>
		/// A shortcut to a failed transaction. Please provide a meaningful description for the failure.
		/// </summary>
		/// <param name="message"></param>
		/// <returns></returns>
		public static OperationResult<T> Fail<T>(string message)
		{
			return new OperationResult<T>
			{
				Message = message,
				Success = false
			};
		}

		/// <summary>
		/// A shortcut to a failed transaction when you have a result object. Please provide a meaningful description for the failure.
		/// </summary>
		/// <param name="message"></param>
		/// <returns></returns>
		public static OperationResult<T> Fail<T>(T result, string message)
		{
			return new OperationResult<T>
			{
				Message = message,
				Result = result,
				Success = false
			};
		}
	}
}