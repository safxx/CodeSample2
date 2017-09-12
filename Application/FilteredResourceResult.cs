using System;
using System.Collections.Generic;

namespace HighLoad.Application
{
    public class FilteredResourceResult<T>
    {
        private readonly SearchResultStatus _status;

        private FilteredResourceResult(SearchResultStatus status, IReadOnlyCollection<T> value)
        {
            _status = status;
            Value = value;
        }

        static FilteredResourceResult()
        {
            ResourceNotFound = new FilteredResourceResult<T>(SearchResultStatus.NotFound, Array.Empty<T>());
            Empty = new FilteredResourceResult<T>(SearchResultStatus.Success, Array.Empty<T>());
        }

        public IReadOnlyCollection<T> Value { get; }
        public static FilteredResourceResult<T> ResourceNotFound { get; }
        public static FilteredResourceResult<T> Empty { get; }
        public bool IsEmpty => Value.Count == 0;
        public bool IsNotFound => _status == SearchResultStatus.NotFound;
        public bool IsSuccess => _status == SearchResultStatus.Success;

        public static FilteredResourceResult<T> Success(IReadOnlyCollection<T> result)
        {
            return new FilteredResourceResult<T>(SearchResultStatus.Success, result);
        }

        enum SearchResultStatus
        {
            None,
            NotFound,
            Success
        }
    }
}