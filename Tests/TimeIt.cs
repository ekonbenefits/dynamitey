using System.Diagnostics;

namespace Dynamitey.Tests
{
    public class TimeIt
    {
        public const int Million = 1000000;

        private Stopwatch _watch1;
        private Stopwatch _watch2;
        private Stopwatch _watch3;
        private bool _skipInitializationCosts;
        public TimeIt(bool skipInitializationCosts = false)
        {
            _watch1 = new Stopwatch();
            _watch2 = new Stopwatch();
            _watch3 = new Stopwatch();
            _skipInitializationCosts = skipInitializationCosts;
        }
        public Tuple<TimeSpan, TimeSpan, TimeSpan> GoThree(int iteration = Million, bool useThree = true)
        {
            if (_skipInitializationCosts)
            {
                iteration++;
            }

            for (int i = 0; i < iteration; i++)
            {
                _watch1.Start();
                Action1();
                _watch1.Stop();
                _watch2.Start();
                Action2();
                _watch2.Stop();
                if (useThree)
                {
                    _watch3.Start();
                    Action3();
                    _watch3.Stop();
                }

                if (i == 0 && _skipInitializationCosts)
                {
                    _watch1.Reset();
                    _watch2.Reset();
                    _watch3.Reset();
                }
            }

            return Tuple.Create(_watch1.Elapsed, _watch2.Elapsed, _watch3.Elapsed);

        }
        public Tuple<TimeSpan, TimeSpan> Go(int iteration = Million)
        {
            var goThree = GoThree(iteration, false);
            return Tuple.Create(goThree.Item1, goThree.Item2);
        }

        public Action Action1 { get; set; }
        public Action Action2 { get; set; }
        public Action Action3 { get; set; }

        public static string RelativeSpeed(Tuple<TimeSpan, TimeSpan> elapsed)
        {
            if (
                (elapsed.Item2 > elapsed.Item1 &&
                 (double)elapsed.Item2.Ticks / elapsed.Item1.Ticks < 1.4)
                ||
                (elapsed.Item1 > elapsed.Item2 &&
                 (double)elapsed.Item1.Ticks / elapsed.Item2.Ticks < 1.4)
                )
            {
                Assert.Ignore("Equivalent");
            }


            if (elapsed.Item2 > elapsed.Item1)
                return String.Format(" {0:0.0} x faster", (double)elapsed.Item2.Ticks / elapsed.Item1.Ticks);
            if (elapsed.Item1 > elapsed.Item2)
                return String.Format(" {0:0.0} x slower", (double)elapsed.Item1.Ticks / elapsed.Item2.Ticks);
            return String.Format("Equivalent");
        }
    }
}
