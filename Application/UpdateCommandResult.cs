namespace HighLoad.Application
{
    public class UpdateCommandResult
    {
        private readonly UpdateResult _status;

        static UpdateCommandResult()
        {
            NotFound = new UpdateCommandResult(UpdateResult.NotFound);
            Success = new UpdateCommandResult(UpdateResult.Success);
            InvalidData = new UpdateCommandResult(UpdateResult.InvalidData);
        }

        public UpdateCommandResult(UpdateResult status)
        {
            _status = status;
        }

        public static UpdateCommandResult NotFound { get; }
        public static UpdateCommandResult Success { get; }
        public static UpdateCommandResult InvalidData { get; }

        public bool IsNotFound => _status == UpdateResult.NotFound;
        public bool IsSuccess => _status == UpdateResult.Success;

        public enum UpdateResult
        {
            None,
            Success,
            NotFound,
            InvalidData
        }
    }
}