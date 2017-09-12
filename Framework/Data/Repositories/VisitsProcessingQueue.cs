using System;
using System.Diagnostics;
using System.Threading.Tasks.Dataflow;
using HighLoad.Framework.Data.ReadModel;

namespace HighLoad.Framework.Data.Repositories
{
    public interface IVisitsProcessingQueue
    {
        void Push(Entities.Visit[] visits);
    }

    internal class VisitsProcessingQueue : IVisitsProcessingQueue
    {
        private readonly IExistingEntitiesLookup<Entities.Visit> _existingVisitsLookup;
        private readonly IMarkViewUpdater _markViewUpdater;
        private readonly ActionBlock<Entities.Visit[]> _processingBlock1;
        private readonly ActionBlock<Entities.Visit[]> _processingBlock2;
        private readonly ActionBlock<Entities.Visit[]> _processingBlock3;
        private readonly IVisitViewUpdater _visitViewUpdater;

        public VisitsProcessingQueue(IVisitViewUpdater visitViewUpdater, IMarkViewUpdater markViewUpdater,
            IExistingEntitiesLookup<Entities.Visit> existingVisitsLookup)
        {
            _visitViewUpdater = visitViewUpdater;
            _markViewUpdater = markViewUpdater;
            _existingVisitsLookup = existingVisitsLookup;

            _processingBlock1 = new ActionBlock<Entities.Visit[]>(visits => { _existingVisitsLookup.AddRange(visits); },
                new ExecutionDataflowBlockOptions {MaxDegreeOfParallelism = 3});

            _processingBlock2 = new ActionBlock<Entities.Visit[]>(async visits =>
            {
                var sw = Stopwatch.StartNew();
                try
                {
                    await _visitViewUpdater.AddNewBatch(visits);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

                Console.WriteLine($"Finished processing {visits.Length} visitViews in {sw.Elapsed}");
                Console.WriteLine(DateTime.Now);
            }, new ExecutionDataflowBlockOptions {MaxDegreeOfParallelism = 4});

            _processingBlock3 = new ActionBlock<Entities.Visit[]>(async visits =>
            {
                var sw = Stopwatch.StartNew();
                try
                {
                    await _markViewUpdater.AddNewBatch(visits);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
                Console.WriteLine($"Finished processing {visits.Length} markViews in {sw.Elapsed}");
                Console.WriteLine(DateTime.Now);
            }, new ExecutionDataflowBlockOptions {MaxDegreeOfParallelism = 4});
        }

        public void Push(Entities.Visit[] visits)
        {
            _processingBlock1.Post(visits);
            _processingBlock2.Post(visits);
            _processingBlock3.Post(visits);
        }
    }
}