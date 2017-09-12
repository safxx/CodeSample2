using System;

namespace HighLoad.Application
{
    public static class FilteredResourceResultExtensionMethods
    {
        public static FilteredResourceResult<T> ResourceNotFoundResponse<T>(this FilteredResourceResult<T> filteredResourceResult, Action action)
        {
            if (filteredResourceResult.IsNotFound)
                action();

            return filteredResourceResult;
        }

        public static FilteredResourceResult<T> EmptyResponse<T>(this FilteredResourceResult<T> filteredResourceResult, Action action)
        {
            if (filteredResourceResult.IsSuccess && filteredResourceResult.IsEmpty)
                action();

            return filteredResourceResult;
        }

        public static FilteredResourceResult<T> NonemptyResponse<T>(this FilteredResourceResult<T> filteredResourceResult, Action<FilteredResourceResult<T>> action)
        {
            if (filteredResourceResult.IsSuccess && !filteredResourceResult.IsEmpty)
                action(filteredResourceResult);

            return filteredResourceResult;
        }
    }
}