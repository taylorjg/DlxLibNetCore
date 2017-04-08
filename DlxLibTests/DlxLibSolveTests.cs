using System;
using System.Collections.Generic;
using System.Linq;
using DlxLib;
using Xunit;

namespace DlxLibTests
{
    public class DlxLibSolveTests
    {
        [Fact]
        public void NullMatrixThrowsArgumentNullException()
        {
            var dlx = new Dlx();
            Assert.Throws<ArgumentNullException>(() => dlx.Solve<int>(null));
        }

        [Fact]
        public void EmptyMatrixReturnsNoSolutions()
        {
            // Arrange
            var matrix = new bool[0, 0];
            var dlx = new Dlx();

            // Act
            var solutions = dlx.Solve(matrix);

            // Assert
            Assert.Empty(solutions);
        }

        [Fact]
        public void MatrixWithSingleRowOfAllOnesReturnsOneSolution()
        {
            // Arrange
            var matrix = new[,]
                {
                    {1, 1, 1}
                };
            var dlx = new Dlx();

            // Act
            var solutions = dlx.Solve(matrix).ToList();

            // Assert
            AssertSolutions(solutions, new[] {
                new[] { 0 }
            });
        }

        [Fact]
        public void MatrixWithSingleRowOfAllZerosReturnsNoSolutions()
        {
            // Arrange
            var matrix = new[,]
                {
                    {0, 0, 0}
                };
            var dlx = new Dlx();

            // Act
            var solutions = dlx.Solve(matrix).ToList();

            // Assert
            Assert.Empty(solutions);
        }

        [Fact]
        public void SolveWithMatrixOfBool()
        {
            // Arrange
            var matrix = new[,]
                {
                    {true, false, false},
                    {false, true, false},
                    {false, false, true}
                };
            var dlx = new Dlx();

            // Act
            var solutions = dlx.Solve(matrix).ToList();

            // Assert
            AssertSolutions(solutions, new[] {
                new[] { 0, 1, 2 }
            });
        }

        [Fact]
        public void SolveWithMatrixOfInt()
        {
            // Arrange
            var matrix = new[,]
            {
                {1, 0, 0},
                {0, 1, 0},
                {0, 0, 1}
            };
            var dlx = new Dlx();

            // Act
            var solutions = dlx.Solve(matrix).ToList();

            // Assert
            AssertSolutions(solutions, new[] {
                new[] { 0, 1, 2 }
            });
        }

        [Fact]
        public void SolveWithMatrixOfCharAndCustomPredicate()
        {
            // Arrange
            var matrix = new[,]
                {
                    {'X', 'O', 'O'},
                    {'O', 'X', 'O'},
                    {'O', 'O', 'X'}
                };
            var dlx = new Dlx();

            // Act
            var solutions = dlx.Solve(matrix, c => c == 'X').ToList();

            // Assert
            AssertSolutions(solutions, new[] {
                new[] { 0, 1, 2 }
            });
        }

        [Fact]
        public void SolveWithArbitraryDataStucture()
        {
            // Arrange
            var data = new List<Tuple<int[], string>>
                {
                    Tuple.Create(new[] {1, 0, 0}, "Some data associated with row 0"),
                    Tuple.Create(new[] {0, 1, 0}, "Some data associated with row 1"),
                    Tuple.Create(new[] {0, 0, 1}, "Some data associated with row 2")
                };

            // Act
            var solutions = new Dlx().Solve(data, d => d, r => r.Item1).ToList();

            // Assert
            AssertSolutions(solutions, new[] {
                new[] { 0, 1, 2 }
            });
        }

        [Fact]
        public void SolveWithArbitraryDataStuctureAndCustomPredicate()
        {
            // Arrange
            var data = new List<Tuple<char[], string>>
                {
                    Tuple.Create(new[] {'X', 'O', 'O'}, "Some data associated with row 0"),
                    Tuple.Create(new[] {'O', 'X', 'O'}, "Some data associated with row 1"),
                    Tuple.Create(new[] {'O', 'O', 'X'}, "Some data associated with row 2")
                };

            // Act
            var solutions = new Dlx().Solve(data, d => d, r => r.Item1, c => c == 'X').ToList();

            // Assert
            AssertSolutions(solutions, new[] {
                new[] { 0, 1, 2 }
            });
        }

        [Fact]
        public void MatrixFromTheOriginalDancingLinksPaperReturnsTheCorrectSolution()
        {
            // Arrange
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

            // Act
            var solutions = dlx.Solve(matrix).ToList();

            // Assert
            AssertSolutions(solutions, new[] {
                new[] { 0, 3, 4 }
            });
        }

        [Fact]
        public void MatrixWithThreeSolutionsReturnsTheCorrectThreeSolutions()
        {
            // Arrange
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

            // Act
            var solutions = dlx.Solve(matrix).ToList();

            // Assert
            AssertSolutions(solutions, new[] {
                new[] {0, 3, 4},
                new[] {1, 2},
                new[] {2, 4, 5},
            });
        }

        [Fact]
        public void CanGetTheFirstSolutionUsingFirst()
        {
            // Arrange
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
            var numSolutionFoundEventsRaised = 0;
            dlx.SolutionFound += (_, __) => numSolutionFoundEventsRaised++;

            // Act
            var firstSolution = dlx.Solve(matrix).First();

            AssertSolution(firstSolution, new[] {
                new[] {0, 3, 4},
                new[] {1, 2},
                new[] {2, 4, 5},
            });
            Assert.Equal(numSolutionFoundEventsRaised, 1);
        }

        [Fact]
        public void SecondaryColumnsCanBeAllZerosUnlikePrimaryColumns()
        {
            // Arrange
            var matrix = new List<List<int>>
            {
                new List<int> {1, 0, 0, 0, 0, 0}, // 0
                new List<int> {0, 1, 1, 0, 0, 0}, // 1
                new List<int> {1, 0, 0, 1, 0, 0}, // 2
                new List<int> {0, 0, 1, 1, 0, 0}, // 3
                new List<int> {0, 1, 0, 0, 0, 0}, // 4
                new List<int> {0, 0, 1, 0, 0, 0}  // 5
            };
            var dlx = new Dlx();

            // Act
            var solutions = dlx.Solve(matrix, d => d, r => r, 4).ToList();

            // Assert
            AssertSolutions(solutions, new[] {
                new[] {0, 3, 4},
                new[] {1, 2},
                new[] {2, 4, 5},
            });
        }

        [Fact]
        public void SecondaryColumnsCanBeCoveredOnlyOnceLikePrimaryColumns()
        {
            // Arrange
            var matrix = new List<List<int>>
            {
                new List<int> {1, 0, 0, 0, 1}, // 0
                new List<int> {0, 1, 1, 0, 0}, // 1
                new List<int> {1, 0, 0, 1, 0}, // 2
                new List<int> {0, 0, 1, 1, 0}, // 3
                new List<int> {0, 1, 0, 0, 1}, // 4
                new List<int> {0, 0, 1, 0, 1}  // 5
            };
            var dlx = new Dlx();

            // Act
            var solutions = dlx.Solve(matrix, d => d, r => r, 4).ToList();

            // Assert
            AssertSolutions(solutions, new[] {
                new[] {1, 2}
            });
        }

        [Fact]
        public void SolveWithMatrixOfBoolAndNumPrimaryColumns()
        {
            // Arrange
            var matrix = new[,]
                {
                    {true, false, false, true, false},
                    {false, true, false, false, false},
                    {false, false, true, false, false}
                };
            var dlx = new Dlx();

            // Act
            const int numPrimaryColumns = 3;
            var solutions = dlx.Solve(matrix, numPrimaryColumns).ToList();

            // Assert
            AssertSolutions(solutions, new[] {
                new[] {0, 1, 2}
            });
        }

        [Fact]
        public void SolveWithMatrixOfIntAndNumPrimaryColumns()
        {
            // Arrange
            var matrix = new[,]
            {
                {1, 0, 0, 1, 0},
                {0, 1, 0, 0, 0},
                {0, 0, 1, 0, 0}
            };
            var dlx = new Dlx();

            // Act
            const int numPrimaryColumns = 3;
            var solutions = dlx.Solve(matrix, numPrimaryColumns).ToList();

            // Assert
            AssertSolutions(solutions, new[] {
                new[] {0, 1, 2}
            });
        }

        [Fact]
        public void SolveWithMatrixOfCharAndCustomPredicateAndNumPrimaryColumns()
        {
            // Arrange
            var matrix = new[,]
                {
                    {'X', 'O', 'O', 'X', 'O'},
                    {'O', 'X', 'O', 'O', 'O'},
                    {'O', 'O', 'X', 'O', 'O'}
                };
            var dlx = new Dlx();

            // Act
            const int numPrimaryColumns = 3;
            var solutions = dlx.Solve(matrix, c => c == 'X', numPrimaryColumns).ToList();

            // Assert
            AssertSolutions(solutions, new[] {
                new[] {0, 1, 2}
            });
        }

        [Fact]
        public void SolveWithArbitraryDataStuctureAndCustomPredicateAndNumPrimaryColumns()
        {
            // Arrange
            var data = new List<Tuple<char[], string>>
                {
                    Tuple.Create(new[] {'X', 'O', 'O', 'X', 'O'}, "Some data associated with row 0"),
                    Tuple.Create(new[] {'O', 'X', 'O', 'O', 'O'}, "Some data associated with row 1"),
                    Tuple.Create(new[] {'O', 'O', 'X', 'O', 'O'}, "Some data associated with row 2")
                };

            // Act
            const int numPrimaryColumns = 3;
            var solutions = new Dlx().Solve(data, d => d, r => r.Item1, c => c == 'X', numPrimaryColumns).ToList();

            // Assert
            AssertSolutions(solutions, new[] {
                new[] {0, 1, 2}
            });
        }

        private static void AssertSolutions(List<Solution> actualSolutions, IEnumerable<int[]> expectedSolutions)
        {
            Assert.Equal(actualSolutions.Count, expectedSolutions.Count());
            actualSolutions.ForEach(solution => AssertSolution(solution, expectedSolutions));
        }

        private static void AssertSolution(Solution solution, IEnumerable<int[]> expectedSolutions)
        {
            Assert.Equal(expectedSolutions.Count(xs => xs.SequenceEqual(solution.RowIndexes)), 1);
        }
    }
}
