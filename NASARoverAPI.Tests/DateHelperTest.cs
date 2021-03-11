using System;
using NASARoverAPI;
using Xunit;

namespace NASARoverAPI.Tests
{
    public class DateHelperTest
    {
        /// <summary>
        /// Test reading the contents of the file.....
        /// </summary>
        [Fact]
        public void GetDatesFromFileAsStringTest()
        {
            try
            {
                var contents = DateHelper.GetDatesFromFileAsString("dates.txt");
                Assert.False(string.IsNullOrEmpty(contents));                

            }
            catch (Exception ex)
            {
                Assert.True(false, "Exception reading file: " + ex.Message);
            }
        }

        /// <summary>
        /// Test parsing of the dates from string representations to DateTime.
        /// Check specific values.
        /// </summary>
        [Fact]
        public void GetDatesFromStringTest()
        {
            var contents = DateHelper.GetDatesFromFileAsString("dates.txt");
            Assert.False(string.IsNullOrEmpty(contents));
            var dates = DateHelper.GetFormattedDatesFromString(contents);
            Assert.True(dates != null);
            Assert.True(dates.Count == 4);

            // Test specific dates below
            Assert.True(DateTime.Compare(new DateTime(2017, 02, 27), dates[0]) == 0);
            Assert.True(DateTime.Compare(new DateTime(2018, 06, 02), dates[1]) == 0);
            Assert.True(DateTime.Compare(new DateTime(2016, 07, 13), dates[2]) == 0);

            // Case for invalid date. There was no April 31st 2018 on calendar.
            // April only goes up to 30th.
            Assert.True(dates[3] != null && dates[3].Ticks == 0); 

        }
    }
}
