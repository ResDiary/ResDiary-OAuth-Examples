using System;
using System.Collections.Generic;
using System.Linq;
using RD.EposServiceConsumer.Helpers.BusinessObjects;

namespace RD.EposServiceConsumer.Helpers
{
    public static class Utilities
    {
        public static Segment FindSegmentByDate(IEnumerable<Segment> segments, DateTime date)
        {
            foreach (Segment segment in segments)
            {
                IEnumerable<TimeScope> timeScopes = from t in segment.TimeScopes
                                                    where (t.FromDate <= date) && (t.ToDate > date)
                                                    select t;

                if (timeScopes.Any())
                    return segment;
            }

            return null;
        }
    }
}