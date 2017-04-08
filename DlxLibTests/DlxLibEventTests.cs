using System.Collections.Generic;
using System.Linq;
using System.Threading;
using DlxLib;
using Xunit;

namespace DlxLibTests
{
    public class DlxLibEventTests
    {
        [Theory]
        [InlineData(0, false)]
        [InlineData(1, true)]
        [InlineData(2, true)]
        [InlineData(3, true)]
        public void StartEventFiresOnlyOnceAndOnlyIfWeActuallyStartToEnumerateTheSolutions(int numSolutionsToTake, bool expectStartedEventToBeRaisedOnce)
        {
            var matrix = new[,]
                {
                    {1, 0, 0, 0},
                    {0, 1, 1, 0},
                    {1, 0, 0, 1},
                    {0, 0, 1, 1},
                    {0, 1, 0, 0},
                    {0, 0, 1, 0}
                };
            var dlx = new Dlx();
            var numStartedEventsRaised = 0;
            dlx.Started += (_, __) => numStartedEventsRaised++;

            dlx.Solve(matrix).Take(numSolutionsToTake).ToList();

            Assert.Equal(numStartedEventsRaised, expectStartedEventToBeRaisedOnce ? 1 : 0);
        }

        [Theory]
        [InlineData(0, false)]
        [InlineData(1, true)]
        [InlineData(2, true)]
        [InlineData(3, true)]
        public void FinishedEventFiresOnlyOnceAndOnlyIfWeActuallyStartToEnumerateTheSolutions(int numSolutionsToTake, bool expectFinishedEventToBeRaisedOnce)
        {
            var matrix = new[,]
                {
                    {1, 0, 0, 0},
                    {0, 1, 1, 0},
                    {1, 0, 0, 1},
                    {0, 0, 1, 1},
                    {0, 1, 0, 0},
                    {0, 0, 1, 0}
                };
            var dlx = new Dlx();
            var numFinishedEventsRaised = 0;
            dlx.Finished += (_, __) => numFinishedEventsRaised++;

            dlx.Solve(matrix).Take(numSolutionsToTake).ToList();

            Assert.Equal(numFinishedEventsRaised, expectFinishedEventToBeRaisedOnce ? 1 : 0);
        }

        [Fact]
        public void CancelledEventFiresUsingCancellationToken()
        {
            var matrix = new bool[0, 0];
            var cancellationTokenSource = new CancellationTokenSource();
            var dlx = new Dlx(cancellationTokenSource.Token);
            var cancelledEventHasBeenRaised = false;
            dlx.Cancelled += (_, __) => cancelledEventHasBeenRaised = true;
            dlx.Started += (_, __) => cancellationTokenSource.Cancel();

            var thread = new Thread(() => dlx.Solve(matrix).FirstOrDefault());

            thread.Start();
            thread.Join();

            Assert.True(cancelledEventHasBeenRaised);
        }

        [Fact]
        public void SearchStepEventFires()
        {
            var matrix = new[,]
                {
                    {0, 0, 1, 0, 1, 1, 0},
                    {1, 0, 0, 1, 0, 0, 1},
                    {0, 1, 1, 0, 0, 1, 0},
                    {1, 0, 0, 1, 0, 0, 0},
                    {0, 1, 0, 0, 0, 0, 1},
                    {0, 0, 0, 1, 1, 0, 1}
                };
            var dlx = new Dlx();
            var searchStepEventHasBeenRaised = false;
            dlx.SearchStep += (_, __) => searchStepEventHasBeenRaised = true;

            dlx.Solve(matrix).First();

            Assert.True(searchStepEventHasBeenRaised);
        }

        [Fact]
        public void SearchStepEventsHaveIncreasingIteration()
        {
            var matrix = new[,]
                {
                    {0, 0, 1, 0, 1, 1, 0},
                    {1, 0, 0, 1, 0, 0, 1},
                    {0, 1, 1, 0, 0, 1, 0},
                    {1, 0, 0, 1, 0, 0, 0},
                    {0, 1, 0, 0, 0, 0, 1},
                    {0, 0, 0, 1, 1, 0, 1}
                };
            var dlx = new Dlx();
            var searchStepEventArgs = new List<SearchStepEventArgs>();
            dlx.SearchStep += (_, e) => searchStepEventArgs.Add(e);

            dlx.Solve(matrix).First();

            Assert.True(searchStepEventArgs.Count >= 5);
            foreach (var index in Enumerable.Range(0, 5))
            {
                Assert.Equal(searchStepEventArgs[index].Iteration, index);
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void SolutionFoundEventFiresOnceForEachSolutionTaken(int numSolutionsToTake)
        {
            var matrix = new[,]
                {
                    {1, 0, 0, 0},
                    {0, 1, 1, 0},
                    {1, 0, 0, 1},
                    {0, 0, 1, 1},
                    {0, 1, 0, 0},
                    {0, 0, 1, 0}
                };
            var dlx = new Dlx();
            var solutionFoundEventArgs = new List<SolutionFoundEventArgs>();
            dlx.SolutionFound += (_, e) => solutionFoundEventArgs.Add(e);

            dlx.Solve(matrix).Take(numSolutionsToTake).ToList();

            Assert.Equal(solutionFoundEventArgs.Count, numSolutionsToTake);
            foreach (var index in Enumerable.Range(0, numSolutionsToTake))
            {
                Assert.Equal(solutionFoundEventArgs[index].SolutionIndex, index);
            }
        }
    }
}
