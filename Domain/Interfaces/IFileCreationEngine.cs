namespace Domain.Interfaces
{
	public interface IFileCreationEngine
	{
		OperationResult CleanupOutputDirectory(string directory);

		Task<OperationResult> CreateFileAsync(string destinationPath, string output);

		OperationResult PrepareOutputDirectory(string path, bool deleteAllItemsInOutputDirectory);
	}
}