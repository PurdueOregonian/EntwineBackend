using Friends5___Backend;

namespace Friends5___Backend_Tests
{
    public class UtilsTests
    {
        [Fact]
        public void YearsSince_CurrentDate_ReturnsZero()
        {
            DateOnly today = DateOnly.FromDateTime(DateTime.Now);
            var result = Utils.YearsSince(today);
            Assert.Equal(0, result);

            DateOnly lastYearSameDate = DateOnly.FromDateTime(DateTime.Now).AddYears(-1);
            result = Utils.YearsSince(lastYearSameDate);
            Assert.Equal(1, result);

            DateOnly lastYearOneDayBefore = DateOnly.FromDateTime(DateTime.Now).AddDays(-1).AddYears(-1);
            result = Utils.YearsSince(lastYearOneDayBefore);
            Assert.Equal(1, result);

            DateOnly lastYearOneDayAfter = DateOnly.FromDateTime(DateTime.Now.AddDays(1).AddYears(-1));
            result = Utils.YearsSince(lastYearOneDayAfter);
            Assert.Equal(0, result);

            DateOnly tenYearsAgo = DateOnly.FromDateTime(DateTime.Now).AddYears(-10);
            result = Utils.YearsSince(tenYearsAgo);
            Assert.Equal(10, result);
        }
    }
}