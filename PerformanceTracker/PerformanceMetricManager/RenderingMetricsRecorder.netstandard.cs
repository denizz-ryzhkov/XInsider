using System.Collections.Generic;
using System.Linq;

namespace PerformanceTracker
{
    public partial class RenderingMetricsRecorder
    {
        internal void PStart()
        {
            throw new NotImplementedInReferenceAssemblyException();
        }

        internal void PStop()
        {
            throw new NotImplementedInReferenceAssemblyException();
        }

        internal IList<FrameMetricsData> PGetFrames()
        {
            throw new NotImplementedInReferenceAssemblyException();
            //return this._storage;
        }

        //private List<IFrameMetricsData> _storage;

        //public RenderingMetricsRecorder()
        //{
        //    //_storage = new List<IFrameMetricsData>(500);
        //}

        //public StartRrecording()
        //{
        //    var pi = CreatePerfEvent();
        //    // todo record here
        //    return pi.Id;
        //}

        //public Guid RecordFrames(TimeSpan recordFramesFor)
        //{
        //    var pi = CreatePerfEvent();
        //    // todo record here
        //    return pi.Id;
        //}

        //private IFrameMetricsData CreatePerfEvent()
        //{
        //    var pi = new FrameMetricsDataData(Guid.NewGuid());
        //    _storage.Add(pi.Id, pi);
        //    return pi;
        //}


        //public IFrameMetricsData GetMetric(Guid id)
        //{
        //    if (_storage.TryGetValue(id, out var pe))
        //    {
        //        return pe;
        //    }

        //    return null;
        //}

        //public IFrameMetricsData[] GetMetrics()
        //{
        //    return _storage.Select(x => x.Value).ToArray();
        //}

    }
}