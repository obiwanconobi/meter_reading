using ensek_test.Services;
using NUnit.Framework;
using System.Text;

namespace ensek_test.UnitTests
{
    [TestFixture]
    public class MeterReadingUploadTests
    {
        MeterReadingService service = new MeterReadingService();

        [Test]
        public async Task csvTest()
        {
            var testPath = @"UnitTests/TestData/Meter_Reading.csv";
            try
            {
                using (var fs = new FileStream(testPath, FileMode.Open, FileAccess.Read))
                {
                    var result = await service.ProcessMeterReadingsAsync(fs);

                    Assert.Equals(result.SuccessCount, 25);
                    Assert.Equals(result.FailureCount, 11);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine();
             }
            
            
          
            
        }
    }
}
