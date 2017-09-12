
namespace HighLoad.Application
{
    public struct CreateCommandResult
    {
        private readonly CreateResult _status;

        static CreateCommandResult()
        {
            Success = new CreateCommandResult(CreateResult.Sucess);
            Failure = new CreateCommandResult(CreateResult.Failure);
        }

        private CreateCommandResult(CreateResult status)
        {
            _status = status;
        }

        public static CreateCommandResult Success { get; }
        public static CreateCommandResult Failure { get; }

        public bool IsFailure => _status == CreateResult.Failure;
        public bool IsSuccess => _status == CreateResult.Sucess;
    }

    enum CreateResult
    {
        None,
        Failure,
        Sucess
    }
}